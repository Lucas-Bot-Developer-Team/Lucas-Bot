using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk;
using Lucas_Bot_OneBot.Core;
using Lucas_Bot_OneBot.Entities;
using System.Text;

namespace Lucas_Bot_OneBot.Modules.Amusement.YiYan;

public static class YiYanProvider
{
    // TODO: 想到啥就写啥吧，最近实在想不出啥想写的

    public static string GetTextMsg(this CqEssenceMessage essenceMessage)
    {
        var messageList = essenceMessage.MessageContent.ToList();
        var resultStrBuilder = new StringBuilder();
        resultStrBuilder.AppendLine($"发送者: {essenceMessage.SenderNickname}");
        resultStrBuilder.AppendLine($"设精者: {essenceMessage.OperatorNickname}");
        resultStrBuilder.AppendLine($"设精时间: {essenceMessage.OperationTime}\n");
        foreach (var message in messageList)
        {
            switch ((int)message!["msg_type"]!)
            {
                case 1: // TextMsg
                    var content = message!["text"]!.ToString();
                    resultStrBuilder.Append(content);
                    if (content.StartsWith('@'))
                        resultStrBuilder.Append(' ');
                    break;
                case 3: // ImageMsg
                    resultStrBuilder.Append("[图片] ");
                    break;
                case 4: // FileMsg
                    resultStrBuilder.Append($"[文件 {message!["file_name"]!}]");
                    break;
                default:
                    break;
            }
        }
        return resultStrBuilder.ToString();
    }

    private static Dictionary<long, IEnumerable<CqEssenceMessage>> _essenceMessageList = new();

    public static async Task<IEnumerable<CqEssenceMessage>> GetEssenceMessageListAsync(long groupId)
    {
        if (_essenceMessageList.TryGetValue(groupId, out var list))
        {
            return list;
        }
        
        var messageList = new List<CqEssenceMessage>();
        for (var i = 0; i < 50; ++i)
        {
            Program.Logger.Info($"初始化精华消息: {groupId}, 页 {i}");
            var queryResult = await Program.HttpSession.GetEssenceMessageListAsync(groupId, i);
            if (queryResult is not null && queryResult.Messages.Count > 0)
                messageList.AddRange(queryResult.Messages);
            if (queryResult!.Messages.Count < 20)
                break;
        }
        _essenceMessageList.Add(groupId, messageList);
        return messageList;
    }

    public static async Task<CqEssenceMessage?> TakeSingleEssenceRandomly(long groupId, long atPersonId = 0)
    {
        var essenceMessageList = await GetEssenceMessageListAsync(groupId);
        var cqEssenceMessages = essenceMessageList.ToList();
        if (cqEssenceMessages is { Count: 0 }) return null;
        if (atPersonId != 0)
        {
            cqEssenceMessages = cqEssenceMessages.FindAll(x => x.SenderId == atPersonId);
            if (cqEssenceMessages is { Count: 0 }) return null;
        }
        var randomNumber = Random.Shared.Next(cqEssenceMessages.Count - 1);
        var randomEssenceMessage = cqEssenceMessages[randomNumber];
        Program.Logger.Info("获取精华消息成功");
        Program.Logger.Info(randomEssenceMessage.GetTextMsg());
        return randomEssenceMessage;
    }

    private enum YiYanTestState
    {
        SUCCESS,
        ERR_OTHER
    }

    public static async void YiYanProcessorTest(Command commandInfo)
    {
        var logger = Program.Logger;
        logger.Info($"{CommandBuilder.DefaultCommandSuffix}yiyan指令被唤起，使用者：{commandInfo.SenderId}");

        long atPersonId = 0;
        var atMsg = commandInfo.OrigMessage.Find(x => x.MsgType.Equals("at"));

        if (atMsg is CqAtMsg atMessage)
        {
            atPersonId = atMessage.IsAtAll ? 0 : atMessage.Target;
        }
        
        var yiyanState = YiYanTestState.SUCCESS;
        var yiyan = "";

        try 
        {
            var groupId = commandInfo.GroupId!.Value;
            var essenceMsg = await TakeSingleEssenceRandomly(groupId, atPersonId);
            yiyan = essenceMsg!.GetTextMsg();
        }
        catch (Exception)
        {
            yiyanState = YiYanTestState.ERR_OTHER;
        }

        var hintMessage = yiyanState switch
        {
            YiYanTestState.SUCCESS => yiyan,
            YiYanTestState.ERR_OTHER => "本群无精华消息，或Bot被部署在不具有此接口的平台上",
            _ => throw new ArgumentOutOfRangeException(nameof(commandInfo))
        };

        try
        {
            await Program.HttpSession.SendMessageAsync(commandInfo.MessageType, commandInfo.SenderId, commandInfo.GroupId,
                new CqMessage(new CqReplyMsg(commandInfo.MessageId), new CqMessage(hintMessage)));
        }
        catch (Exception ex)
        {
            logger.Error("出现异常：", ex);
        }

        logger.Info($"查询精华消息：{yiyanState}");
    }
    
}