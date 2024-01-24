﻿using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace EleCho.GoCqHttpSdk.Utils
{
    internal static class GlobalConfig
    {
        public static TimeSpan WaitTimeout = TimeSpan.FromSeconds(5);
        public static int WebSocketBufferSize = 1024;
        public static Encoding TextEncoding = Encoding.UTF8;
    }
}