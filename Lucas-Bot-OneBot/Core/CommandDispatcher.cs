using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Post;
using Lucas_Bot_OneBot.Entities;
using Lucas_Bot_OneBot.Helpers;

namespace Lucas_Bot_OneBot.Core;

public static class CommandDispatcher
{
    private static Dictionary<string, Action<Command>> CommandHandlers { get; } = new();

    public static void RegisterCommandHandler(string commandTrigger, Action<Command> commandHandler)
        => CommandHandlers.Add(commandTrigger, commandHandler);

    public static async Task GroupCommandDispatchMiddleware(CqGroupMessagePostContext context, Func<Task> next)
    {
        Program.Logger.Info($"收到群 {context.GroupId} 发送者 {context.UserId} 发送的消息");
        try
        {
            var command = CommandBuilder.BuildCommand(context);
            if (CommandHandlers.TryGetValue(command.CommandTrigger, out var entry))
            {
                Program.Logger.Info($"触发指令： {command.CommandTrigger}");
                entry.Invoke(command);
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
                entry.Invoke(command);
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

    public static async Task DefaultCommandTrigger(Command commandInfo)
    {
        var logger = Program.Logger;
        var filter = 4;
        var hasPossibleEntry = false;
        logger.Info($"未知指令 {commandInfo.CommandTrigger}，根据Levenshtein Distance算法寻找所有可能结果");
        var hintMessage = "您可能在寻找以下指令: \n";
        foreach (var kvPair in CommandHandlers)
        {
            var distance = Utilities.GetLevenshteinDistance(commandInfo.CommandTrigger, kvPair.Key);
            if (distance <= filter)
            {
                logger.Info($"{commandInfo.CommandTrigger} -> {kvPair.Key}: distance = {distance}");
                hasPossibleEntry = true;
                hintMessage += $"{CommandBuilder.DefaultCommandSuffix}{kvPair.Key} (相似度: {filter - distance})\n";
            }
        }
        hintMessage = hintMessage.Remove(hintMessage.Length - 1);

        try
        {
            if (hasPossibleEntry)
                await Program.HttpSession.SendMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
                    commandInfo.GroupId,
                    new CqMessage(new CqReplyMsg(commandInfo.MessageId), new CqMessage(hintMessage)));
        }
        catch (Exception ex)
        {
            logger.Error("出现异常：", ex);
        }
    }
}