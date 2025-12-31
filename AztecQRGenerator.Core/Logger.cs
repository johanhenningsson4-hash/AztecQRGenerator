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
            // Try multiple safe locations in order of preference
            string logDirectory = null;
            
            // Option 1: User's AppData (most reliable)
            try
            {
                logDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "AztecQRGenerator",
                    "Logs"
                );
                
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
                
                logFilePath = Path.Combine(logDirectory, $"AztecQR_{DateTime.Now:yyyyMMdd}.log");
                
                // Test write access
                File.AppendAllText(logFilePath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Logger initialized in AppData{Environment.NewLine}");
                return;
            }
            catch
            {
                // AppData failed, try next option
            }

            // Option 2: User's Documents folder
            try
            {
                logDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "AztecQRGenerator",
                    "Logs"
                );
                
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
                
                logFilePath = Path.Combine(logDirectory, $"AztecQR_{DateTime.Now:yyyyMMdd}.log");
                
                // Test write access
                File.AppendAllText(logFilePath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Logger initialized in Documents{Environment.NewLine}");
                return;
            }
            catch
            {
                // Documents failed, try next option
            }

            // Option 3: Temp folder with subdirectory
            try
            {
                logDirectory = Path.Combine(Path.GetTempPath(), "AztecQRGenerator", "Logs");
                
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
                
                logFilePath = Path.Combine(logDirectory, $"AztecQR_{DateTime.Now:yyyyMMdd}.log");
                
                // Test write access
                File.AppendAllText(logFilePath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Logger initialized in Temp{Environment.NewLine}");
                return;
            }
            catch
            {
                // Temp folder failed, use final fallback
            }

            // Option 4: Direct temp file (last resort)
            try
            {
                logFilePath = Path.Combine(Path.GetTempPath(), $"AztecQR_{DateTime.Now:yyyyMMdd}.log");
                File.AppendAllText(logFilePath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Logger initialized in Temp (fallback){Environment.NewLine}");
            }
            catch
            {
                // Even temp file failed - use null file path (logging will be disabled)
                logFilePath = null;
            }
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

            // If no valid log file path, fail silently
            if (string.IsNullOrEmpty(logFilePath))
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
            if (string.IsNullOrEmpty(logFilePath))
            {
                return;
            }

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

        /// <summary>
        /// Gets the current log file path
        /// </summary>
        public string GetLogFilePath()
        {
            return logFilePath ?? "Logging disabled (no writable location found)";
        }
    }
}
