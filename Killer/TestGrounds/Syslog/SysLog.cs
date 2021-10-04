using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System;
using Discord;

namespace Interaction.TestingGrounds.Services {
    public class SysLog {
        internal SysLog() {
            _lock = new SemaphoreSlim(1, 1);
            _consoleLock = new SemaphoreSlim(1, 1);
            _activeSysLogs = new Dictionary<string, SysLog>();
            FolderNameMatch = new Regex(@"\W");

            ConsolePrintLineStatic($"[SysLog.SysLog()] Instance!");
            ConsolePrintLineStatic($"[SysLog.SysLog()] End!");
        }
        static SysLog() { }

        private static readonly SysLog _instance = new();

        public static SysLog Instance {
            get { return _instance; }
        }

        public ImmutableDictionary<string, SysLog> ActiveSysLogs => _activeSysLogs.ToImmutableDictionary();

        public const ConsoleColor DEFAULT_COLOR = ConsoleColor.Cyan;
        public const ConsoleColor DEFAULT_STATIC_COLOR = ConsoleColor.Green;

        private readonly string LogPath = "SysLogs";
        private readonly string FileType = "txt";

        private Dictionary<string, SysLog> _activeSysLogs;

        private Regex FolderNameMatch;
        private readonly SemaphoreSlim _consoleLock;
        private readonly SemaphoreSlim _lock;

        #region Static Methods
        public ConsoleColor SeverityColor(LogSeverity s)
            => SysLogMessage.SeverityColor(s);

        //Prints a message into the console with a timestamp.
        public void ConsolePrintLineStatic(string message) {
            _writeLine($"[{DateStamp(DateTimeOffset.Now)}] {message}", DEFAULT_STATIC_COLOR);
        }
        #endregion

        #region Properties
        public string Name { get; protected set; }
        #endregion

        #region Constructors
        private SysLog(string name) {
            Name = name;
        }

        //Asynchronously creates a SysLog object.
        public async Task<SysLog> CreateAsync(string name) {
            try {
                await _lock.WaitAsync().ConfigureAwait(false);

                return Create(name);
            } catch (Exception e) {
                ConsolePrintLineStatic($"[SysLog.CreateAsync({name})] {e}");
            } finally {
                _lock.Release();
            }
            return null;
        }

        public async Task<SysLog> CreateAsync_(string name) {
            Console.WriteLine($"[SysLog.Create({name})] Init!");

            try {
                await _lock.WaitAsync().ConfigureAwait(false);

                _activeSysLogs ??= new Dictionary<string, SysLog>();
                FolderNameMatch ??= new Regex(@"\W");


                name = name?.ToLower();

                Console.WriteLine($"[SysLog.Create({name})] Started!");
                if (_activeSysLogs.TryGetValue(name, out var log)) {
                    Console.WriteLine($"[SysLog.Create({name})] Already found!");
                    return log;
                } else if (string.IsNullOrEmpty(name)) {
                    Console.WriteLine($"[SysLog.Create({name})] Name is empty!");
                } else if (FolderNameMatch.IsMatch(name)) {
                    Console.WriteLine($"[SysLog.Create({name})] Name is not a valid name!");
                } else {
                    log = new SysLog(name);
                    _activeSysLogs.Add(name, log);
                    Console.WriteLine($"[SysLog.Create({name})] Created!");
                    return log;
                }
                Console.WriteLine($"[SysLog.Create({name})] Ended!");
                return null;
            } catch (Exception e) {
                Console.WriteLine($"[SysLog.Create({name})] {e}");
            } finally {
                _lock.Release();
            }

            return null;
        }

        //Returns the SysLog object of the given name.
        //Creates a new SysLog object if none exist already.
        public SysLog Create(string name) {

            //Make the name lower-case.
            name = name?.ToLower();

            //Make sure the name isn't empty.
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"The name cannot be empty!", nameof(name));

            //Check if name contains any invalid characters.
            if (FolderNameMatch.IsMatch(name))
                throw new ArgumentException($"Name '{name}' is not a valid name!", nameof(name));

            //Lock to prevent 2 instances from running at the same time.
            lock (_lock) {
                try {

                    // See if the name isn already in use.
                    if (_activeSysLogs.TryGetValue(name, out var log)) {
                        ConsolePrintLineStatic($"[SysLog.Create({name})] Already found!");
                        return log;
                    }

                    //If the name isn't already in use.
                    else {
                        log = new SysLog(name);
                        _activeSysLogs.Add(name, log);
                        ConsolePrintLineStatic($"[SysLog.Create({name})] Created!");
                        return log;
                    }
                }

                //If an exception is thrown, log it and return nothing.
                catch (Exception e) {
                    ConsolePrintLineStatic($"[SysLog.Create({name})] {e}");
                    return null;
                }
            }
        }
        #endregion

        #region Methods
        //Asynchronously writes the message int the console in the desired color.
        protected async Task _writeLineAsync(string data, ConsoleColor color) {
            try {
                if (_consoleLock != null) await _consoleLock.WaitAsync().ConfigureAwait(false);
                _writeLine(data, color);
            } finally {
                _consoleLock.Release();
            }
        }
        protected async Task _writeLineAsync_(string data, ConsoleColor color) {
            try {
                await _consoleLock.WaitAsync().ConfigureAwait(false);
                Console.ForegroundColor = color;
                await Console.Out.WriteLineAsync(data);
                Console.ForegroundColor = DEFAULT_COLOR;
            } finally {
                _consoleLock.Release();
            }
        }

        //Write the message into the console in the desired color.
        //Synchronizes writes using lock mechanism.
        protected void _writeLine(string data, ConsoleColor color) {
            lock (_consoleLock) {
                try {
                    Console.ForegroundColor = color;
                    Console.Out.WriteLine(data);
                } finally {
                    Console.ForegroundColor = DEFAULT_COLOR;
                }
            }
        }

        public string DateStamp(DateTimeOffset dto) => dto.ToString("yyyy/MM/dd HH:mm:ss");
        public string DateStamp() => DateStamp(DateTimeOffset.Now);
        public string DateStamp(int days) => DateTime.Now.AddDays(days).ToString("yyyy/MM/dd HH:mm:ss");

        public Task WriteLine(object data, LogSeverity severity = LogSeverity.Info, bool printDate = true) {
            var color = SeverityColor(severity);
            var output = (printDate) ? $"[{DateStamp()}] {data}" : $"{data}";
            return _writeLineAsync(output, color);
        }

        public async Task WriteLineAsync(object data, LogSeverity severity = LogSeverity.Info, bool printDate = true) {
            await Task.CompletedTask;
            await WriteLine(data, severity, printDate);
        }
        public Task WriteLine(Exception e, bool printDate = true)
            => Instance.WriteLine(e, LogSeverity.Error, printDate);
        #endregion

        public async Task LogAsync(SysLogMessage msg) {
            //In-case of mysterious empty prints.
            //if(msg.Exception == null && string.IsNullOrEmpty(msg.Message)) Console.WriteLine($"[SysLog.LogAsync()] No info was given :: StackTrace: {new System.Diagnostics.StackTrace()}");

            //3) Format output.
            if ((msg.Output & LogOutput.Console) > 0) {
                await _writeLineAsync(msg.ToString(), msg.Color);
                //Console.ForegroundColor = msg.Color;
                //await Console.Out.WriteLineAsync(msg.ToString());
                //Console.ForegroundColor = DEFAULT_COLOR;
            }
        }

        public async Task LogAsync(
            Type type, string message = null, Exception exception = null, string parameterString = null,
            LogOutput? logOutput = null, LogSeverity level = LogSeverity.Debug, [CallerMemberName] string name = null
        ) {
            await LogAsync(new SysLogMessage(message, exception, $"{type.Name}.{name}({parameterString})", level, logOutput));
        }

        public Task Critical(Type type, string message = null, Exception exception = null, string parameterString = null, LogOutput? logOutput = null, [CallerMemberName] string name = null)
            => LogAsync(type, message, exception, parameterString, logOutput, LogSeverity.Critical, name);
        public Task Error(Type type, string message = null, Exception exception = null, string parameterString = null, LogOutput? logOutput = null, [CallerMemberName] string name = null)
            => LogAsync(type, message, exception, parameterString, logOutput, LogSeverity.Error, name);
        public Task Warning(Type type, string message = null, Exception exception = null, string parameterString = null, LogOutput? logOutput = null, [CallerMemberName] string name = null)
            => LogAsync(type, message, exception, parameterString, logOutput, LogSeverity.Warning, name);
        public Task Info(Type type, string message = null, Exception exception = null, string parameterString = null, LogOutput? logOutput = null, [CallerMemberName] string name = null)
            => LogAsync(type, message, exception, parameterString, logOutput, LogSeverity.Info, name);
        public Task Verbose(Type type, string message = null, Exception exception = null, string parameterString = null, LogOutput? logOutput = null, [CallerMemberName] string name = null)
            => LogAsync(type, message, exception, parameterString, logOutput, LogSeverity.Verbose, name);
        public Task Debug(Type type, string message = null, Exception exception = null, string parameterString = null, LogOutput? logOutput = null, [CallerMemberName] string name = null)
            => LogAsync(type, message, exception, parameterString, logOutput, LogSeverity.Debug, name);
        public async Task LogAsync(string source = null, LogSeverity level = LogSeverity.Debug, string message = null, Exception exception = null, LogOutput? logOutput = null)
            => await LogAsync(new SysLogMessage(message, exception, source, level, logOutput));
        public Task Critical(string source = null, string message = null, Exception exception = null, LogOutput? logOutput = null)
            => LogAsync(source: source, message: message, exception: exception, level: LogSeverity.Critical, logOutput: logOutput);
        public Task Error(string source = null, string message = null, Exception exception = null, LogOutput? logOutput = null)
            => LogAsync(source: source, message: message, exception: exception, level: LogSeverity.Error, logOutput: logOutput);
        public Task Warning(string source = null, string message = null, Exception exception = null, LogOutput? logOutput = null)
            => LogAsync(source: source, message: message, exception: exception, level: LogSeverity.Warning, logOutput: logOutput);
        public Task Info(string source = null, string message = null, Exception exception = null, LogOutput? logOutput = null)
            => LogAsync(source: source, message: message, exception: exception, level: LogSeverity.Info, logOutput: logOutput);
        public Task Verbose(string source = null, string message = null, Exception exception = null, LogOutput? logOutput = null)
            => LogAsync(source: source, message: message, exception: exception, level: LogSeverity.Verbose, logOutput: logOutput);
        public Task Debug(string source = null, string message = null, Exception exception = null, LogOutput? logOutput = null)
            => LogAsync(source: source, message: message, exception: exception, level: LogSeverity.Debug, logOutput: logOutput);

        #region FileWriting
        private Task SavingTask = Task.CompletedTask;
        private ConcurrentQueue<FileWriteTaskData> WriteQueue = new ConcurrentQueue<FileWriteTaskData>();

        private async Task WriteToLogFile(string data, string path) {
            //var f = new FileInfo(path);
            //Console.WriteLine($"TEST 6 :: path: {f.FullName}");
            //Console.WriteLine("TEST 1");


            var index = path.LastIndexOf('\\');
            var folder = path.Substring(0, index);

            Directory.CreateDirectory(folder);
            await File.AppendAllTextAsync(path, $"{data}\n");
            //using (StreamWriter writer = File.AppendText(path ?? LogFile)) { await writer.WriteLineAsync(data); }
        }
        #endregion
    }

    class FileWriteTaskData {
        public string Data;
        public string Path;
    }
}