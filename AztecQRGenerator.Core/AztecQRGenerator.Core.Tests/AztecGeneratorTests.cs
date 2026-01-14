using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZXing;
using ZXing.Aztec;

namespace AztecQR.Tests
{
    [TestClass]
    public class AztecGeneratorTests
    {
        private AztecGenerator generator;
        private string testBase64Data;

        [TestInitialize]
        public void Setup()
        {
            generator = new AztecGenerator();
            testBase64Data = TestHelpers.GetBase64TestData("Hello World!");
            TestHelpers.CleanupOldTestFiles();
        }

        #region GenerateAztecCodeAsBitmap Tests

        [TestMethod]
        public void GenerateAztecCodeAsBitmap_ValidInput_ReturnsBitmap()
        {
            // Act
            Bitmap result = generator.GenerateAztecCodeAsBitmap(testBase64Data, 2, 300);

            // Assert
            Assert.IsNotNull(result, "Bitmap should not be null");
            Assert.AreEqual(300, result.Width, "Width should match pixel density");
            Assert.AreEqual(300, result.Height, "Height should match pixel density");
            Assert.IsTrue(TestHelpers.IsValidBarcodeBitmap(result), "Bitmap should contain valid barcode pattern");

            result.Dispose();
        }

        [TestMethod]
        public void GenerateAztecCodeAsBitmap_DifferentSizes_GeneratesCorrectDimensions()
        {
            // Arrange
            int[] sizes = { 100, 200, 500 };

            foreach (int size in sizes)
            {
                // Act
                using (Bitmap result = generator.GenerateAztecCodeAsBitmap(testBase64Data, 2, size))
                {
                    // Assert
                    Assert.AreEqual(size, result.Width, $"Width should be {size}");
                    Assert.AreEqual(size, result.Height, $"Height should be {size}");
                }
            }
        }

        [TestMethod]
        public void GenerateAztecCodeAsBitmap_DifferentCorrectionLevels_Succeeds()
        {
            // Arrange
            int[] correctionLevels = { 0, 2, 5, 10 };

            foreach (int level in correctionLevels)
            {
                // Act
                using (Bitmap result = generator.GenerateAztecCodeAsBitmap(testBase64Data, level, 200))
                {
                    // Assert
                    Assert.IsNotNull(result, $"Bitmap should be generated with correction level {level}");
                    Assert.IsTrue(TestHelpers.IsValidBarcodeBitmap(result), "Bitmap should contain valid barcode pattern");
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateAztecCodeAsBitmap_NullString_ThrowsArgumentException()
        {
            // Act
            generator.GenerateAztecCodeAsBitmap(null, 2, 300);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateAztecCodeAsBitmap_EmptyString_ThrowsArgumentException()
        {
            // Act
            generator.GenerateAztecCodeAsBitmap("", 2, 300);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateAztecCodeAsBitmap_WhitespaceString_ThrowsArgumentException()
        {
            // Act
            generator.GenerateAztecCodeAsBitmap("   ", 2, 300);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateAztecCodeAsBitmap_InvalidBase64_ThrowsArgumentException()
        {
            // Act
            generator.GenerateAztecCodeAsBitmap("NotValidBase64!@#", 2, 300);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateAztecCodeAsBitmap_ZeroPixelDensity_ThrowsArgumentException()
        {
            // Act
            generator.GenerateAztecCodeAsBitmap(testBase64Data, 2, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateAztecCodeAsBitmap_NegativePixelDensity_ThrowsArgumentException()
        {
            // Act
            generator.GenerateAztecCodeAsBitmap(testBase64Data, 2, -100);
        }

        [TestMethod]
        public void GenerateAztecCodeAsBitmap_NegativeCorrectionLevel_UsesDefault()
        {
            // Act - Should not throw, should use default value
            using (Bitmap result = generator.GenerateAztecCodeAsBitmap(testBase64Data, -1, 200))
            {
                // Assert
                Assert.IsNotNull(result, "Bitmap should be generated with default correction level");
            }
        }

        [TestMethod]
        public void GenerateAztecCodeAsBitmap_LargeData_Succeeds()
        {
            // Arrange
            string largeData = TestHelpers.GetBase64TestData(new string('A', 1000));

            // Act
            using (Bitmap result = generator.GenerateAztecCodeAsBitmap(largeData, 2, 400))
            {
                // Assert
                Assert.IsNotNull(result, "Should handle large data");
                Assert.IsTrue(TestHelpers.IsValidBarcodeBitmap(result), "Large data bitmap should be valid");
            }
        }

        #endregion

        #region GenerateAztecCodeToFile Tests

        [TestMethod]
        public void GenerateAztecCodeToFile_PNG_CreatesValidFile()
        {
            // Arrange
            string filePath = TestHelpers.GetTestFilePath("aztec_test", "png");

            try
            {
                // Act
                bool result = generator.GenerateAztecCodeToFile(testBase64Data, 2, 300, filePath, ImageFormat.Png);

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
        public void GenerateAztecCodeToFile_JPEG_CreatesValidFile()
        {
            // Arrange
            string filePath = TestHelpers.GetTestFilePath("aztec_test", "jpg");

            try
            {
                // Act
                bool result = generator.GenerateAztecCodeToFile(testBase64Data, 2, 300, filePath, ImageFormat.Jpeg);

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
        public void GenerateAztecCodeToFile_BMP_CreatesValidFile()
        {
            // Arrange
            string filePath = TestHelpers.GetTestFilePath("aztec_test", "bmp");

            try
            {
                // Act
                bool result = generator.GenerateAztecCodeToFile(testBase64Data, 2, 300, filePath, ImageFormat.Bmp);

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
        public void GenerateAztecCodeToFile_NullFilePath_ThrowsArgumentException()
        {
            // Act
            generator.GenerateAztecCodeToFile(testBase64Data, 2, 300, null, ImageFormat.Png);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateAztecCodeToFile_EmptyFilePath_ThrowsArgumentException()
        {
            // Act
            generator.GenerateAztecCodeToFile(testBase64Data, 2, 300, "", ImageFormat.Png);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GenerateAztecCodeToFile_NullFormat_ThrowsArgumentNullException()
        {
            // Arrange
            string filePath = TestHelpers.GetTestFilePath("aztec_test", "png");

            try
            {
                // Act
                generator.GenerateAztecCodeToFile(testBase64Data, 2, 300, filePath, null);
            }
            finally
            {
                TestHelpers.DeleteTestFile(filePath);
            }
        }

        [TestMethod]
        public void GenerateAztecCodeToFile_RelativePath_UsesDocumentsFolder()
        {
            // Arrange
            string relativePath = "test_aztec.png";

            try
            {
                // Act
                bool result = generator.GenerateAztecCodeToFile(testBase64Data, 2, 300, relativePath, ImageFormat.Png);

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

        #endregion

        #region GenerateAztecBitmap Tests (Legacy Method)

        [TestMethod]
        public void GenerateAztecBitmap_ValidInput_CreatesTimestampedFiles()
        {
            // Act
            bool result = generator.GenerateAztecBitmap(testBase64Data, 2, 300);

            // Assert
            Assert.IsTrue(result, "Method should return true");
            // Note: Files are created in Documents folder with timestamps
            // We can't easily verify them without knowing exact timestamp
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateAztecBitmap_InvalidData_ThrowsException()
        {
            // Act
            generator.GenerateAztecBitmap("InvalidBase64", 2, 300);
        }

        #endregion

        #region Integration Tests

        [TestMethod]
        public void AztecGenerator_MultipleCalls_AllSucceed()
        {
            // Act & Assert - Generate multiple Aztec codes in succession
            for (int i = 0; i < 5; i++)
            {
                string data = TestHelpers.GetBase64TestData($"Test {i}");
                using (Bitmap result = generator.GenerateAztecCodeAsBitmap(data, 2, 200))
                {
                    Assert.IsNotNull(result, $"Iteration {i} should succeed");
                }
            }
        }

        [TestMethod]
        public void AztecGenerator_BitmapAndFile_ProduceSameSize()
        {
            // Arrange
            string filePath = TestHelpers.GetTestFilePath("aztec_comparison", "png");

            try
            {
                // Act
                using (Bitmap bitmapResult = generator.GenerateAztecCodeAsBitmap(testBase64Data, 2, 300))
                {
                    generator.GenerateAztecCodeToFile(testBase64Data, 2, 300, filePath, ImageFormat.Png);

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
        public void AztecGenerator_SpecialCharacters_HandlesCorrectly()
        {
            // Arrange
            string specialData = TestHelpers.GetBase64TestData("Special: !@#$%^&*()_+-=[]{}|;:',.<>?/");

            // Act
            using (Bitmap result = generator.GenerateAztecCodeAsBitmap(specialData, 2, 300))
            {
                // Assert
                Assert.IsNotNull(result, "Should handle special characters");
                Assert.IsTrue(TestHelpers.IsValidBarcodeBitmap(result), "Barcode should be valid");
            }
        }

        [TestMethod]
        public void AztecGenerator_VersusQRGenerator_BothWork()
        {
            // Arrange
            var qrGenerator = new QRGenerator();

            // Act
            using (Bitmap aztecResult = generator.GenerateAztecCodeAsBitmap(testBase64Data, 2, 300))
            using (Bitmap qrResult = qrGenerator.GenerateQRCodeAsBitmap(testBase64Data, 2, 300))
            {
                // Assert
                Assert.IsNotNull(aztecResult, "Aztec code should be generated");
                Assert.IsNotNull(qrResult, "QR code should be generated");
                Assert.IsTrue(TestHelpers.IsValidBarcodeBitmap(aztecResult), "Aztec code should be valid");
                Assert.IsTrue(TestHelpers.IsValidBarcodeBitmap(qrResult), "QR code should be valid");
            }
        }

        #endregion

        [TestCleanup]
        public void Cleanup()
        {
            generator = null;
        }
    }
}
