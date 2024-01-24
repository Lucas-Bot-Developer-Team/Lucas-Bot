using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using Lucas_Bot_OneBot.Core;
using Lucas_Bot_OneBot.Entities;

namespace Lucas_Bot_OneBot.Modules.Amusement;

internal static class IfYouAreADragon
{
    private static readonly List<string> DragonPictures = [];

    static IfYouAreADragon()
    {
        var files = Directory.GetFiles("assets/long");
        foreach (var file in files)
        {
            if (File.Exists(file) &&
                new List<string>()
                { ".jpg", ".png", ".gif" }.Contains(Path.GetExtension(file).ToLower()))
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
            await Program.HttpSession.SendMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
                commandInfo.GroupId,
                new CqMessage(imageMessage));
            logger.Info("发送图片成功");
        }
        catch (Exception ex)
        {
            logger.Error("出现异常：", ex);
        }
    }
}
