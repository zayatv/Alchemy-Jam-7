using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Core.Scripts.Systems.Logging
{
    public static class GameLogger
    {
        public static void Log(LogLevel level, string message, [CallerFilePath] string file = null)
        {
            var logEntry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Sender = Path.GetFileNameWithoutExtension(file),
                Level = level,
                Message = message
            };

            OutputToUnityConsole(logEntry);
        }
        
        private static void OutputToUnityConsole(LogEntry entry)
        {
            switch (entry.Level)
            {
                case LogLevel.Debug:
                    Debug.Log(entry.ToString());
                    break;
                case LogLevel.Info:
                    Debug.Log(entry.ToString());
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(entry.ToString());
                    break;
                case LogLevel.Error:
                    Debug.LogError(entry.ToString());
                    break;
                case LogLevel.Critical:
                    Debug.LogError($"CRITICAL: {entry.ToString()}");
                    break;
            }
        }
    }
}