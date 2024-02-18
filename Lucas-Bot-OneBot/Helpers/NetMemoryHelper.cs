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
using EleCho.GoCqHttpSdk.Message;
using Lucas_Bot_OneBot.Core;
using Lucas_Bot_OneBot.Entities;

namespace Lucas_Bot_OneBot.Helpers;

public static class NetMemoryHelper
{
    public static async void GcProcessor(Command commandInfo)
    {
        if (commandInfo.SenderId.ToString() != Program.GetManagerId())
        {
            await Program.HttpSession.GenericReplyMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
                commandInfo.GroupId, commandInfo.MessageId,
                new CqMessage("很抱歉，您无权使用本指令。"));
            return;
        }
        GC.Collect();
        await Program.HttpSession.GenericReplyMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
            commandInfo.GroupId, commandInfo.MessageId,
            new CqMessage($"已执行手动垃圾回收，当前 Bot 进程占用内存 {Utilities.GetUsedMemory():F2} MB"));
    }
}