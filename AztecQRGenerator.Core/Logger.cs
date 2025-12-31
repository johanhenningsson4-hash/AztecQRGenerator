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
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    public sealed class Logger
    {
        private static readonly Lazy<Logger> instance = new Lazy<Logger>(() => new Logger());
        private static readonly object lockObject = new object();
        private readonly string logFilePath;
        private readonly long maxLogFileSize = 5 * 1024 * 1024; // 5 MB
        private LogLevel minimumLogLevel = LogLevel.Info;

        private Logger()
        {
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            logFilePath = Path.Combine(logDirectory, $"AztecQR_{DateTime.Now:yyyyMMdd}.log");
        }

        public static Logger Instance => instance.Value;

        public void SetMinimumLogLevel(LogLevel level)
        {
            minimumLogLevel = level;
        }

        public void Debug(string message, Exception ex = null)
        {
            Log(LogLevel.Debug, message, ex);
        }

        public void Info(string message, Exception ex = null)
        {
            Log(LogLevel.Info, message, ex);
        }

        public void Warning(string message, Exception ex = null)
        {
            Log(LogLevel.Warning, message, ex);
        }

        public void Error(string message, Exception ex = null)
        {
            Log(LogLevel.Error, message, ex);
        }

        private void Log(LogLevel level, string message, Exception ex)
        {
            if (level < minimumLogLevel)
            {
                return;
            }

            try
            {
                lock (lockObject)
                {
                    RotateLogIfNeeded();

                    var logEntry = new StringBuilder();
                    logEntry.Append($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ");
                    logEntry.Append($"[{level.ToString().ToUpper()}] ");
                    logEntry.Append(message);

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
            catch
            {
                // Fail silently to prevent logging from crashing the application
            }
        }

        private void RotateLogIfNeeded()
        {
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
            catch
            {
                // Fail silently
            }
        }

        public void LogMethodEntry(string className, string methodName, params object[] parameters)
        {
            var paramString = parameters != null && parameters.Length > 0 
                ? $"Parameters: {string.Join(", ", parameters)}" 
                : "No parameters";
            Debug($"Entering {className}.{methodName} - {paramString}");
        }

        public void LogMethodExit(string className, string methodName, bool success = true)
        {
            Debug($"Exiting {className}.{methodName} - Success: {success}");
        }
    }
}
