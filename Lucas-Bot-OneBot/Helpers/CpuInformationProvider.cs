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

using Lucas_Bot_OneBot.Core;

namespace Lucas_Bot_OneBot.Helpers;
using System.Runtime.InteropServices;
using System.Diagnostics;

public static class CpuInformationProvider
{
    private static string _GetCpuNameWindowsImpl()
    {
        var cmd = new Process();
        cmd.StartInfo.FileName = "cmd.exe";
        cmd.StartInfo.RedirectStandardInput = true;
        cmd.StartInfo.RedirectStandardOutput = true;
        cmd.StartInfo.CreateNoWindow = true;
        cmd.StartInfo.UseShellExecute = false;
        cmd.Start();
        cmd.StandardInput.WriteLine("wmic cpu get name"); // Windows Implementation of CPU Name
        cmd.StandardInput.Flush();
        cmd.StandardInput.Close();
        cmd.WaitForExit();
        var rawLines = cmd.StandardOutput.ReadToEnd().Split('\n');
        for (var i = 0; i < rawLines.Length; ++i)
        {
            if (rawLines[i].Trim().Equals("Name"))
            {
                return rawLines[i + 1].Trim();
            }
        }

        return "Unknown CPU";
    }

    private static string _GetCpuNameLinuxImpl()
    {
        try
        {
            var cmd = "cat /proc/cpuinfo | grep name | cut -f2 -d:";
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
            var cpuInfo = process.StandardOutput.ReadToEnd();
            return cpuInfo.Trim();
        }
        catch (Exception e)
        {
            Program.Logger.Error($"_GetCpuInfoLinuxImpl error: {e.Message}", e);
            return "Unknown CPU";
        }
    }

    public static string GetCpuInfo()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return _GetCpuNameWindowsImpl();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return _GetCpuNameLinuxImpl();
        return "Unsupported Platform";
    }

    public static void TestCpuNameImpl()
    {
        Console.WriteLine(GetCpuInfo());
    }
}