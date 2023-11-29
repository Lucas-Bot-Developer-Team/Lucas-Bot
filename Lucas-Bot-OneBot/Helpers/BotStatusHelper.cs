using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using Lucas_Bot_OneBot.Core;
using Lucas_Bot_OneBot.Entities;
using System.Runtime.InteropServices;

namespace Lucas_Bot_OneBot.Helpers;

internal static class BotStatusHelper
{

    public static TimeSpan ScheduledRebootTime { get; set; } = new TimeSpan();

    public static async void HelpProcessor(Command command)
    {
        await Program.HttpSession.SendMessageAsync(command.MessageType, command.SenderId, command.GroupId,
            new CqMessage(
                "Suzanne-Phigros 试运行 ver 0.0.4\n" +
                "帮助已迁移: https://www.yuque.com/lucas1522/sxcczd"));
    }

    public static async void StatusProcessor(Command command)
    {
        var cpuUsage = await Utilities.GetCpuUsageForProcess();
        await Program.HttpSession.SendMessageAsync(command.MessageType, command.SenderId, command.GroupId,
            new CqMessage("服务在线\n" +
            $"Runtime Identifier: {RuntimeInformation.RuntimeIdentifier}\n" +
            $"Operating System: {RuntimeInformation.OSDescription}\n" +
            $"Architecture: {RuntimeInformation.OSArchitecture}\n" +
            $".NET Runtime: {RuntimeInformation.FrameworkDescription}\n" +
            $"CPU Usage: {cpuUsage:F3} %\n" +
            $"Memory Allocated: {Utilities.GetUsedMemory():F2} MB\n" +
            $"GC Information: Heap size = {GC.GetTotalMemory(true) / 1048576.0:F2} MB; Collection Count = {GC.CollectionCount(0)}/{GC.CollectionCount(1)}/{GC.CollectionCount(2)}\n" +
            $"Thread Count: {ThreadPool.ThreadCount}\n" +
            $"Uptime: {Program.GetRunTime():c}\n" +
            $"Scheduled Reboot: {ScheduledRebootTime - Program.GetRunTime()}"));
    }
}