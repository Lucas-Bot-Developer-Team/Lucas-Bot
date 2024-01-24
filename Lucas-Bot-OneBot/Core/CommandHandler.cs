using Lucas_Bot_OneBot.Entities;

namespace Lucas_Bot_OneBot.Core;

public enum CommandHandlerType
{
    GROUP_ONLY,
    PRIVATE_ONLY,
    GROUP_AND_PRIVATE
}

internal record struct CommandHandler(string Name, Action<Command> Handler, CommandHandlerType Type);