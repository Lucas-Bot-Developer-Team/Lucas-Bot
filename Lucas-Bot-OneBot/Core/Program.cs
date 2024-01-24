// See https://aka.ms/new-console-template for more information

using Lucas_Bot_OneBot.Modules.Amusement.YiYan;

namespace Lucas_Bot_OneBot.Core;
using EleCho.GoCqHttpSdk;
using log4net;
using Helpers;
using Modules.Amusement;
using Modules.Phigros.Credentials;
using System.Diagnostics;
using Entities;
using Modules.Phigros.Services;
using System.Xml.Serialization;

internal static class Program
{

    // 配置全局Logger
    public static ILog Logger { get; } = LogManager.GetLogger("Lucas-Bot-OneBot");

    // 初始化反向HTTP连接
    private static ICqPostSession RHttpSession { get; set; } = new CqRHttpSession(new CqRHttpSessionOptions()
    {
        BaseUri = new Uri("http" + $"://{RHttpIpProvider.GetIpAddress()}:5900")
    });

    // 初始化正向HTTP连接
    public static ICqActionSession HttpSession { get; private set; } = new CqHttpSession(new CqHttpSessionOptions());

    private static Stopwatch StopWatch { get; } = new Stopwatch();

    private static BotConfig? _botConfig;

    private static void InitializeConfig()
    {
        // 显示 Logo
        var logo = Utilities.GetLogo();
        foreach (var row in logo)
        {
            Logger.Info(row);
        }
        
        // 从 config.xml 读取配置信息
        if (File.Exists("config.xml"))
        {
            try
            {
                var config = new XmlSerializer(typeof(BotConfig))
                    .Deserialize(File.OpenRead("config.xml")) as BotConfig;
                _botConfig = config;
                if (_botConfig is null)
                {
                    throw new ArgumentNullException();
                }
                BotStatusHelper.ScheduledRebootTime = _botConfig.ScheduledRebootTime;
                HttpSession = new CqHttpSession(new CqHttpSessionOptions()
                {
                    BaseUri = new Uri(_botConfig.HttpSessionProvider)
                });
            }
            catch (Exception ex)
            {
                Logger.Error("读取配置文件时出现异常", ex);
                Environment.Exit(1);
            }
        }
        else
        {
            Logger.Error("配置文件不存在");
            Logger.Warn("请执行 dotnet Lucas-Bot-OneBot.dll --generate-config-file 生成配置文件");
            Logger.Warn("并修改后命名为 config.xml, 放置在可执行文件根目录下");
            Environment.Exit(1);
        }
    }

    private static async void InitializeConnection(ConnectionType type)
    {
        switch (type)
        {
            case ConnectionType.HTTP:
                Logger.Info($"正向HTTP请求地址：{_botConfig!.HttpSessionProvider}");
                Logger.Info($"反向HTTP请求地址：http://{RHttpIpProvider.GetIpAddress()}:5900");
                break;
            case ConnectionType.WEB_SOCKET:
                var wsSession = new CqWsSession(new CqWsSessionOptions()
                {
                    BaseUri = new Uri(_botConfig!.HttpSessionProvider),
                
                });
                HttpSession = wsSession;
                RHttpSession = wsSession;
            
                Logger.Info($"WebSocket 监听地址：{_botConfig.HttpSessionProvider}");
                break;
            case ConnectionType.REVERSE_WEB_SOCKET:
                var rWsSession = new CqRWsSession(new CqRWsSessionOptions()
                {
                    BaseUri = new Uri($"http://{RHttpIpProvider.GetIpAddress()}:5900"),
                    
                });
                HttpSession = rWsSession;
                RHttpSession = rWsSession;
            
                Logger.Info($"反向 WebSocket 监听地址：{rWsSession.BaseUri}");
                break;
        }
            
        // 注册消息处理中间件
        RHttpSession.UseGroupMessage(CommandDispatcher.GroupCommandDispatchMiddleware);
        RHttpSession.UsePrivateMessage(CommandDispatcher.PrivateCommandDispatchMiddleware);
        // RHttpSession.UseGroupEssenceChanged();

        // 启动反向HTTP会话
        await StartSessionAsync(RHttpSession);
        
        try
        {
            var info = await HttpSession.GetLoginInformationAsync();
            Logger.Info("当前登陆信息:");
            Logger.Info($"Gensokyo 状态: {info!.Status.ToString()}");
            Logger.Info($"Discord AppID: {info.UserId}");
            Logger.Info($"昵称: {info.Nickname}");
            Logger.Info($"服务端返回的额外信息: {info.EchoData}");
        }
        catch (Exception ex)
        {
            Logger.Error("尝试连接 Gensokyo 失败，进程将退出", ex);
            Environment.Exit(1);
        }

        Logger.Info("已建立到 Gensokyo 的连接，机器人已上线");
        return;

        async Task StartSessionAsync(ICqPostSession session)
        {
            switch (_botConfig!.ConnectionType)
            {
                case ConnectionType.HTTP:
                    await ((CqRHttpSession)session).StartAsync();
                    break;
                case ConnectionType.WEB_SOCKET:
                    var wsSession = session as CqWsSession;
                    wsSession!.UseHeartbeat(WebSocketHeartbeat.WebSocketHeartbeatMiddleware);
                    await wsSession!.StartAsync();
                    break;
                case ConnectionType.REVERSE_WEB_SOCKET:
                    var rWsSession = session as CqRWsSession;
                    rWsSession!.UseHeartbeat(WebSocketHeartbeat.WebSocketHeartbeatMiddleware);
                    await rWsSession!.StartAsync();
                    Logger.Info("反向 WebSocket 连接已启动，将会等待 10 秒客户端启动时间");
                    await Task.Delay(10000);
                    break;
            }
                
        }
    }
    
    public static async Task Main(string[] args)
    {
        Logger.Info("Log4Net 已配置");

        if (args.Contains("-g") || args.Contains("--generate-config-file"))
        {
            BotConfig.GenerateBotConfigFileSample();
            Logger.Info("默认配置文件已生成，请根据实际使用修改");
            return;
        }

        InitializeConfig();

        if (_botConfig!.IsInDebugMode)
        {
            Logger.Warn("处于调试环境中，正在应用调试配置");
            Logger.Warn("调试时请关闭 Windows 防火墙");

            CommandBuilder.RegisterCommandPrefix('/');
        }
        else
        {
            CommandBuilder.RegisterCommandPrefix('!');
            CommandBuilder.RegisterCommandPrefix('！');
        }
        StopWatch.Start();
        Logger.Info("计时器已启动");

        Utilities.InitMongoDbConnection(_botConfig.MongoDBAddress);


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
        CommandDispatcher.RegisterCommandHandler("batch", PhigrosModuleHelper.BatchProcessor);
        // 控制是否打开游戏内头像
        CommandDispatcher.RegisterCommandHandler("avatar", AccountManager.AvatarSettingProcessor);
        CommandDispatcher.RegisterCommandHandler("long", IfYouAreADragon.DragonPictureProcessor);
        // 关于
        CommandDispatcher.RegisterCommandHandler("about", BotStatusHelper.AboutProcessor);

        // Test
        if (!_botConfig.DeployedInDiscord)
            CommandDispatcher.RegisterCommandHandler("yiyan", YiYanProvider.YiYanProcessorTest, CommandHandlerType.GROUP_ONLY);
        
        // 配置程序停止时动作
        Console.CancelKeyPress += (_, _) =>
        {
            StopSession(RHttpSession);
            Logger.Info("已断开和 Gensokyo 的连接，机器人已下线");
            StopWatch.Stop();
            Environment.Exit(0);
        };

        InitializeConnection(_botConfig.ConnectionType);

        // 阻塞主线程，启动消息处理
        if (_botConfig.IsInDebugMode)
        {
            await Task.Delay(-1);
        }
        else
        {
            await Task.Delay(_botConfig.ScheduledRebootTime);
        }
        Logger.Warn("超过 config.xml 中的自动重启时间，应用将退出，请根据操作系统给配置自动重启（如使用systemd、脚本等）");
        return;
        
        void StopSession(ICqPostSession session)
        {
            switch (_botConfig!.ConnectionType)
            {
                case ConnectionType.HTTP:
                    ((CqRHttpSession)session).Stop();
                    break;
                case ConnectionType.WEB_SOCKET:
                    ((CqWsSession)session).Stop();
                    break;
                case ConnectionType.REVERSE_WEB_SOCKET:
                    ((CqRWsSession)session).Stop();
                    break;
            }
        }
    }

    public static TimeSpan GetRunTime()
    {
        return StopWatch.Elapsed.StripMilliseconds();
    }
}