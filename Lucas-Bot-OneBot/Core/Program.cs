// See https://aka.ms/new-console-template for more information
namespace Lucas_Bot_OneBot.Core;
using EleCho.GoCqHttpSdk;
using log4net;
using Lucas_Bot_OneBot.Helpers;
using Lucas_Bot_OneBot.Modules.Amusement;
using Lucas_Bot_OneBot.Modules.Phigros.Credentials;
using Lucas_Bot_OneBot.Modules.Phigros;
using System.Diagnostics;
using System.Runtime.InteropServices;

internal class Program
{

    // 配置全局Logger
    public static ILog Logger { get; } = LogManager.GetLogger("Lucas-Bot-OneBot");

    // 初始化反向HTTP连接
    public static CqRHttpSession RHttpSession { get; private set; } = new(new CqRHttpSessionOptions()
    {
        BaseUri = new Uri("http" + $"://{RHttpIpProvider.GetIpAddress()}:5900")
    });

    // 初始化正向HTTP连接
    public static CqHttpSession HttpSession { get; private set; } = new(new CqHttpSessionOptions()
    {
        BaseUri = new Uri("http://192.168.31.101:5700")
    });

    private static Stopwatch StopWatch { get; } = new Stopwatch();

    public static async Task Main(string[] args)
    {
        Logger.Info("Log4Net 已配置");
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Logger.Warn("处于调试环境中，正在应用调试配置");
            Logger.Warn("调试时请关闭 Windows 防火墙");
            HttpSession = new(new CqHttpSessionOptions()
            {
                BaseUri = new Uri("http://192.168.31.105:5700")
            });

            CommandBuilder.RegisterCommandPrefix('/');
        }
        else
        {
            CommandBuilder.RegisterCommandPrefix('!');
            CommandBuilder.RegisterCommandPrefix('！');
        }

        Logger.Info($"正向HTTP请求地址：htt" + $"p://{HttpSession.BaseClient.BaseAddress}");
        Logger.Info($"反向HTTP请求地址：htt" + $"p://{RHttpIpProvider.GetIpAddress()}:5900");
        StopWatch.Start();
        Logger.Info("计时器已启动");

        Utilities.InitMongoDbConnection("mongodb://192.168.31.104:27017");


        // 账号绑定功能
        CommandDispatcher.RegisterCommandHandler("bind", AccountManager.BindProcessor);
        CommandDispatcher.RegisterCommandHandler("unbind", AccountManager.UnbindProcessor);
        // Best 19 查分图生成
        CommandDispatcher.RegisterCommandHandler("b19", PhigrosModuleHelper.BestImageProcessor);
        // 文字形式输出Bests
        CommandDispatcher.RegisterCommandHandler("bests", PhigrosModuleHelper.BestsProcessor);
        // 推荐推分曲目
        CommandDispatcher.RegisterCommandHandler("suggest", PhigrosModuleHelper.SuggestProber);
        // 帮助
        CommandDispatcher.RegisterCommandHandler("help", BotStatusHelper.HelpProcessor);
        // 查询单曲成绩
        CommandDispatcher.RegisterCommandHandler("acc", PhigrosModuleHelper.AccuracyProcessor);
        // 查询母机状态
        CommandDispatcher.RegisterCommandHandler("status", BotStatusHelper.StatusProcessor);
        // 查询曲目信息
        CommandDispatcher.RegisterCommandHandler("song", AssetsHelpers.SongInfoProcessor);
        // 查询存档信息
        CommandDispatcher.RegisterCommandHandler("info", PhigrosModuleHelper.InfoProcessor);
        // 控制是否打开游戏内头像
        CommandDispatcher.RegisterCommandHandler("avatar", AccountManager.AvatarSettingProcessor);
        CommandDispatcher.RegisterCommandHandler("long", IfYouAreADragon.DragonPictureProcessor);

        // 配置程序停止时动作
        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            RHttpSession.Stop();
            Logger.Info("已断开和 Shamrock 的连接，机器人已下线");
            StopWatch.Stop();
            Environment.Exit(0);
        };

        // 注册消息处理中间件
        RHttpSession.UseGroupMessage(CommandDispatcher.GroupCommandDispatchMiddleware);
        RHttpSession.UsePrivateMessage(CommandDispatcher.PrivateCommandDispatchMiddleware);

        // 启动反向HTTP会话
        await RHttpSession.StartAsync();
        Logger.Info("已建立到 Shamrock 的正/反向HTTP连接，机器人已上线");

        // 阻塞主线程，启动消息处理
        await Task.Delay(-1);
    }

    public static TimeSpan GetRunTime()
    {
        return StopWatch.Elapsed.StripMilliseconds();
    }
}