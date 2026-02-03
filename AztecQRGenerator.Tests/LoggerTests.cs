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
            Assert.That(instance1, Is.SameAs(instance2), "Logger should be a singleton");
        }

        [Test]
        public void Logger_GetLogFilePath_ReturnsValidPath()
        {
            // Act
            string logPath = Logger.Instance.GetLogFilePath();

            // Assert
            Assert.IsNotNull(logPath);
            Assert.That(logPath, Is.Not.Empty);
        }

        [Test]
        public void Logger_Info_DoesNotThrow()
        {
            // Act & Assert
            Assert.That(() => Logger.Instance.Info("Test info message"), Throws.Nothing);
        }

        [Test]
        public void Logger_Debug_DoesNotThrow()
        {
            // Act & Assert
            Assert.That(() => Logger.Instance.Debug("Test debug message"), Throws.Nothing);
        }

        [Test]
        public void Logger_Warning_DoesNotThrow()
        {
            // Act & Assert
            Assert.That(() => Logger.Instance.Warning("Test warning message"), Throws.Nothing);
        }

        [Test]
        public void Logger_Error_DoesNotThrow()
        {
            // Act & Assert
            Assert.That(() => Logger.Instance.Error("Test error message"), Throws.Nothing);
        }

        [Test]
        public void Logger_ErrorWithException_DoesNotThrow()
        {
            // Arrange
            var exception = new Exception("Test exception");

            // Act & Assert
            Assert.That(() => Logger.Instance.Error("Test error with exception", exception), Throws.Nothing);
        }

        [Test]
        public void Logger_SetMinimumLogLevel_DoesNotThrow()
        {
            // Act & Assert
            Assert.That(() => Logger.Instance.SetMinimumLogLevel(LogLevel.Debug), Throws.Nothing);
            Assert.That(() => Logger.Instance.SetMinimumLogLevel(LogLevel.Info), Throws.Nothing);
            Assert.That(() => Logger.Instance.SetMinimumLogLevel(LogLevel.Warning), Throws.Nothing);
            Assert.That(() => Logger.Instance.SetMinimumLogLevel(LogLevel.Error), Throws.Nothing);
        }

        [Test]
        public void Logger_LogMethodEntry_DoesNotThrow()
        {
            // Act & Assert
            Assert.That(() => 
                Logger.Instance.LogMethodEntry("TestClass", "TestMethod", "param1", 123), Throws.Nothing);
        }

        [Test]
        public void Logger_LogMethodExit_DoesNotThrow()
        {
            // Act & Assert
            Assert.That(() => 
                Logger.Instance.LogMethodExit("TestClass", "TestMethod", true), Throws.Nothing);
        }
    }
}
