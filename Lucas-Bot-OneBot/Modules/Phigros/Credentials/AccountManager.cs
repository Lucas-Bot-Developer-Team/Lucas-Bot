using System.Text.RegularExpressions;
using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using Lucas_Bot_OneBot.Core;
using Lucas_Bot_OneBot.Entities;
using Lucas_Bot_OneBot.Helpers;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Lucas_Bot_OneBot.Modules.Phigros.Credentials;

public static class AccountManager
{
    private enum BindState
    {
        SUCCESS,
        ERR_INSTRUCTION_FORMAT,
        ERR_ALREADY_BOUND,
        ERR_INVALID_SESSIONTOKEN
    }

    private enum UnbindState
    {
        SUCCESS,
        ERR_NOT_BOUND,
    }

    private enum AvatarSettingState
    {
        SUCCESS,
        ERR_NOT_BOUND,
        ERR_INSTRUCTION_FORMAT,
    }

    public static async void BindProcessor(Command commandInfo)
    {
        var logger = Program.Logger;

        logger.Info($"{CommandBuilder.DefaultCommandSuffix}bind指令被唤起，使用者：{commandInfo.SenderId}");
        var bindState = BindState.SUCCESS;
        var collection = Utilities.GetCollection("phi", "sessionToken");
        if (commandInfo.Parameters.Count != 1)
            bindState = BindState.ERR_INSTRUCTION_FORMAT;
        logger.Info("通过指令格式校验");

        var sessionToken = commandInfo.Parameters.Any() ? commandInfo.Parameters[0].Trim() : "";
        var regex = new Regex("[a-z0-9]{25}");
        if (bindState == BindState.SUCCESS &&
            !(regex.IsMatch(sessionToken) && sessionToken.Length == 25))
            bindState = BindState.ERR_INVALID_SESSIONTOKEN;
        logger.Info("通过SessionToken格式校验");
        if (bindState == BindState.SUCCESS &&
            await collection.Find(Builders<BsonDocument>.Filter.Eq("qq", commandInfo.SenderId.ToString())).AnyAsync())
            bindState = BindState.ERR_ALREADY_BOUND;
        logger.Info("通过账号校验");

        if (bindState == BindState.SUCCESS)
        {
            await collection.InsertOneAsync(new BsonDocument()
            {
                { "qq", commandInfo.SenderId.ToString() },
                { "sessionToken", sessionToken }
            });
        }

        var hintMessage = bindState switch
        {
            BindState.SUCCESS => "绑定成功。为避免隐私泄露，请您尽快撤回包含sessionToken的信息（若为私聊绑定请忽略）。",
            BindState.ERR_INSTRUCTION_FORMAT => "指令格式无效，请重新输入。若您感到疑惑请查询帮助。",
            BindState.ERR_ALREADY_BOUND => "您已经绑定账号，需要先解绑后才能绑定。",
            BindState.ERR_INVALID_SESSIONTOKEN => "sessionToken格式无效，请重新输入。若您感到疑惑请查询帮助。",
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

        logger.Info($"绑定状态：{bindState.ToString()}");
    }

    public static async void UnbindProcessor(Command commandInfo)
    {
        var logger = Program.Logger;
        logger.Info($"{CommandBuilder.DefaultCommandSuffix}unbind指令被唤起，使用者：{commandInfo.SenderId}");

        var unbindState = UnbindState.SUCCESS;
        var collection = Utilities.GetCollection("phi", "sessionToken");
        var queryResult = collection.Find(Builders<BsonDocument>.Filter.Eq("qq", commandInfo.SenderId.ToString()));

        if (!await queryResult.AnyAsync())
            unbindState = UnbindState.ERR_NOT_BOUND;

        if (unbindState == UnbindState.SUCCESS)
        {
            await collection.DeleteManyAsync(Builders<BsonDocument>.Filter.Eq("qq", commandInfo.SenderId.ToString()));
        }

        var hintMessage = unbindState switch
        {
            UnbindState.SUCCESS => "解绑成功。",
            UnbindState.ERR_NOT_BOUND => "您未绑定账号。",
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

        logger.Info($"解绑状态：{unbindState}");
    }

    public static async void AvatarSettingProcessor(Command commandInfo)
    {
        var logger = Program.Logger;
        logger.Info($"/avatar指令被唤起，使用者：{commandInfo.SenderId}");
        var avatarState = false;
        var hintMessage = "";

        var avatarSettingState = AvatarSettingState.SUCCESS;
        var collection = Utilities.GetCollection("phi", "sessionToken");
        var queryResult = collection.Find(Builders<BsonDocument>.Filter.Eq("qq", commandInfo.SenderId.ToString()));

        if (!await queryResult.AnyAsync())
            avatarSettingState = AvatarSettingState.ERR_NOT_BOUND;
        if (commandInfo.Parameters.Count != 1 ||
            !(commandInfo.Parameters[0].Trim().ToLower().Equals("on")
             || commandInfo.Parameters[0].Trim().ToLower().Equals("off")))
        {
            avatarSettingState = AvatarSettingState.ERR_INSTRUCTION_FORMAT;
        }
        else
        {
            avatarState = commandInfo.Parameters[0].Trim().ToLower() switch
            {
                "on" => true,
                "off" => false,
                _ => throw new ArgumentOutOfRangeException(nameof(commandInfo))
            };
        }

        if (avatarSettingState == AvatarSettingState.SUCCESS)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("qq", commandInfo.SenderId.ToString());
            var update = Builders<BsonDocument>.Update.Set("useGameAvatar", avatarState);
            var result = await collection.UpdateManyAsync(filter, update);
            logger.Info($"更改的数据库对象：{result.ModifiedCount}");
            hintMessage = avatarState switch
            {
                true => "设置成功。当您下次查分时，将会使用游戏内头像。",
                false => "设置成功。当您下次查分时，将会使用您的QQ头像。"
            };
        }

        hintMessage = avatarSettingState switch
        {
            AvatarSettingState.SUCCESS => hintMessage,
            AvatarSettingState.ERR_NOT_BOUND => "您未绑定账号。",
            AvatarSettingState.ERR_INSTRUCTION_FORMAT => $"指令格式错误，应为{CommandBuilder.DefaultCommandSuffix}avatar <on|off>",
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

        logger.Info($"解绑状态：{avatarSettingState.ToString()}");
    }
}