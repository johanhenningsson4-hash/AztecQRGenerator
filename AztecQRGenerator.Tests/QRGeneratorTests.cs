using NUnit.Framework;
using AztecQR;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace AztecQRGenerator.Tests
{
    [TestFixture]
    public class QRGeneratorTests
    {
        private QRGenerator generator;

        [SetUp]
        public void Setup()
        {
            generator = new QRGenerator();
        }

        [TearDown]
        public void TearDown()
        {
            generator = null;
        }

        #region GenerateQRCodeAsBitmap Tests

        [Test]
        public void GenerateQRCodeAsBitmap_ValidInput_ReturnsBitmap()
        {
            // Arrange
            string base64Data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Hello World"));
            int correction = 2;
            int pixelDensity = 300;

            // Act
            Bitmap result = generator.GenerateQRCodeAsBitmap(base64Data, correction, pixelDensity);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(pixelDensity, result.Width);
            Assert.AreEqual(pixelDensity, result.Height);
            
            // Cleanup
            result.Dispose();
        }

        [Test]
        public void GenerateQRCodeAsBitmap_NullInput_ThrowsArgumentException()
        {
            // Arrange
            string base64Data = null;
            int correction = 2;
            int pixelDensity = 300;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                generator.GenerateQRCodeAsBitmap(base64Data, correction, pixelDensity));
        }

        [Test]
        public void GenerateQRCodeAsBitmap_EmptyString_ThrowsArgumentException()
        {
            // Arrange
            string base64Data = "";
            int correction = 2;
            int pixelDensity = 300;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                generator.GenerateQRCodeAsBitmap(base64Data, correction, pixelDensity));
        }

        [Test]
        public void GenerateQRCodeAsBitmap_InvalidBase64_ThrowsArgumentException()
        {
            // Arrange
            string base64Data = "InvalidBase64!@#$%";
            int correction = 2;
            int pixelDensity = 300;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                generator.GenerateQRCodeAsBitmap(base64Data, correction, pixelDensity));
        }

        [Test]
        public void GenerateQRCodeAsBitmap_ZeroPixelDensity_ThrowsArgumentException()
        {
            // Arrange
            string base64Data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Test"));
            int correction = 2;
            int pixelDensity = 0;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                generator.GenerateQRCodeAsBitmap(base64Data, correction, pixelDensity));
        }

        [Test]
        public void GenerateQRCodeAsBitmap_NegativePixelDensity_ThrowsArgumentException()
        {
            // Arrange
            string base64Data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Test"));
            int correction = 2;
            int pixelDensity = -100;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                generator.GenerateQRCodeAsBitmap(base64Data, correction, pixelDensity));
        }

        [Test]
        [TestCase(200)]
        [TestCase(300)]
        [TestCase(500)]
        public void GenerateQRCodeAsBitmap_DifferentSizes_ReturnsCorrectSize(int size)
        {
            // Arrange
            string base64Data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Test"));
            int correction = 2;

            // Act
            Bitmap result = generator.GenerateQRCodeAsBitmap(base64Data, correction, size);

            // Assert
            Assert.AreEqual(size, result.Width);
            Assert.AreEqual(size, result.Height);
            
            // Cleanup
            result.Dispose();
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(10)]
        public void GenerateQRCodeAsBitmap_DifferentCorrectionLevels_GeneratesSuccessfully(int correction)
        {
            // Arrange
            string base64Data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Test"));
            int pixelDensity = 300;

            // Act
            Bitmap result = generator.GenerateQRCodeAsBitmap(base64Data, correction, pixelDensity);

            // Assert
            Assert.IsNotNull(result);
            
            // Cleanup
            result.Dispose();
        }

        #endregion

        #region GenerateQRCodeToFile Tests

        [Test]
        public void GenerateQRCodeToFile_ValidInput_ReturnsTrue()
        {
            // Arrange
            string base64Data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Test"));
            string fileName = $"test_qr_{Guid.NewGuid()}.png";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string outputPath = System.IO.Path.Combine(documentsPath, "AztecQRGenerator", "Output", fileName);

            try
            {
                // Act
                bool result = generator.GenerateQRCodeToFile(base64Data, 2, 300, fileName, ImageFormat.Png);

                // Assert
                Assert.IsTrue(result);
                Assert.IsTrue(System.IO.File.Exists(outputPath), "QR code file should exist");
            }
            finally
            {
                // Cleanup
                if (System.IO.File.Exists(outputPath))
                {
                    System.IO.File.Delete(outputPath);
                }
            }
        }

        [Test]
        public void GenerateQRCodeToFile_NullFilePath_ThrowsArgumentException()
        {
            // Arrange
            string base64Data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Test"));

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                generator.GenerateQRCodeToFile(base64Data, 2, 300, null, ImageFormat.Png));
        }

        [Test]
        public void GenerateQRCodeToFile_NullFormat_ThrowsArgumentNullException()
        {
            // Arrange
            string base64Data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Test"));
            string fileName = "test.png";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                generator.GenerateQRCodeToFile(base64Data, 2, 300, fileName, null));
        }

        [Test]
        [TestCase("png", typeof(ImageFormat))]
        [TestCase("jpg", typeof(ImageFormat))]
        [TestCase("bmp", typeof(ImageFormat))]
        public void GenerateQRCodeToFile_DifferentFormats_GeneratesSuccessfully(string extension, Type formatType)
        {
            // Arrange
            string base64Data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Test"));
            string fileName = $"test_qr_{Guid.NewGuid()}.{extension}";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string outputPath = System.IO.Path.Combine(documentsPath, "AztecQRGenerator", "Output", fileName);

            ImageFormat format = extension == "png" ? ImageFormat.Png :
                               extension == "jpg" ? ImageFormat.Jpeg :
                               ImageFormat.Bmp;

            try
            {
                // Act
                bool result = generator.GenerateQRCodeToFile(base64Data, 2, 300, fileName, format);

                // Assert
                Assert.IsTrue(result);
                Assert.IsTrue(System.IO.File.Exists(outputPath));
            }
            finally
            {
                // Cleanup
                if (System.IO.File.Exists(outputPath))
                {
                    System.IO.File.Delete(outputPath);
                }
            }
        }

        #endregion

        #region GenerateQRBitmap Tests

        [Test]
        public void GenerateQRBitmap_ValidInput_ReturnsTrue()
        {
            // Arrange
            string base64Data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Test"));
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string outputDir = System.IO.Path.Combine(documentsPath, "AztecQRGenerator", "Output");

            // Get files before generation
            var filesBefore = System.IO.Directory.Exists(outputDir) 
                ? System.IO.Directory.GetFiles(outputDir, "QRCode_*.png")
                : new string[0];

            try
            {
                // Act
                bool result = generator.GenerateQRBitmap(base64Data, 2, 300);

                // Assert
                Assert.IsTrue(result);

                // Verify file was created
                var filesAfter = System.IO.Directory.GetFiles(outputDir, "QRCode_*.png");
                Assert.That(filesAfter.Length, Is.GreaterThan(filesBefore.Length), "New file should be created");
            }
            finally
            {
                // Cleanup - remove newly created files
                if (System.IO.Directory.Exists(outputDir))
                {
                    var filesAfter = System.IO.Directory.GetFiles(outputDir, "QRCode_*.png");
                    foreach (var file in filesAfter)
                    {
                        if (!filesBefore.Contains(file))
                        {
                            System.IO.File.Delete(file);
                        }
                    }
                }
            }
        }

        #endregion

        #region Edge Cases and Integration Tests

        [Test]
        public void GenerateQRCodeAsBitmap_LargeData_GeneratesSuccessfully()
        {
            // Arrange
            string largeText = new string('A', 2000);
            string base64Data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(largeText));
            int correction = 2;
            int pixelDensity = 500;

            // Act
            Bitmap result = generator.GenerateQRCodeAsBitmap(base64Data, correction, pixelDensity);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(pixelDensity, result.Width);
            
            // Cleanup
            result.Dispose();
        }

        [Test]
        public void GenerateQRCodeAsBitmap_SpecialCharacters_GeneratesSuccessfully()
        {
            // Arrange
            string specialText = "Hello! @#$%^&*() 123 ραινσϊ";
            string base64Data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(specialText));
            int correction = 2;
            int pixelDensity = 300;

            // Act
            Bitmap result = generator.GenerateQRCodeAsBitmap(base64Data, correction, pixelDensity);

            // Assert
            Assert.IsNotNull(result);
            
            // Cleanup
            result.Dispose();
        }

        [Test]
        public void GenerateQRCodeAsBitmap_MinimumSize_GeneratesSuccessfully()
        {
            // Arrange
            string base64Data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("A"));
            int correction = 2;
            int pixelDensity = 50;

            // Act
            Bitmap result = generator.GenerateQRCodeAsBitmap(base64Data, correction, pixelDensity);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(pixelDensity, result.Width);
            
            // Cleanup
            result.Dispose();
        }

        #endregion
    }
}
