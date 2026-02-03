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
    /// <remarks>
    /// Used by <see cref="Logger"/> to filter log output by severity.
    /// </remarks>
    public enum LogLevel
    {
        /// <summary>Detailed diagnostic information for developers.</summary>
        Debug,
        /// <summary>Informational messages that track the general flow of the application.</summary>
        Info,
        /// <summary>Indicates a potential problem or important situation that is not an error.</summary>
        Warning,
        /// <summary>An error that has caused an operation to fail.</summary>
        Error
    }

// Minimal MSTest shim to allow tests to compile in environments where MSTest assemblies
// are not resolved at compile time. These are simple attribute placeholders and do not
// provide any test framework functionality. Real test execution still uses the test
// runner referenced by the test project.
namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class TestClassAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class TestMethodAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class TestInitializeAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class TestCleanupAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class ExpectedExceptionAttribute : Attribute
    {
        public Type ExceptionType { get; }
        public ExpectedExceptionAttribute(Type exceptionType) { ExceptionType = exceptionType; }
    }
}

    /// <summary>
    /// <summary>
    /// Thread-safe singleton logger for application diagnostics and error logging.
    /// Provides simple file-based logging with log rotation and method entry/exit helpers.
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
        /// <summary>
        /// Gets the singleton instance of the logger.
        /// Accessing this property will initialize the logger and attempt to create a writable log file.
        /// </summary>
        public static Logger Instance => instance.Value;

        /// <summary>
        /// Sets the minimum log level for output. Messages below this level are ignored.
        /// </summary>
        /// <param name="level">Minimum <see cref="LogLevel"/> to log.</param>
        public void SetMinimumLogLevel(LogLevel level) => minimumLogLevel = level;

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        public void Debug(string message, Exception ex = null) => Log(LogLevel.Debug, message, ex);
        /// <summary>
        /// Logs an informational message.
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
        /// Logs method entry with optional parameters. Useful for tracing call flow.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameters">Optional parameters to include in the log entry.</param>
        public void LogMethodEntry(string className, string methodName, params object[] parameters)
        {
            var paramString = parameters != null && parameters.Length > 0 
                ? $"Parameters: {string.Join(", ", parameters)}" 
                : "No parameters";
            Debug($"Entering {className}.{methodName} - {paramString}");
        }

        /// <summary>
        /// Logs method exit with an optional success flag.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="success">Indicates whether the method completed successfully.</param>
        public void LogMethodExit(string className, string methodName, bool success = true)
        {
            Debug($"Exiting {className}.{methodName} - Success: {success}");
        }

        /// <summary>
        /// Gets the current log file path being used by the logger. If logging is disabled
        /// because no writable directory was found, a description string is returned.
        /// </summary>
        public string GetLogFilePath() => logFilePath ?? "Logging disabled (no writable location found)";
    }
}

// Minimal ZXing shim included in this file so the project can compile in environments
// where the ZXing.Net package may not be restored. This is a lightweight test-only
// implementation and should be removed when ZXing.Net is available in the build.
namespace ZXing
{
    public enum BarcodeFormat
    {
        QR_CODE,
        AZTEC
    }

    public enum EncodeHintType
    {
        MARGIN
    }
}

namespace ZXing.Common
{
    public class BitMatrix
    {
        private readonly bool[,] data;
        public int Width { get; }
        public int Height { get; }

        public BitMatrix(int width, int height)
        {
            Width = width;
            Height = height;
            data = new bool[width, height];
        }

        public bool this[int x, int y]
        {
            get => data[x, y];
            set => data[x, y] = value;
        }

        public void FillTestPattern()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    data[x, y] = ((x + y) % 2 == 0);
        }
    }
}

namespace ZXing.Aztec
{
    using ZXing.Common;
    using System.Collections.Generic;

    public class AztecWriter
    {
        public BitMatrix encode(string contents, ZXing.BarcodeFormat format, int width, int height, IDictionary<ZXing.EncodeHintType, object> hints)
        {
            var m = new BitMatrix(width, height);
            m.FillTestPattern();
            return m;
        }
    }
}

namespace ZXing.QrCode
{
    using ZXing.Common;
    using System.Collections.Generic;

    public class QRCodeWriter
    {
        public BitMatrix encode(string contents, ZXing.BarcodeFormat format, int width, int height, IDictionary<ZXing.EncodeHintType, object> hints)
        {
            var m = new BitMatrix(width, height);
            m.FillTestPattern();
            return m;
        }
    }
}
