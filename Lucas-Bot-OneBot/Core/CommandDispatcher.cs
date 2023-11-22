using EleCho.GoCqHttpSdk.Post;
using Lucas_Bot_OneBot.Entities;

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
        }
        catch (ArgumentException e)
        {
            Program.Logger.Info($"无效的指令：{e.Message}");
        }
        await next.Invoke();
    }
}