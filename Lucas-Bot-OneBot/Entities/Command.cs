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

namespace Lucas_Bot_OneBot.Entities;
using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;

public struct Command
{
    /// <summary>
    /// 触发的指令
    /// </summary>
    public string CommandTrigger;

    /// <summary>
    /// 指令包含的参数
    /// </summary>
    public List<string> Parameters;

    /// <summary>
    /// 消息来源类型
    /// </summary>
    public CqMessageType MessageType;

    /// <summary>
    /// 消息发送者
    /// </summary>
    public long SenderId;

    /// <summary>
    /// 消息来源群聊（可选）
    /// </summary>
    public long? GroupId;

    /// <summary>
    /// 原消息引用
    /// </summary>
    public CqMessage OrigMessage;

    /// <summary>
    /// 消息ID（回复时需要）
    /// </summary>
    public long MessageId;
}