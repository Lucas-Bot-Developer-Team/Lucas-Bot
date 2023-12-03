using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using Lucas_Bot_OneBot.Core;
using Lucas_Bot_OneBot.Entities;
using System.Runtime.InteropServices;

namespace Lucas_Bot_OneBot.Helpers;

internal static class BotStatusHelper
{

    private static List<string> _tips = new List<string>()
    {
        "宇宙中密度最大的物质不是黑洞，是 node_modules 。", // <- H开头查分Bot
        "Rust 只是把该让程序员做的事情推给了编译器，最后的结果是让程序员小脑萎缩 。", // <- M开头查分Bot
        "当 Rust 成为“巨坑”：拖慢开发速度、员工被折磨数月信心全无，无奈还得硬着头皮继续", // <- M开头查分Bot
        "\"b\" + \"a\" + +\"a\" + \"a\"; // -> baNaNa", // <- H开头查分Bot
        "听说国内一堆软件开坑第一件事情就是加关于，刚想起有这回事，倒没什么动力加了，就这样了"
    };

    public static TimeSpan ScheduledRebootTime { get; set; } = new TimeSpan();

    public static async void HelpProcessor(Command command)
    {
        await Program.HttpSession.SendMessageAsync(command.MessageType, command.SenderId, command.GroupId,
            new CqMessage(
                "Suzanne-Phigros 试运行 ver 0.0.4\n" +
                "帮助已迁移: https://www.yuque.com/lucas1522/sxcczd"));
    }

    public static async void AboutProcessor(Command command)
    {
        lock (_tips)
        {
            if (_tips.Count == 5)
            {
                var gameTips = File.ReadAllLines("assets/phigros-ingame-statistics/tips.txt");
                _tips.AddRange(gameTips);
            }
        }
        var rd = new Random((int)DateTime.Now.Ticks);
        int index = rd.Next(_tips.Count);
        await Program.HttpSession.SendMessageAsync(command.MessageType, command.SenderId, command.GroupId,
            new CqMessage(
                "Suzanne-Phigros 试运行 ver 0.0.4\n" +
                $"Powered by {RuntimeInformation.FrameworkDescription}\n" +
                $"Tip: {_tips[index]}"));
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