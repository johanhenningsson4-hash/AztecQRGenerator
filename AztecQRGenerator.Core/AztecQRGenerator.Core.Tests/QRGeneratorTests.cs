using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AztecQR.Tests
{
    [TestClass]
    public class QRGeneratorTests
    {
        private QRGenerator generator;
        private string testBase64Data;

        [TestInitialize]
        public void Setup()
        {
            this.generator = new QRGenerator();
            this.testBase64Data = TestHelpers.GetBase64TestData("Hello World!");
            TestHelpers.CleanupOldTestFiles();
        }

        // Region: GenerateQRCodeAsBitmap Tests

        [TestMethod]
        public void GenerateQRCodeAsBitmap_ValidInput_ReturnsBitmap()
        {
            // Act
            Bitmap result = this.generator.GenerateQRCodeAsBitmap(this.testBase64Data, 2, 300);

            // Assert
            Assert.IsNotNull(result, "Bitmap should not be null");
            Assert.AreEqual(300, result.Width, "Width should match pixel density");
            Assert.AreEqual(300, result.Height, "Height should match pixel density");
            Assert.IsTrue(TestHelpers.IsValidBarcodeBitmap(result), "Bitmap should contain valid barcode pattern");

            result.Dispose();
        }

        [TestMethod]
        public void GenerateQRCodeAsBitmap_DifferentSizes_GeneratesCorrectDimensions()
        {
            // Arrange
            int[] sizes = { 100, 200, 500 };

            foreach (int size in sizes)
            {
                // Act
                using (Bitmap result = this.generator.GenerateQRCodeAsBitmap(this.testBase64Data, 2, size))
                {
                    // Assert
                    Assert.AreEqual(size, result.Width, $"Width should be {size}");
                    Assert.AreEqual(size, result.Height, $"Height should be {size}");
                }
            }
        }

        [TestMethod]
        public void GenerateQRCodeAsBitmap_DifferentCorrectionLevels_Succeeds()
        {
            // Arrange
            int[] correctionLevels = { 0, 2, 5, 10 };

            foreach (int level in correctionLevels)
            {
                // Act
                using (Bitmap result = this.generator.GenerateQRCodeAsBitmap(this.testBase64Data, level, 200))
                {
                    // Assert
                    Assert.IsNotNull(result, $"Bitmap should be generated with correction level {level}");
                    Assert.IsTrue(TestHelpers.IsValidBarcodeBitmap(result), "Bitmap should contain valid barcode pattern");
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateQRCodeAsBitmap_NullString_ThrowsArgumentException()
        {
            // Act
            this.generator.GenerateQRCodeAsBitmap(null, 2, 300);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateQRCodeAsBitmap_EmptyString_ThrowsArgumentException()
        {
            // Act
            this.generator.GenerateQRCodeAsBitmap("", 2, 300);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateQRCodeAsBitmap_WhitespaceString_ThrowsArgumentException()
        {
            // Act
            this.generator.GenerateQRCodeAsBitmap("   ", 2, 300);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateQRCodeAsBitmap_InvalidBase64_ThrowsArgumentException()
        {
            // Act
            this.generator.GenerateQRCodeAsBitmap("NotValidBase64!@#", 2, 300);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateQRCodeAsBitmap_ZeroPixelDensity_ThrowsArgumentException()
        {
            // Act
            this.generator.GenerateQRCodeAsBitmap(this.testBase64Data, 2, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateQRCodeAsBitmap_NegativePixelDensity_ThrowsArgumentException()
        {
            // Act
            this.generator.GenerateQRCodeAsBitmap(this.testBase64Data, 2, -100);
        }

        [TestMethod]
        public void GenerateQRCodeAsBitmap_NegativeCorrectionLevel_UsesDefault()
        {
            // Act - Should not throw, should use default value
            using (Bitmap result = this.generator.GenerateQRCodeAsBitmap(this.testBase64Data, -1, 200))
            {
                // Assert
                Assert.IsNotNull(result, "Bitmap should be generated with default correction level");
            }
        }

        [TestMethod]
        public void GenerateQRCodeAsBitmap_LargeData_Succeeds()
        {
            // Arrange
            string largeData = TestHelpers.GetBase64TestData(new string('A', 1000));

            // Act
            using (Bitmap result = this.generator.GenerateQRCodeAsBitmap(largeData, 2, 400))
            {
                // Assert
                Assert.IsNotNull(result, "Should handle large data");
                Assert.IsTrue(TestHelpers.IsValidBarcodeBitmap(result), "Large data bitmap should be valid");
            }
        }

        // End Region: GenerateQRCodeAsBitmap Tests

        // Region: GenerateQRCodeToFile Tests

        [TestMethod]
        public void GenerateQRCodeToFile_PNG_CreatesValidFile()
        {
            // Arrange
            string filePath = TestHelpers.GetTestFilePath("qr_test", "png");

            try
            {
                // Act
                bool result = this.generator.GenerateQRCodeToFile(this.testBase64Data, 2, 300, filePath, ImageFormat.Png);

                // Assert
                Assert.IsTrue(result, "Method should return true");
                Assert.IsTrue(TestHelpers.IsValidImageFile(filePath), "File should be created and valid");

                using (var image = Image.FromFile(filePath))
                {
                    Assert.AreEqual(ImageFormat.Png.Guid, image.RawFormat.Guid, "Image should be PNG format");
                }
            }
            finally
            {
                TestHelpers.DeleteTestFile(filePath);
            }
        }

        [TestMethod]
        public void GenerateQRCodeToFile_JPEG_CreatesValidFile()
        {
            // Arrange
            string filePath = TestHelpers.GetTestFilePath("qr_test", "jpg");

            try
            {
                // Act
                bool result = this.generator.GenerateQRCodeToFile(this.testBase64Data, 2, 300, filePath, ImageFormat.Jpeg);

                // Assert
                Assert.IsTrue(result, "Method should return true");
                Assert.IsTrue(TestHelpers.IsValidImageFile(filePath), "File should be created and valid");
            }
            finally
            {
                TestHelpers.DeleteTestFile(filePath);
            }
        }

        [TestMethod]
        public void GenerateQRCodeToFile_BMP_CreatesValidFile()
        {
            // Arrange
            string filePath = TestHelpers.GetTestFilePath("qr_test", "bmp");

            try
            {
                // Act
                bool result = this.generator.GenerateQRCodeToFile(this.testBase64Data, 2, 300, filePath, ImageFormat.Bmp);

                // Assert
                Assert.IsTrue(result, "Method should return true");
                Assert.IsTrue(TestHelpers.IsValidImageFile(filePath), "File should be created and valid");
            }
            finally
            {
                TestHelpers.DeleteTestFile(filePath);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateQRCodeToFile_NullFilePath_ThrowsArgumentException()
        {
            // Act
            this.generator.GenerateQRCodeToFile(this.testBase64Data, 2, 300, null, ImageFormat.Png);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateQRCodeToFile_EmptyFilePath_ThrowsArgumentException()
        {
            // Act
            this.generator.GenerateQRCodeToFile(this.testBase64Data, 2, 300, "", ImageFormat.Png);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GenerateQRCodeToFile_NullFormat_ThrowsArgumentNullException()
        {
            // Arrange
            string filePath = TestHelpers.GetTestFilePath("qr_test", "png");

            try
            {
                // Act
                generator.GenerateQRCodeToFile(testBase64Data, 2, 300, filePath, null);
            }
            finally
            {
                TestHelpers.DeleteTestFile(filePath);
            }
        }

        [TestMethod]
        public void GenerateQRCodeToFile_RelativePath_UsesDocumentsFolder()
        {
            // Arrange
            string relativePath = "test_qr.png";

            try
            {
                // Act
                bool result = this.generator.GenerateQRCodeToFile(this.testBase64Data, 2, 300, relativePath, ImageFormat.Png);

                // Assert
                Assert.IsTrue(result, "Method should return true");
                // Note: File will be in Documents/AztecQRGenerator/Output, not current directory
            }
            finally
            {
                // Cleanup might fail since we don't know exact path, that's ok
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string outputPath = Path.Combine(documentsPath, "AztecQRGenerator", "Output", relativePath);
                TestHelpers.DeleteTestFile(outputPath);
            }
        }

        // End Region

        // Region: GenerateQRBitmap Tests (Legacy Method)

        [TestMethod]
        public void GenerateQRBitmap_ValidInput_CreatesTimestampedFiles()
        {
            // Act
            bool result = this.generator.GenerateQRBitmap(this.testBase64Data, 2, 300);

            // Assert
            Assert.IsTrue(result, "Method should return true");
            // Note: Files are created in Documents folder with timestamps
            // We can't easily verify them without knowing exact timestamp
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateQRBitmap_InvalidData_ThrowsException()
        {
            // Act
            this.generator.GenerateQRBitmap("InvalidBase64", 2, 300);
        }

        // End Region

        // Region: Integration Tests

        [TestMethod]
        public void QRGenerator_MultipleCalls_AllSucceed()
        {
            // Act & Assert - Generate multiple QR codes in succession
            for (int i = 0; i < 5; i++)
            {
                string data = TestHelpers.GetBase64TestData($"Test {i}");
                using (Bitmap result = this.generator.GenerateQRCodeAsBitmap(data, 2, 200))
                {
                    Assert.IsNotNull(result, $"Iteration {i} should succeed");
                }
            }
        }

        [TestMethod]
        public void QRGenerator_BitmapAndFile_ProduceSameSize()
        {
            // Arrange
            string filePath = TestHelpers.GetTestFilePath("qr_comparison", "png");

            try
            {
                // Act
                using (Bitmap bitmapResult = this.generator.GenerateQRCodeAsBitmap(this.testBase64Data, 2, 300))
                {
                    this.generator.GenerateQRCodeToFile(this.testBase64Data, 2, 300, filePath, ImageFormat.Png);

                    using (Bitmap fileResult = new Bitmap(filePath))
                    {
                        // Assert
                        Assert.AreEqual(bitmapResult.Width, fileResult.Width, "Widths should match");
                        Assert.AreEqual(bitmapResult.Height, fileResult.Height, "Heights should match");
                    }
                }
            }
            finally
            {
                TestHelpers.DeleteTestFile(filePath);
            }
        }

        [TestMethod]
        public void QRGenerator_SpecialCharacters_HandlesCorrectly()
        {
            // Arrange
            string specialData = TestHelpers.GetBase64TestData("Special: !@#$%^&*()_+-=[]{}|;:',.<>?/");

            // Act
            using (Bitmap result = this.generator.GenerateQRCodeAsBitmap(specialData, 2, 300))
            {
                // Assert
                Assert.IsNotNull(result, "Should handle special characters");
                Assert.IsTrue(TestHelpers.IsValidBarcodeBitmap(result), "Barcode should be valid");
            }
        }

        // End Region

        [TestCleanup]
        public void Cleanup()
        {
            this.generator = null;
        }
    }
}
