using EleCho.GoCqHttpSdk.Post;
using Lucas_Bot_OneBot.Entities;

namespace Lucas_Bot_OneBot.Core;

public static class CommandBuilder
{
    private static HashSet<char> CommandPrefix { get; } = new();

    public static void RegisterCommandPrefix(char prefix) => CommandPrefix.Add(prefix);

    public static Command BuildCommand(CqMessagePostContext context)
    {
        var command = new Command
        {
            MessageType = context.MessageType,
            OrigMessage = context.Message,
            SenderId = context.UserId,
            // 若消息来自群聊，置GroupId为群号，否则置为null
            GroupId = context is CqGroupMessagePostContext groupContext ? groupContext.GroupId : null,
            MessageId = context.MessageId
        };

        var content = context.Message.Text.Trim();

        // 判断有效指令：是否以指令触发符开头
        if (!content.Any() || !CommandPrefix.Any(prefix => content.StartsWith(prefix)))
        {
            throw new ArgumentException("此上下文不是有效的指令", nameof(context));
        }

        // 按空格切割消息内容
        var slicedContentEnumerable =
            from slice in content.Split(' ')
            where slice.Length != 0
            select slice;

        // 过滤掉仅有一个指令触发符的指令
        var slicedContent = slicedContentEnumerable as string[] ?? slicedContentEnumerable.ToArray();
        if (slicedContent.FirstOrDefault() is null
            || slicedContent.FirstOrDefault()!.Length == 1)
        {
            throw new ArgumentException("指令格式无效", nameof(context));
        }

        command.CommandTrigger = slicedContent[0][1..].ToLower();
        command.Parameters = slicedContent.Length == 1
            ? []
            : [.. slicedContent[1..]];

        return command;
    }

    public static char DefaultCommandSuffix => CommandPrefix is not { Count: 0 } ? CommandPrefix.First() : '/';
}