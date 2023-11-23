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
                "Suzanne-Phigros 试运行 ver 0.0.4" +
                (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? " 调试模式\n" : "\n") +
                $"指令：\n{CommandBuilder.DefaultCommandSuffix}help 查看帮助\n" +
                $"{CommandBuilder.DefaultCommandSuffix}b19 生成查分图\n" +
                $"{CommandBuilder.DefaultCommandSuffix}bind <sessionToken> 绑定sessionToken\n" +
                $"{CommandBuilder.DefaultCommandSuffix}avatar <on|off> 控制查分图使用游戏内/QQ头像\n" +
                $"{CommandBuilder.DefaultCommandSuffix}unbind 解除绑定\n" +
                $"{CommandBuilder.DefaultCommandSuffix}bests <Bests数量> 文字形式输出Best19（或更多）\n" +
                $"{CommandBuilder.DefaultCommandSuffix}suggest 推荐推分曲目\n" +
                $"{CommandBuilder.DefaultCommandSuffix}acc <曲目ID/名称/别名> <难度（可选）> 查询单曲成绩信息\n" +
                $"{CommandBuilder.DefaultCommandSuffix}status 查询服务状态\n" +
                $"{CommandBuilder.DefaultCommandSuffix}song <曲目ID/名称/别名> 查询曲目信息\n" +
                $"{CommandBuilder.DefaultCommandSuffix}info 查看绑定的账号信息\n" +
                $"{CommandBuilder.DefaultCommandSuffix}long 龙图盲盒\n" +
                "更多功能：正在开发中\n绑定请尽量私聊完成（无需加好友）！"));
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