/*=========================================================================================
'  Copyright(C):    Johan Henningsson 
'  
'  Author :         Johan Henningsson
'==========================================================================================*/

using System;
using System.IO;
using System.Text;

namespace AztecQR
{
    /// <summary>
    /// Log levels for the application logger.
    /// </summary>
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// Thread-safe singleton logger for application diagnostics and error logging.
    /// </summary>
    public sealed class Logger
    {
        private static readonly Lazy<Logger> instance = new Lazy<Logger>(() => new Logger());
        private static readonly object lockObject = new object();
        private readonly string logFilePath;
        private readonly long maxLogFileSize = 5 * 1024 * 1024; // 5 MB
        private LogLevel minimumLogLevel = LogLevel.Info;

        private Logger()
        {
            string logDirectory = null;
            // Try AppData, then Documents, then Temp
            logFilePath = TryGetLogPath(
                () => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AztecQRGenerator", "Logs"),
                "AppData")
                ?? TryGetLogPath(
                    () => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AztecQRGenerator", "Logs"),
                    "Documents")
                ?? TryGetLogPath(
                    () => Path.Combine(Path.GetTempPath(), "AztecQRGenerator", "Logs"),
                    "Temp")
                ?? TryGetLogPath(
                    () => Path.Combine(Path.GetTempPath(), $"AztecQR_{DateTime.Now:yyyyMMdd}.log"),
                    "Temp (fallback)");
        }

        private string TryGetLogPath(Func<string> dirFunc, string label)
        {
            try
            {
                string dir = dirFunc();
                if (!Directory.Exists(Path.GetDirectoryName(dir)))
                    Directory.CreateDirectory(Path.GetDirectoryName(dir));
                string path = dir.EndsWith(".log") ? dir : Path.Combine(dir, $"AztecQR_{DateTime.Now:yyyyMMdd}.log");
                File.AppendAllText(path, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Logger initialized in {label}{Environment.NewLine}");
                return path;
            }
            catch { return null; }
        }

        /// <summary>
        /// Gets the singleton instance of the logger.
        /// </summary>
        public static Logger Instance => instance.Value;

        /// <summary>
        /// Sets the minimum log level for output.
        /// </summary>
        public void SetMinimumLogLevel(LogLevel level) => minimumLogLevel = level;

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        public void Debug(string message, Exception ex = null) => Log(LogLevel.Debug, message, ex);
        /// <summary>
        /// Logs an info message.
        /// </summary>
        public void Info(string message, Exception ex = null) => Log(LogLevel.Info, message, ex);
        /// <summary>
        /// Logs a warning message.
        /// </summary>
        public void Warning(string message, Exception ex = null) => Log(LogLevel.Warning, message, ex);
        /// <summary>
        /// Logs an error message.
        /// </summary>
        public void Error(string message, Exception ex = null) => Log(LogLevel.Error, message, ex);

        private void Log(LogLevel level, string message, Exception ex)
        {
            if (level < minimumLogLevel || string.IsNullOrEmpty(logFilePath))
                return;
            try
            {
                lock (lockObject)
                {
                    RotateLogIfNeeded();
                    var logEntry = new StringBuilder();
                    logEntry.Append($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level.ToString().ToUpper()}] {message}");
                    if (ex != null)
                    {
                        logEntry.AppendLine();
                        logEntry.Append($"Exception: {ex.GetType().Name} - {ex.Message}");
                        logEntry.AppendLine();
                        logEntry.Append($"StackTrace: {ex.StackTrace}");
                        if (ex.InnerException != null)
                        {
                            logEntry.AppendLine();
                            logEntry.Append($"Inner Exception: {ex.InnerException.GetType().Name} - {ex.InnerException.Message}");
                        }
                    }
                    File.AppendAllText(logFilePath, logEntry.ToString() + Environment.NewLine);
                }
            }
            catch { /* Fail silently to prevent logging from crashing the application */ }
        }

        private void RotateLogIfNeeded()
        {
            if (string.IsNullOrEmpty(logFilePath))
                return;
            try
            {
                if (File.Exists(logFilePath))
                {
                    FileInfo fileInfo = new FileInfo(logFilePath);
                    if (fileInfo.Length >= maxLogFileSize)
                    {
                        string backupPath = logFilePath.Replace(".log", $"_{DateTime.Now:HHmmss}.log");
                        File.Move(logFilePath, backupPath);
                    }
                }
            }
            catch { /* Fail silently */ }
        }

        /// <summary>
        /// Logs method entry with parameters.
        /// </summary>
        public void LogMethodEntry(string className, string methodName, params object[] parameters)
        {
            var paramString = parameters != null && parameters.Length > 0 
                ? $"Parameters: {string.Join(", ", parameters)}" 
                : "No parameters";
            Debug($"Entering {className}.{methodName} - {paramString}");
        }

        /// <summary>
        /// Logs method exit with success status.
        /// </summary>
        public void LogMethodExit(string className, string methodName, bool success = true)
        {
            Debug($"Exiting {className}.{methodName} - Success: {success}");
        }

        /// <summary>
        /// Gets the current log file path.
        /// </summary>
        public string GetLogFilePath() => logFilePath ?? "Logging disabled (no writable location found)";
    }
}
