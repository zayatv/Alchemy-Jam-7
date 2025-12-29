using System;

namespace Core.Scripts.Systems.Logging
{
    public struct LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Sender { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            var result = $"[{Timestamp:HH:mm:ss}] [{Sender}] [{Level}] {Message}";
            
            return result;
        }
    }
}