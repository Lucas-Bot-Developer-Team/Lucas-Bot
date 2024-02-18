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

using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Action;
using EleCho.GoCqHttpSdk.Message;
using Lucas_Bot_OneBot.Core;
using Lucas_Bot_OneBot.Entities;

namespace Lucas_Bot_OneBot.Helpers;

public static class GenericMessageHandler
{
    public static async Task<CqSendMessageActionResult?> GenericReplyMessageAsync
        (this ICqActionSession session, CqMessageType messageType, long? userId, long? groupId, long messageId, CqMessage message,
            bool withReply = true)
    {
        // Post-process CqMessage
        var platformType = Program.GetDeployedPlatformType();
        Program.Logger.Info($"Message Type = {messageType}");

        if (withReply)
        {
            if (platformType is PlatformType.GENSOKYO_DISCORD 
                or PlatformType.LAGRANGE_CORE
                or PlatformType.QQ_GUILD) // 修正 Discord (Gensokyo-Discord) 平台的回复问题
            {
                if (messageType == CqMessageType.Group)
                {
                    message = message.WithHeads(new CqAtMsg(userId!.Value), "\n"); // 添加一个换行回复
                }
            }
            else
            {
                message = message.WithHead(new CqReplyMsg(messageId));
            }
        }
        
        // Call SendGroupMessageAsync (Wrap SendGroupMessageAsync calls, cross platforms compability)
        switch (messageType)
        {
            case CqMessageType.Group:
                var groupResult = await session.SendGroupMessageAsync(groupId!.Value, message);
                return groupResult;
            case CqMessageType.Private:
                var privateResult = await session.SendPrivateMessageAsync(userId!.Value, message);
                return privateResult;
            case CqMessageType.Unknown:
            default:
                var defaultResult = await session.SendMessageAsync(messageType, userId, groupId, message);
                return defaultResult;
        }
    }
}