using NUnit.Framework;
using AztecQR;
using System;

namespace AztecQRGenerator.Tests
{
    [TestFixture]
    public class LoggerTests
    {
        [Test]
        public void Logger_Instance_IsSingleton()
        {
            // Act
            var instance1 = Logger.Instance;
            var instance2 = Logger.Instance;

            // Assert
            Assert.AreSame(instance1, instance2, "Logger should be a singleton");
        }

        [Test]
        public void Logger_GetLogFilePath_ReturnsValidPath()
        {
            // Act
            string logPath = Logger.Instance.GetLogFilePath();

            // Assert
            Assert.IsNotNull(logPath);
            Assert.IsNotEmpty(logPath);
        }

        [Test]
        public void Logger_Info_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => Logger.Instance.Info("Test info message"));
        }

        [Test]
        public void Logger_Debug_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => Logger.Instance.Debug("Test debug message"));
        }

        [Test]
        public void Logger_Warning_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => Logger.Instance.Warning("Test warning message"));
        }

        [Test]
        public void Logger_Error_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => Logger.Instance.Error("Test error message"));
        }

        [Test]
        public void Logger_ErrorWithException_DoesNotThrow()
        {
            // Arrange
            var exception = new Exception("Test exception");

            // Act & Assert
            Assert.DoesNotThrow(() => Logger.Instance.Error("Test error with exception", exception));
        }

        [Test]
        public void Logger_SetMinimumLogLevel_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => Logger.Instance.SetMinimumLogLevel(LogLevel.Debug));
            Assert.DoesNotThrow(() => Logger.Instance.SetMinimumLogLevel(LogLevel.Info));
            Assert.DoesNotThrow(() => Logger.Instance.SetMinimumLogLevel(LogLevel.Warning));
            Assert.DoesNotThrow(() => Logger.Instance.SetMinimumLogLevel(LogLevel.Error));
        }

        [Test]
        public void Logger_LogMethodEntry_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => 
                Logger.Instance.LogMethodEntry("TestClass", "TestMethod", "param1", 123));
        }

        [Test]
        public void Logger_LogMethodExit_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => 
                Logger.Instance.LogMethodExit("TestClass", "TestMethod", true));
        }
    }
}
