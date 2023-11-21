﻿using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using Lucas_Bot_OneBot.Core;
using Lucas_Bot_OneBot.Entities;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas_Bot_OneBot.Modules.Amusement;

internal static class IfYouAreADragon
{
    private static List<string> dragonPictures = new();

    static IfYouAreADragon()
    {
        var files = Directory.GetFiles("assets/long");
        foreach (var file in files)
        {
            if (File.Exists(file) &&
                new List<string>()
                { ".jpg", ".png", ".gif" }.Contains(Path.GetExtension(file).ToLower()))
            {
                dragonPictures.Add(Path.GetFullPath(file));
            }
        }

    }

    public static async void DragonPictureProcessor(Command commandInfo)
    {
        var logger = Program.Logger;
        logger.Info($"{CommandBuilder.DefaultCommandSuffix}long指令被唤起，使用者：{commandInfo.SenderId}");
        try
        {
            var imagePath = dragonPictures[Random.Shared.Next(0, dragonPictures.Count - 1)];
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