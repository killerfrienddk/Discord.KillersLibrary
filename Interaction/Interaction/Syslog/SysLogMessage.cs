using System.Collections.Generic;
using System;
using Discord;

namespace Interaction.Services {
    public class SysLogMessage {
        public static readonly Dictionary<LogSeverity, ConsoleColor> SeverityColors = new Dictionary<LogSeverity, ConsoleColor> {
            { LogSeverity.Debug, ConsoleColor.DarkGray },
            { LogSeverity.Verbose, ConsoleColor.Blue },
            { LogSeverity.Info, ConsoleColor.White },
            { LogSeverity.Warning, ConsoleColor.DarkYellow },
            { LogSeverity.Error, ConsoleColor.Red },
            { LogSeverity.Critical, ConsoleColor.DarkRed },
        };
        public static LogSeverity ThresholdLevel { get; } = LogSeverity.Error;

        public ConsoleColor Color => SeverityColor(Level);
        public string Message { get; }
        public Exception Exception { get; }
        public string Source { get; }
        public LogSeverity Level { get; }
        public LogOutput Output { get; }
        public DateTimeOffset Timestamp { get; } = DateTimeOffset.Now;

        public SysLogMessage(string message = null, Exception exception = null, string source = null, LogSeverity? severity = null, LogOutput? logOutput = null) {
            Message = message;
            Exception = exception;
            Source = source;
            Level = severity ?? ((Exception == null) ? LogSeverity.Error : LogSeverity.Info);
            Output = logOutput ?? ((Level <= ThresholdLevel) ? LogOutput.ConsoleAndFile : LogOutput.Console);
        }

        public static ConsoleColor SeverityColor(LogSeverity s) => SeverityColors.GetValueOrDefault(s, ConsoleColor.Magenta);

        public override string ToString() {
            string data_data = (!string.IsNullOrEmpty(Message)) ? $"{Message}" : "";
            string exception_data = (Exception != null) ? $"{(!string.IsNullOrEmpty(Message) ? " :: Exception:" : "")} {Exception.Message} " : "";

            //string sourceStr = (!string.IsNullOrEmpty(Source)) ? $"[{Source}]" : "";

            return $"[{SysLog.Instance.DateStamp(Timestamp)}] <{Level}> [{Source}] {data_data}{exception_data}";
        }
    }
}