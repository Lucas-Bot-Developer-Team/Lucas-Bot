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

using System.Xml.Serialization;

namespace Lucas_Bot_OneBot.Entities;

[XmlRoot("config")]
public class BotConfig
{
    public required string HttpSessionProvider { get; set; }

    public required string MongoDbAddress { get; set; }

    public required bool IsInDebugMode { get; set; }
    public required bool EnableScheduledReboot { get; set; }

    public required TimeSpan ScheduledRebootTime { get; set; }
    
    public required ConnectionType ConnectionType { get; set; }
    public required PlatformType DeployedPlatformType { get; set; }
    public required string BotManagerId { get; set; }

    public static void GenerateBotConfigFileSample()
    {
        var botConfig = new BotConfig
        {
            HttpSessionProvider = "http://127.0.0.1:5700",
            MongoDbAddress = "mongodb://127.0.0.1",
            IsInDebugMode = true,
            EnableScheduledReboot = true,
            ScheduledRebootTime = TimeSpan.FromMinutes(30),
            ConnectionType = ConnectionType.HTTP,
            DeployedPlatformType = PlatformType.OPEN_SHAMROCK,
            BotManagerId = "123456789"
        };
        var xmlSerializer = new XmlSerializer(botConfig.GetType());
        var writeStream = new FileStream("config-sample.xml", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
        xmlSerializer.Serialize(writeStream, botConfig);
    }
}
