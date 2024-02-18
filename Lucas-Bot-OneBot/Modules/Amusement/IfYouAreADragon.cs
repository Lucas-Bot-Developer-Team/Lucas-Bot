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
using Lucas_Bot_OneBot.Helpers;

namespace Lucas_Bot_OneBot.Modules.Amusement;

internal static class IfYouAreADragon
{
    private static readonly List<string> DragonPictures = [];

    static IfYouAreADragon()
    {
        var files = Directory.GetFiles("assets/long");
        var extension = new List<string> { ".jpg", ".png" };
        if (Program.GetDeployedPlatformType() == PlatformType.OPEN_SHAMROCK)
            extension.Add( ".gif" );
        foreach (var file in files)
        {
            if (File.Exists(file) &&
                extension.Contains(Path.GetExtension(file).ToLower()))
            {
                DragonPictures.Add(Path.GetFullPath(file));
            }
        }

    }

    public static async void DragonPictureProcessor(Command commandInfo)
    {
        var logger = Program.Logger;
        logger.Info($"{CommandBuilder.DefaultCommandSuffix}long指令被唤起，使用者：{commandInfo.SenderId}");
        try
        {
            var imagePath = DragonPictures[Random.Shared.Next(0, DragonPictures.Count - 1)];
            logger.Info($"图片路径: {imagePath}");
            var imageStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var imageMessage = CqImageMsg.FromStream(imageStream);
            // var imageMessage = CqImageMsg.FromFile(imagePath);
            await Program.HttpSession.GenericReplyMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
                commandInfo.GroupId, commandInfo.MessageId,
                new CqMessage(imageMessage), withReply: false);
            logger.Info("发送图片成功");
        }
        catch (Exception ex)
        {
            logger.Error("出现异常：", ex);
        }
    }
}
