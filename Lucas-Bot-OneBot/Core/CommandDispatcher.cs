//      ___          ___          ___          ___          ___          ___          ___     
//     /\  \        /\__\        /\  \        /\  \        /\__\        /\__\        /\  \    
//    /::\  \      /:/  /        \:\  \      /::\  \      /::|  |      /::|  |      /::\  \   
//   /:/\ \  \    /:/  /          \:\  \    /:/\:\  \    /:|:|  |     /:|:|  |     /:/\:\  \  
//  _\:\~\ \  \  /:/  /  ___       \:\  \  /::\~\:\  \  /:/|:|  |__  /:/|:|  |__  /::\~\:\  \ 
// /\ \:\ \ \__\/:/__/  /\__\_______\:\__\/:/\:\ \:\__\/:/ |:| /\__\/:/ |:| /\__\/:/\:\ \:\__\
// \:\ \:\ \/__/\:\  \ /:/  /\::::::::/__/\/__\:\/:/  /\/__|:|/:/  /\/__|:|/:/  /\:\~\:\ \/__/
//  \:\ \:\__\   \:\  /:/  /  \:\~~\~~         \::/  /     |:/:/  /     |:/:/  /  \:\ \:\__\  
//   \:\/:/  /    \:\/:/  /    \:\  \          /:/  /      |::/  /      |::/  /    \:\ \/__/  
//    \::/  /      \::/  /      \:\__\        /:/  /       /:/  /       /:/  /      \:\__\    
//     \/__/        \/__/        \/__/        \/__/        \/__/        \/__/        \/__/    


using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Post;
using Lucas_Bot_OneBot.Entities;
using Lucas_Bot_OneBot.Helpers;

namespace Lucas_Bot_OneBot.Core;

public static class CommandDispatcher
{
    private static Dictionary<string, CommandHandler> CommandHandlers { get; } = new();

    // TODO: 增加指令分组（虽然我也不知道多久能写完，但是先放一个在这儿，万一有人闲得无聊翻repo看到了呢）
    public static void RegisterCommandHandler(string commandTrigger, Action<Command> commandHandler, CommandHandlerType handlerType = CommandHandlerType.GROUP_AND_PRIVATE)
        => CommandHandlers.Add(commandTrigger, new CommandHandler(commandTrigger, commandHandler, handlerType));

    public static async Task GroupCommandDispatchMiddleware(CqGroupMessagePostContext context, Func<Task> next)
    {
        
        Program.Logger.Info($"收到 Channel {context.GroupId} 发送者 {context.UserId} 发送的消息");
        try
        {
            var command = CommandBuilder.BuildCommand(context);
            if (CommandHandlers.TryGetValue(command.CommandTrigger, out var entry))
            {
                Program.Logger.Info($"触发指令: {command.CommandTrigger}");
                Program.Logger.Info($"指令类型: {entry.Type}");
                if (entry.Type != CommandHandlerType.PRIVATE_ONLY)
                    entry.Handler.Invoke(command);
            }
            else
            {
                await DefaultCommandTrigger(command);
            }
        }
        catch (ArgumentException e)
        {
            Program.Logger.Info($"无效的指令：{e.Message}");
        }
        await next.Invoke();
    }

    public static async Task PrivateCommandDispatchMiddleware(CqPrivateMessagePostContext context, Func<Task> next)
    {
        Program.Logger.Info($"收到 发送者 {context.UserId} 发送的消息");
        try
        {
            var command = CommandBuilder.BuildCommand(context);
            if (CommandHandlers.TryGetValue(command.CommandTrigger, out var entry))
            {
                Program.Logger.Info($"触发指令： {command.CommandTrigger}");
                Program.Logger.Info($"指令类型: {entry.Type}");
                if (entry.Type != CommandHandlerType.GROUP_ONLY)
                    entry.Handler.Invoke(command);
            }
            else
            {
                await DefaultCommandTrigger(command);
            }
        }
        catch (ArgumentException e)
        {
            Program.Logger.Info($"无效的指令：{e.Message}");
        }
        await next.Invoke();
    }

    private static async Task DefaultCommandTrigger(Command commandInfo)
    {
        var logger = Program.Logger;
        const int filter = 4;
        var hasPossibleEntry = false;
        
        // QQ 频道 - 公域机器人兜底回复，告知审核已无隐藏指令，防止打回
        // （ 防止根据算法查找所有已有指令时发现隐藏指令 ）
        if (Program.GetDeployedPlatformType() is PlatformType.QQ_GUILD)
        {
            logger.Info($"未知指令 {commandInfo.CommandTrigger}，触发频道机器人兜底回复");
            await Program.HttpSession.GenericReplyMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
                commandInfo.GroupId, commandInfo.MessageId,
                new CqMessage("你输入的指令不对哦，请查看机器人帮助列表获取可用指令"));
            return;
        }
        
        logger.Info($"未知指令 {commandInfo.CommandTrigger}，根据Levenshtein Distance算法寻找所有可能结果");
        var hintMessage = "您可能在寻找以下指令: \n";
        foreach (var kvPair in CommandHandlers)
        {
            var distance = Utilities.GetLevenshteinDistance(commandInfo.CommandTrigger, kvPair.Key);
            if (distance > filter) continue;
            logger.Info($"{commandInfo.CommandTrigger} -> {kvPair.Key}: distance = {distance}");
            hasPossibleEntry = true;
            hintMessage += $"{CommandBuilder.DefaultCommandSuffix}{kvPair.Key} (相似度: {(filter - distance) / 2.0:F1})\n";
        }
        hintMessage = hintMessage.Remove(hintMessage.Length - 1);

        try
        {
            if (hasPossibleEntry)
                await Program.HttpSession.GenericReplyMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
                    commandInfo.GroupId, commandInfo.MessageId,
                    new CqMessage(hintMessage));
        }
        catch (Exception ex)
        {
            logger.Error("出现异常：", ex);
        }
    }
}