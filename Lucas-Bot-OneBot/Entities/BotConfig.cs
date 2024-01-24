using System.Xml.Serialization;

namespace Lucas_Bot_OneBot.Entities;

[XmlRoot("config")]
public class BotConfig
{
    public required string HttpSessionProvider { get; set; }

    public required string MongoDBAddress { get; set; }

    public required bool IsInDebugMode { get; set; }

    public required TimeSpan ScheduledRebootTime { get; set; }
    
    public required ConnectionType ConnectionType { get; set; }
    public required bool DeployedInDiscord { get; set; }

    public static void GenerateBotConfigFileSample()
    {
        var botConfig = new BotConfig()
        {
            HttpSessionProvider = "http://127.0.0.1:5700",
            MongoDBAddress = "mongodb://127.0.0.1",
            IsInDebugMode = true,
            ScheduledRebootTime = TimeSpan.FromMinutes(30),
            ConnectionType = ConnectionType.HTTP,
            DeployedInDiscord = false
        };
        var xmlSerializer = new XmlSerializer(botConfig.GetType());
        var writeStream = new FileStream("config-sample.xml", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
        xmlSerializer.Serialize(writeStream, botConfig);
    }
}
