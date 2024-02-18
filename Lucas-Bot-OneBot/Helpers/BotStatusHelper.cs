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

using System.Reflection;
using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using Lucas_Bot_OneBot.Core;
using Lucas_Bot_OneBot.Entities;
using System.Runtime.InteropServices;

namespace Lucas_Bot_OneBot.Helpers;

internal static class BotStatusHelper
{

    public static string GetVersion()
    {
        var version = Assembly.GetEntryAssembly()!.GetName().Version!;
        return $"{version.Major}.{version.Minor}.{version.Build}";
    }

    private static readonly List<string> Tips = [];

    public static TimeSpan ScheduledRebootTime { get; set; }

    public static async void HelpProcessor(Command command)
    {
        if (Program.GetDeployedPlatformType() is PlatformType.QQ_GUILD)
        {
            await Program.HttpSession.GenericReplyMessageAsync(command.MessageType, command.SenderId, command.GroupId, command.MessageId,
                new CqMessage(
                    $"Suzanne-Phigros 试运行 ver {GetVersion()}\n" +
                    "请查看指令列表以获取帮助"), withReply: false);
        }
        else
        {
            await Program.HttpSession.GenericReplyMessageAsync(command.MessageType, command.SenderId, command.GroupId, command.MessageId,
                new CqMessage(
                    $"Suzanne-Phigros 试运行 ver {GetVersion()}\n" +
                    "帮助已迁移: https://www.yuque.com/lucas1522/sxcczd"), withReply: false);
        }
    }

    public static async void AboutProcessor(Command command)
    {
        lock (Tips)
        {
            if (Tips.Count == 0)
            {
                var gameTips = File.ReadAllLines("assets/phigros-ingame-statistics/tips.txt");
                Tips.AddRange(gameTips);
            }
        }
        var rd = new Random((int)DateTime.Now.Ticks);
        // ReSharper disable once InconsistentlySynchronizedField
        var index = rd.Next(Tips.Count);
        await Program.HttpSession.GenericReplyMessageAsync(command.MessageType, command.SenderId, 
            command.GroupId, command.MessageId,
            new CqMessage(
                $"Suzanne-Phigros 试运行 ver {GetVersion()}\n" +
                $"Powered by {RuntimeInformation.FrameworkDescription}\n" +
                
                // ReSharper disable once InconsistentlySynchronizedField
                $"Tip: {Tips[index]}"), withReply: false);
    }

    public static async void StatusProcessor(Command command)
    {
        var cpuUsage = await Utilities.GetCpuUsageForProcess();
        // await Program.HttpSession.SendPrivateMessageAsync(command.SenderId, new CqMessage("test"));
        await Program.HttpSession.GenericReplyMessageAsync(command.MessageType, command.SenderId, command.GroupId,
            command.MessageId,
            new CqMessage("服务在线\n" +
            $"Runtime Identifier: {RuntimeInformation.RuntimeIdentifier}\n" +
            $"Operating System: {Environment.OSVersion.VersionString}\n" +
            $"Architecture: {RuntimeInformation.OSArchitecture}\n" +
            $".NET Runtime: {RuntimeInformation.FrameworkDescription}\n" +
            $"CPU Model: {CpuInformationProvider.GetCpuInfo()}\n" + 
            $"CPU Usage: {cpuUsage:F3} %\n" +
            $"Memory Allocated: {Utilities.GetUsedMemory():F2} MB\n" +
            // Disable GC Information (Not Correct)
            // $"GC Information: Heap size = {GC.GetTotalMemory(true) / 1048576.0:F2} MB; Collection Count = {GC.CollectionCount(0)}/{GC.CollectionCount(1)}/{GC.CollectionCount(2)}\n" +
            $"Thread Count: {ThreadPool.ThreadCount}\n" +
            $"Uptime: {Program.GetRunTime():c}\n" +
            $"Deployed Platform Type: {Program.GetDeployedPlatformType()}" +
            (Program.GetConnectionType() is not ConnectionType.HTTP ? 
                $"\nWebSocket Heartbeat Count : {WebSocketHeartbeat.GetHeartBeatCount()}" 
                : "")
            ), withReply: false);
    }

    public static async void RebootProcessor(Command command)
    {
        if (command.SenderId.ToString() != Program.GetManagerId())
        {
            await Program.HttpSession.GenericReplyMessageAsync(command.MessageType, command.SenderId, command.GroupId,
                command.MessageId, new CqMessage("很抱歉，您无权使用本指令。"), withReply: false
                );
            return;
        }

        await Program.HttpSession.GenericReplyMessageAsync(command.MessageType, command.SenderId, command.GroupId,
            command.MessageId, new CqMessage("Bot 将在5秒后重启"));
        await Task.Delay(5000);
        Environment.Exit(0);
    }
}