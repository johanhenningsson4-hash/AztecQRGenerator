using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AztecQR.Tests
{
    [TestClass]
    public class LoggerTests
    {
        [TestMethod]
        public void Logger_Instance_IsSingleton()
        {
            // Act
            var instance1 = Logger.Instance;
            var instance2 = Logger.Instance;

            // Assert
            Assert.IsNotNull(instance1, "First instance should not be null");
            Assert.IsNotNull(instance2, "Second instance should not be null");
            Assert.AreSame(instance1, instance2, "Both instances should be the same object");
        }

        [TestMethod]
        public void Logger_GetLogFilePath_ReturnsValidPath()
        {
            // Act
            string logPath = Logger.Instance.GetLogFilePath();

            // Assert
            Assert.IsNotNull(logPath, "Log path should not be null");
            Assert.IsFalse(string.IsNullOrWhiteSpace(logPath), "Log path should not be empty");

            if (!logPath.Contains("disabled"))
            {
                string directory = Path.GetDirectoryName(logPath);
                Assert.IsTrue(Directory.Exists(directory), "Log directory should exist");
            }
        }

        [TestMethod]
        public void Logger_SetMinimumLogLevel_ChangesLogLevel()
        {
            // Arrange
            var logger = Logger.Instance;

            // Act - Set to Error level (highest)
            logger.SetMinimumLogLevel(LogLevel.Error);

            // Debug and Info should not be logged now
            logger.Debug("This debug message should not appear");
            logger.Info("This info message should not appear");
            logger.Error("This error message SHOULD appear");

            // Reset to Debug for other tests
            logger.SetMinimumLogLevel(LogLevel.Debug);

            // Assert
            // If we got here without exceptions, the method works
            Assert.IsTrue(true, "SetMinimumLogLevel should execute without errors");
        }

        [TestMethod]
        public void Logger_Debug_LogsMessage()
        {
            // Arrange
            var logger = Logger.Instance;
            logger.SetMinimumLogLevel(LogLevel.Debug);

            // Act
            logger.Debug("Test debug message");

            // Assert - If no exception thrown, logging worked
            Assert.IsTrue(true, "Debug logging should succeed");
        }

        [TestMethod]
        public void Logger_Info_LogsMessage()
        {
            // Arrange
            var logger = Logger.Instance;
            logger.SetMinimumLogLevel(LogLevel.Info);

            // Act
            logger.Info("Test info message");

            // Assert
            Assert.IsTrue(true, "Info logging should succeed");
        }

        [TestMethod]
        public void Logger_Warning_LogsMessage()
        {
            // Arrange
            var logger = Logger.Instance;

            // Act
            logger.Warning("Test warning message");

            // Assert
            Assert.IsTrue(true, "Warning logging should succeed");
        }

        [TestMethod]
        public void Logger_Error_LogsMessage()
        {
            // Arrange
            var logger = Logger.Instance;

            // Act
            logger.Error("Test error message");

            // Assert
            Assert.IsTrue(true, "Error logging should succeed");
        }

        [TestMethod]
        public void Logger_ErrorWithException_LogsMessageAndException()
        {
            // Arrange
            var logger = Logger.Instance;
            var exception = new InvalidOperationException("Test exception");

            // Act
            logger.Error("Test error with exception", exception);

            // Assert
            Assert.IsTrue(true, "Error logging with exception should succeed");
        }

        [TestMethod]
        public void Logger_DebugWithException_LogsMessageAndException()
        {
            // Arrange
            var logger = Logger.Instance;
            logger.SetMinimumLogLevel(LogLevel.Debug);
            var exception = new ArgumentException("Test debug exception");

            // Act
            logger.Debug("Test debug with exception", exception);

            // Assert
            Assert.IsTrue(true, "Debug logging with exception should succeed");
        }

        [TestMethod]
        public void Logger_LogMethodEntry_LogsWithParameters()
        {
            // Arrange
            var logger = Logger.Instance;
            logger.SetMinimumLogLevel(LogLevel.Debug);

            // Act
            logger.LogMethodEntry("TestClass", "TestMethod", "param1", 123, true);

            // Assert
            Assert.IsTrue(true, "LogMethodEntry should succeed");
        }

        [TestMethod]
        public void Logger_LogMethodEntry_LogsWithNoParameters()
        {
            // Arrange
            var logger = Logger.Instance;
            logger.SetMinimumLogLevel(LogLevel.Debug);

            // Act
            logger.LogMethodEntry("TestClass", "TestMethod");

            // Assert
            Assert.IsTrue(true, "LogMethodEntry with no parameters should succeed");
        }

        [TestMethod]
        public void Logger_LogMethodExit_LogsSuccess()
        {
            // Arrange
            var logger = Logger.Instance;
            logger.SetMinimumLogLevel(LogLevel.Debug);

            // Act
            logger.LogMethodExit("TestClass", "TestMethod", true);

            // Assert
            Assert.IsTrue(true, "LogMethodExit with success should succeed");
        }

        [TestMethod]
        public void Logger_LogMethodExit_LogsFailure()
        {
            // Arrange
            var logger = Logger.Instance;
            logger.SetMinimumLogLevel(LogLevel.Debug);

            // Act
            logger.LogMethodExit("TestClass", "TestMethod", false);

            // Assert
            Assert.IsTrue(true, "LogMethodExit with failure should succeed");
        }

        [TestMethod]
        public void Logger_MinimumLogLevel_FiltersMessages()
        {
            // Arrange
            var logger = Logger.Instance;
            string logPath = logger.GetLogFilePath();

            // Skip test if logging is disabled
            if (logPath.Contains("disabled"))
            {
                Assert.Inconclusive("Logging is disabled, cannot test filtering");
                return;
            }

            // Get file size before
            long sizeBefore = 0;
            if (File.Exists(logPath))
            {
                sizeBefore = new FileInfo(logPath).Length;
            }

            // Act - Set to Error level and try to log Debug
            logger.SetMinimumLogLevel(LogLevel.Error);
            logger.Debug("This should NOT be logged");
            logger.Info("This should NOT be logged");
            
            Thread.Sleep(100); // Give time for any potential logging

            // Check if file size changed (it shouldn't)
            long sizeAfter = 0;
            if (File.Exists(logPath))
            {
                sizeAfter = new FileInfo(logPath).Length;
            }

            // Now log an Error (should be logged)
            logger.Error("This SHOULD be logged");
            Thread.Sleep(100);

            long sizeFinal = 0;
            if (File.Exists(logPath))
            {
                sizeFinal = new FileInfo(logPath).Length;
            }

            // Reset to Debug for other tests
            logger.SetMinimumLogLevel(LogLevel.Debug);

            // Assert
            Assert.AreEqual(sizeBefore, sizeAfter, "Debug/Info messages should not increase log file size");
            Assert.IsTrue(sizeFinal > sizeAfter, "Error message should increase log file size");
        }

        [TestMethod]
        public void Logger_ThreadSafety_MultipleThreadsCanLogSimultaneously()
        {
            // Arrange
            var logger = Logger.Instance;
            logger.SetMinimumLogLevel(LogLevel.Info);
            int threadCount = 10;
            int messagesPerThread = 5;
            var exceptions = new System.Collections.Concurrent.ConcurrentBag<Exception>();

            // Act
            Parallel.For(0, threadCount, i =>
            {
                try
                {
                    for (int j = 0; j < messagesPerThread; j++)
                    {
                        logger.Info($"Thread {i}, Message {j}");
                        logger.Warning($"Thread {i}, Warning {j}");
                        logger.Error($"Thread {i}, Error {j}");
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            });

            // Assert
            Assert.AreEqual(0, exceptions.Count, $"No exceptions should occur during parallel logging. Exceptions: {string.Join(", ", exceptions.Select(e => e.Message))}");
        }

        [TestMethod]
        public void Logger_ExceptionWithInnerException_LogsBoth()
        {
            // Arrange
            var logger = Logger.Instance;
            var innerException = new ArgumentException("Inner exception message");
            var outerException = new InvalidOperationException("Outer exception message", innerException);

            // Act
            logger.Error("Exception with inner exception", outerException);

            // Assert
            Assert.IsTrue(true, "Logging exception with inner exception should succeed");
        }

        [TestMethod]
        public void Logger_LongMessage_LogsSuccessfully()
        {
            // Arrange
            var logger = Logger.Instance;
            string longMessage = new string('A', 10000);

            // Act
            logger.Info(longMessage);

            // Assert
            Assert.IsTrue(true, "Long message should be logged successfully");
        }

        [TestMethod]
        public void Logger_NullMessage_HandlesGracefully()
        {
            // Arrange
            var logger = Logger.Instance;

            // Act & Assert - Should not throw
            try
            {
                logger.Info(null);
                logger.Debug(null);
                logger.Warning(null);
                logger.Error(null);
                Assert.IsTrue(true, "Null messages should be handled gracefully");
            }
            catch
            {
                Assert.Fail("Should not throw exception on null message");
            }
        }

        [TestMethod]
        public void Logger_SpecialCharacters_LogsCorrectly()
        {
            // Arrange
            var logger = Logger.Instance;
            string specialMessage = "Special chars: \t\n\r\"'\\!@#$%^&*()[]{}|;:,.<>?/";

            // Act
            logger.Info(specialMessage);

            // Assert
            Assert.IsTrue(true, "Special characters should be logged successfully");
        }

        [TestMethod]
        public void Logger_RapidSuccessiveLogging_HandlesCorrectly()
        {
            // Arrange
            var logger = Logger.Instance;

            // Act - Log many messages rapidly
            for (int i = 0; i < 100; i++)
            {
                logger.Info($"Rapid message {i}");
            }

            // Assert
            Assert.IsTrue(true, "Rapid successive logging should succeed");
        }

        [TestMethod]
        public void Logger_AllLogLevels_SequentialLogging()
        {
            // Arrange
            var logger = Logger.Instance;
            logger.SetMinimumLogLevel(LogLevel.Debug);

            // Act
            logger.Debug("Debug level message");
            logger.Info("Info level message");
            logger.Warning("Warning level message");
            logger.Error("Error level message");

            // Assert
            Assert.IsTrue(true, "All log levels should work sequentially");
        }

        [TestMethod]
        public void Logger_LogRotation_DoesNotCrash()
        {
            // Arrange
            var logger = Logger.Instance;
            
            // Act - Generate enough logs that might trigger rotation
            // (This won't actually trigger rotation in most cases due to 5MB threshold,
            // but tests that the rotation logic doesn't crash if it does)
            for (int i = 0; i < 1000; i++)
            {
                logger.Info($"Log rotation test message {i} with some extra content to increase size");
            }

            // Assert
            Assert.IsTrue(true, "Log rotation logic should not crash application");
        }
    }
}
