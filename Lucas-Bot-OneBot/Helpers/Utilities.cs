using System.Diagnostics;
using log4net;
using Lucas_Bot_OneBot.Core;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Lucas_Bot_OneBot.Helpers;

internal static class Utilities
{
    private static bool _databaseConnected = false;
    private static MongoClient? _client;
    private static readonly ILog Logger = Program.Logger;

    /// <summary>
    /// 初始化MongoDB数据库连接
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    public static void InitMongoDbConnection(string connectionString)
    {
        Logger.Info("正在建立MongoDB数据库连接...");
        Logger.Info("数据库地址: " + connectionString);
        if (_databaseConnected) return;
        _databaseConnected = true;

        _client = new MongoClient(connectionString);
        Logger.Info("数据库连接建立成功");
    }

    /// <summary>
    /// 封装的MongoDB查询操作
    /// </summary>
    /// <param name="databaseName">数据库名</param>
    /// <param name="collectionName">集合名</param>
    /// <returns>查询结果</returns>
    /// <exception cref="Exception"></exception>
    public static IMongoCollection<BsonDocument> GetCollection(string databaseName, string collectionName)
    {
        if (!_databaseConnected)
            throw new Exception("数据库未连接");
        Logger.Info($"数据库查询操作: GetCollection(databaseName = {databaseName}, collectionName = {collectionName})");
        var database = _client!.GetDatabase(databaseName);
        var collection = database.GetCollection<BsonDocument>(collectionName);
        return collection;
    }

    /// <summary>
    /// 获得当前的unix时间戳
    /// </summary>
    /// <returns>当前的unix时间戳</returns>
    public static long GetUnixTime()
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Logger.Info($"GetCurrentUnixTime");
        return Convert.ToInt64((DateTime.Now.ToUniversalTime() - epoch).TotalMilliseconds);
    }


    public static DateTime TimeStampToDateTime(long timeStamp)
    {
        var dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
        var lTime = timeStamp;
        var toNow = new TimeSpan(lTime);
        return dtStart.Add(toNow);
    }

    public static long ToTimeStamp(this DateTime dt)
    {
        var dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
        var toNow = dt.Subtract(dtStart);
        var timeStamp = toNow.Ticks;
        return timeStamp;
    }

    public static TimeSpan StripMilliseconds(this TimeSpan time)
    {
        return new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds);
    }

    public static string GetAvatarUri(long qq)
    {
        return $"https://q1.qlogo.cn/g?b=qq&nk={qq}&s=5";
    }

    public static async Task<double> GetCpuUsageForProcess()
    {
        var startTime = DateTime.UtcNow;
        var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

        await Task.Delay(500);

        var endTime = DateTime.UtcNow;
        var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

        var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
        var totalMsPassed = (endTime - startTime).TotalMilliseconds;

        var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

        return cpuUsageTotal * 100;
    }

    public static double GetUsedMemory()
    {
        return Process.GetCurrentProcess().WorkingSet64 * 1.0 / (1024 * 1024);
    }

    /// <summary>
    /// 获得字符串的Levenshtein Distance
    /// </summary>
    /// <param name="lhs">字符串1</param>
    /// <param name="rhs">字符串2</param>
    /// <returns>Levenshtein Distance</returns>
    public static int GetLevenshteinDistance(string lhs, string rhs)
    {
        int m = lhs.Length;
        int n = rhs.Length;
        var L = new int[m + 1, n + 1];
        for (var i = 0; i <= m; i++)
        {
            for (var j = 0; j <= n; j++)
            {
                if (i == 0 || j == 0)
                {
                    L[i, j] = 0;
                }
                else if (lhs[i - 1] == rhs[j - 1])
                {
                    L[i, j] = L[i - 1, j - 1] + 1;
                }
                else
                {
                    L[i, j] = Math.Max(L[i - 1, j], L[i, j - 1]);
                }
            }
        }
        int lcs = L[m, n];
        return m - lcs + n - lcs;
    }
}
