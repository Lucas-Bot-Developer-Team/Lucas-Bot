// See https://aka.ms/new-console-template for more information
namespace Lucas_Bot_OneBot.Core;
using EleCho.GoCqHttpSdk;
using log4net;
using Lucas_Bot_OneBot.Helpers;
using Lucas_Bot_OneBot.Modules.Amusement;
using Lucas_Bot_OneBot.Modules.Phigros.Credentials;
using System.Diagnostics;
using Lucas_Bot_OneBot.Entities;
using Lucas_Bot_OneBot.Modules.Phigros.Services;
using System.Xml.Serialization;

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
    public static CqHttpSession HttpSession { get; private set; } = new(new CqHttpSessionOptions());

    private static Stopwatch StopWatch { get; } = new Stopwatch();

    public static async Task Main(string[] args)
    {
        Logger.Info("Log4Net 已配置");
        var isInDebugMode = false;
        var mongoDBAddress = "";
        var scheduledRebootTime = new TimeSpan();

        if (args.Contains("-g") || args.Contains("--generate-config-file"))
        {
            BotConfig.GenerateBotConfigFileSample();
            Logger.Info("默认配置文件已生成，请根据实际使用修改");
            return;
        }

        // 从config.xml 读取配置信息
        if (File.Exists("config.xml"))
        {
            try
            {
                var config = new XmlSerializer(typeof(BotConfig))
                                .Deserialize(File.OpenRead("config.xml")) as BotConfig;
                isInDebugMode = config!.IsInDebugMode;
                scheduledRebootTime = config!.ScheduledRebootTime;
                mongoDBAddress = config!.MongoDBAddress;
                BotStatusHelper.ScheduledRebootTime = scheduledRebootTime;
                HttpSession = new(new CqHttpSessionOptions()
                {
                    BaseUri = new Uri(config!.HttpSessionProvider)
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

        if (isInDebugMode)
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

        Logger.Info($"正向HTTP请求地址：{HttpSession.BaseClient.BaseAddress}");
        Logger.Info($"反向HTTP请求地址：htt" + $"p://{RHttpIpProvider.GetIpAddress()}:5900");
        StopWatch.Start();
        Logger.Info("计时器已启动");

        Utilities.InitMongoDbConnection(mongoDBAddress);


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
        await Task.Delay(scheduledRebootTime);
        Logger.Warn("超过config.xml中的自动重启时间，应用将退出，请根据操作系统给配置自动重启（如使用systemd、脚本等）");
        Environment.Exit(0);
    }

    public static TimeSpan GetRunTime()
    {
        return StopWatch.Elapsed.StripMilliseconds();
    }
}