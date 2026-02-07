using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AztecQR.Tests
{
    [TestClass]
    public class FlexibleInputTests
    {
        private QRGenerator qrGenerator;
        private string testText;
        private byte[] testBytes;

        [TestInitialize]
        public void Setup()
        {
            this.qrGenerator = new QRGenerator();
            this.testText = "Hello, Flexible Input!";
            this.testBytes = Encoding.UTF8.GetBytes(this.testText);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.qrGenerator = null;
        }

        // Region: Text Input Tests

        [TestMethod]
        public void GenerateQRCodeFromText_ValidInput_ReturnsBitmap()
        {
            // Act
            using (Bitmap result = this.qrGenerator.GenerateQRCodeFromText(this.testText))
            {
                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(300, result.Width);
                Assert.AreEqual(300, result.Height);
            }
        }

        [TestMethod]
        public void GenerateQRCodeFromText_NullInput_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                this.qrGenerator.GenerateQRCodeFromText(null));
        }

        [TestMethod]
        public void GenerateQRCodeFromText_EmptyInput_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                this.qrGenerator.GenerateQRCodeFromText(""));
        }

        [TestMethod]
        public void GenerateQRCodeFromText_CustomSize_ReturnsCorrectSize()
        {
            // Act
            using (Bitmap result = this.qrGenerator.GenerateQRCodeFromText(this.testText, lPixelDensity: 500))
            {
                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(500, result.Width);
                Assert.AreEqual(500, result.Height);
            }
        }

        [TestMethod]
        public void GenerateQRCodeFromText_UnicodeText_GeneratesSuccessfully()
        {
            // Arrange
            string unicodeText = "Hello 世界! 🚀 Émojis";

            // Act
            using (Bitmap result = this.qrGenerator.GenerateQRCodeFromText(unicodeText, encoding: Encoding.UTF8))
            {
                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(300, result.Width);
            }
        }

        [TestMethod]
        public void GenerateQRCodeFromTextToFile_ValidInput_ReturnsTrue()
        {
            // Arrange
            string filePath = $"test_text_{DateTime.Now:yyyyMMddHHmmssfff}.png";

            try
            {
                // Act
                bool result = this.qrGenerator.GenerateQRCodeFromTextToFile(
                    this.testText, filePath, ImageFormat.Png);

                // Assert
                Assert.IsTrue(result);
            }
            finally
            {
                // Cleanup
                try { System.IO.File.Delete(filePath); } catch { }
            }
        }

        // Region: Byte Array Tests

        [TestMethod]
        public void GenerateQRCodeFromBytes_ValidInput_ReturnsBitmap()
        {
            // Act
            using (Bitmap result = this.qrGenerator.GenerateQRCodeFromBytes(this.testBytes))
            {
                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(300, result.Width);
                Assert.AreEqual(300, result.Height);
            }
        }

        [TestMethod]
        public void GenerateQRCodeFromBytes_NullInput_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                this.qrGenerator.GenerateQRCodeFromBytes(null));
        }

        [TestMethod]
        public void GenerateQRCodeFromBytes_EmptyArray_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                this.qrGenerator.GenerateQRCodeFromBytes(new byte[0]));
        }

        [TestMethod]
        public void GenerateQRCodeFromBytes_BinaryData_GeneratesSuccessfully()
        {
            // Arrange
            byte[] binaryData = { 0x00, 0xFF, 0x7F, 0x80, 0x01, 0xFE };

            // Act
            using (Bitmap result = this.qrGenerator.GenerateQRCodeFromBytes(binaryData))
            {
                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(300, result.Width);
            }
        }

        [TestMethod]
        public void GenerateQRCodeFromBytesToFile_ValidInput_ReturnsTrue()
        {
            // Arrange
            string filePath = $"test_bytes_{DateTime.Now:yyyyMMddHHmmssfff}.png";

            try
            {
                // Act
                bool result = this.qrGenerator.GenerateQRCodeFromBytesToFile(
                    this.testBytes, filePath, ImageFormat.Png);

                // Assert
                Assert.IsTrue(result);
            }
            finally
            {
                // Cleanup
                try { System.IO.File.Delete(filePath); } catch { }
            }
        }

        // Region: Async Tests

        [TestMethod]
        public async Task GenerateQRCodeFromTextAsync_ValidInput_ReturnsBitmap()
        {
            // Act
            using (Bitmap result = await this.qrGenerator.GenerateQRCodeFromTextAsync(this.testText))
            {
                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(300, result.Width);
                Assert.AreEqual(300, result.Height);
            }
        }

        [TestMethod]
        public async Task GenerateQRCodeFromTextAsync_WithCancellation_ThrowsOperationCanceledException()
        {
            // Arrange
            using (var cts = new CancellationTokenSource())
            {
                cts.Cancel(); // Cancel immediately

                // Act & Assert
                await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () =>
                    await this.qrGenerator.GenerateQRCodeFromTextAsync(this.testText, cancellationToken: cts.Token));
            }
        }

        [TestMethod]
        public async Task GenerateQRCodeFromBytesAsync_ValidInput_ReturnsBitmap()
        {
            // Act
            using (Bitmap result = await this.qrGenerator.GenerateQRCodeFromBytesAsync(this.testBytes))
            {
                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(300, result.Width);
            }
        }

        [TestMethod]
        public async Task GenerateQRCodeFromTextToFileAsync_ValidInput_ReturnsTrue()
        {
            // Arrange
            string filePath = $"test_text_async_{DateTime.Now:yyyyMMddHHmmssfff}.png";

            try
            {
                // Act
                bool result = await this.qrGenerator.GenerateQRCodeFromTextToFileAsync(
                    this.testText, filePath, ImageFormat.Png);

                // Assert
                Assert.IsTrue(result);
            }
            finally
            {
                // Cleanup
                try { System.IO.File.Delete(filePath); } catch { }
            }
        }

        [TestMethod]
        public async Task GenerateQRCodeFromBytesToFileAsync_ValidInput_ReturnsTrue()
        {
            // Arrange
            string filePath = $"test_bytes_async_{DateTime.Now:yyyyMMddHHmmssfff}.png";

            try
            {
                // Act
                bool result = await this.qrGenerator.GenerateQRCodeFromBytesToFileAsync(
                    this.testBytes, filePath, ImageFormat.Png);

                // Assert
                Assert.IsTrue(result);
            }
            finally
            {
                // Cleanup
                try { System.IO.File.Delete(filePath); } catch { }
            }
        }

        // Region: Consistency Tests

        [TestMethod]
        public void DifferentInputMethods_SameData_ProduceSimilarResults()
        {
            // Arrange
            string originalText = "Consistency Test Data";
            byte[] textBytes = Encoding.UTF8.GetBytes(originalText);
            string base64Text = Convert.ToBase64String(textBytes);

            // Act
            using (Bitmap fromText = this.qrGenerator.GenerateQRCodeFromText(originalText))
            using (Bitmap fromBytes = this.qrGenerator.GenerateQRCodeFromBytes(textBytes))
            using (Bitmap fromBase64 = this.qrGenerator.GenerateQRCodeAsBitmap(base64Text, 2, 300))
            {
                // Assert - All should have same dimensions
                Assert.AreEqual(fromText.Width, fromBytes.Width);
                Assert.AreEqual(fromText.Height, fromBytes.Height);
                Assert.AreEqual(fromBytes.Width, fromBase64.Width);
                Assert.AreEqual(fromBytes.Height, fromBase64.Height);
            }
        }

        [TestMethod]
        public void CustomParameters_AllInputTypes_RespectParameters()
        {
            // Arrange
            int customSize = 400;
            int customCorrection = 3;

            // Act
            using (Bitmap fromText = this.qrGenerator.GenerateQRCodeFromText(
                this.testText, customCorrection, customSize))
            using (Bitmap fromBytes = this.qrGenerator.GenerateQRCodeFromBytes(
                this.testBytes, customCorrection, customSize))
            {
                // Assert
                Assert.AreEqual(customSize, fromText.Width);
                Assert.AreEqual(customSize, fromBytes.Width);
            }
        }

        // Region: Error Handling Tests

        [TestMethod]
        public void GenerateQRCodeFromText_InvalidPixelDensity_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                this.qrGenerator.GenerateQRCodeFromText(this.testText, lPixelDensity: -100));
        }

        [TestMethod]
        public void GenerateQRCodeFromBytes_InvalidPixelDensity_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                this.qrGenerator.GenerateQRCodeFromBytes(this.testBytes, lPixelDensity: 0));
        }

        [TestMethod]
        public void GenerateQRCodeFromTextToFile_NullFilePath_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                this.qrGenerator.GenerateQRCodeFromTextToFile(this.testText, null, ImageFormat.Png));
        }

        [TestMethod]
        public void GenerateQRCodeFromBytesToFile_NullFormat_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                this.qrGenerator.GenerateQRCodeFromBytesToFile(this.testBytes, "test.png", null));
        }
    }
}
