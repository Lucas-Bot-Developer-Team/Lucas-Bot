using EleCho.GoCqHttpSdk.Post;

namespace Lucas_Bot_OneBot.Core;

public static class WebSocketHeartbeat
{
    private static int _heartbeatCount;
    
    public static void WebSocketHeartbeatMiddleware(CqHeartbeatPostContext context)
    {
        _heartbeatCount++;
        if (_heartbeatCount % 10 == 0)
        {
            Program.Logger.Info($"WebSocket 心跳存活，已和服务器进行 {_heartbeatCount} 次握手");
        }
    }
}