using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using Lucas_Bot_OneBot.Core;
using Lucas_Bot_OneBot.Entities;
using SkiaSharp;

namespace Lucas_Bot_OneBot.Modules.Phigros.Services;

internal static class AssetsHelpers
{
    private enum AssetsQueryState
    {
        SUCCESS,
        ERR_NOT_BOUND,
        ERR_INTERNAL,
        ERR_INSTRUCTION_FORMAT
    }

    public static async void SongInfoProcessor(Command commandInfo)
    {
        var logger = Program.Logger;
        var suggestsState = AssetsQueryState.SUCCESS;
        var hintMessage = "";

        logger.Info($"{CommandBuilder.DefaultCommandSuffix}song指令被唤起，使用者：{commandInfo.SenderId}");
        logger.Info("从数据库中查询歌曲信息");

        try
        {
            var result = AssetsHelper.QuerySongInfoFromInput(string.Join(' ', commandInfo.Parameters));
            var aliases = AssetsHelper.ReverseQueryAliasFromSongId(result.SongId);
            var aliasString = "[ " + string.Join("; ", aliases) + " ]";

            var image = SKBitmap.Decode(result.Illustration);
            var stream = new MemoryStream();
            image.Encode(stream, SKEncodedImageFormat.Png, 50);

            var replyMessage = new CqMessage(new CqReplyMsg(commandInfo.MessageId))
        {
            $"歌曲名称: {result.SongName}\n",
            $"歌曲ID: {result.SongId}\n",
            $"曲师: {result.Composer}\n",
            $"曲绘画师: {result.Illustrator}\n",
            $"别名: {aliasString}\n",
            $"各难度信息：\n"
        };
            foreach (var chart in result.Charts)
            {
                replyMessage.Add($"[ 难度: {chart.Key}; ");
                replyMessage.Add($"定数: {chart.Value.Difficulty:F1}; ");
                replyMessage.Add($"谱师: {chart.Value.Charter} ]\n");
            }
            logger.Info($"曲绘绝对路径：{Path.GetFullPath(result.Illustration)}");
            await Program.HttpSession.SendMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
                   commandInfo.GroupId, replyMessage);
            await Program.HttpSession.SendMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
                   commandInfo.GroupId, new CqMessage(CqImageMsg.FromBytes(stream.ToArray())));
        }
        catch (PhigrosAPIException.PhigrosAPIException e)
        {
            logger.Error("FSharp层出现异常", e);
            hintMessage = $"[PhigrosAPIException]\n{e.Data0}";
            suggestsState = AssetsQueryState.ERR_INTERNAL;
        }
        catch (Exception e)
        {
            logger.Error("出现其他异常：", e);
            hintMessage = e.Message;
            suggestsState = AssetsQueryState.ERR_INTERNAL;
        }

        hintMessage = suggestsState switch
        {
            AssetsQueryState.SUCCESS => hintMessage,
            AssetsQueryState.ERR_NOT_BOUND => $"您未绑定。请先使用{CommandBuilder.DefaultCommandSuffix}bind指令完成绑定。",
            AssetsQueryState.ERR_INTERNAL => hintMessage,
            AssetsQueryState.ERR_INSTRUCTION_FORMAT => throw new ArgumentOutOfRangeException(nameof(commandInfo)),
            _ => throw new ArgumentOutOfRangeException(nameof(commandInfo))
        };

        try
        {
            if (suggestsState != AssetsQueryState.SUCCESS)
                await Program.HttpSession.SendMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
                    commandInfo.GroupId,
                    new CqMessage(new CqReplyMsg(commandInfo.MessageId), new CqMessage(hintMessage)));
        }
        catch (Exception ex)
        {
            logger.Error("出现异常：", ex);
        }

        logger.Info($"song 查询状态：{suggestsState}");
    }
}