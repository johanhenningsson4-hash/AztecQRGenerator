using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace AztecQR.Tests
{
    /// <summary>
    /// Helper utilities for unit tests
    /// </summary>
    public static class TestHelpers
    {
        private static readonly string TestOutputDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "AztecQRGenerator",
            "TestOutput"
        );

        /// <summary>
        /// Generates Base64 encoded test data from a string
        /// </summary>
        public static string GetBase64TestData(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }



        /// <summary>
        /// Gets the test output directory path
        /// </summary>
        public static string GetTestOutputDirectory()
        {
            if (!Directory.Exists(TestOutputDir))
            {
                Directory.CreateDirectory(TestOutputDir);
            }
            return TestOutputDir;
        }

        /// <summary>
        /// Generates a unique test file path
        /// </summary>
        public static string GetTestFilePath(string prefix, string extension)
        {
            string fileName = $"{prefix}_{DateTime.Now:yyyyMMddHHmmssfff}.{extension}";
            return Path.Combine(GetTestOutputDirectory(), fileName);
        }

        /// <summary>
        /// Cleans up test files older than 1 hour
        /// </summary>
        public static void CleanupOldTestFiles()
        {
            try
            {
                if (!Directory.Exists(TestOutputDir))
                {
                    return;
                }

                var cutoffTime = DateTime.Now.AddHours(-1);
                var files = Directory.GetFiles(TestOutputDir);

                foreach (var file in files)
                {
                    try
                    {
                        var fileInfo = new FileInfo(file);
                        if (fileInfo.CreationTime < cutoffTime)
                        {
                            File.Delete(file);
                        }
                    }
                    catch
                    {
                        // Ignore cleanup failures
                    }
                }
            }
            catch
            {
                // Ignore cleanup failures
            }
        }

        /// <summary>
        /// Verifies that a bitmap is valid QR/Aztec code
        /// </summary>
        public static bool IsValidBarcodeBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return false;
            }

            // Check dimensions
            if (bitmap.Width <= 0 || bitmap.Height <= 0)
            {
                return false;
            }

            // Check that it contains both black and white pixels
            bool hasBlack = false;
            bool hasWhite = false;

            // Sample a few pixels to verify it's not blank
            int sampleSize = Math.Min(10, bitmap.Width / 2);
            for (int i = 0; i < sampleSize && (!hasBlack || !hasWhite); i++)
            {
                int x = i * (bitmap.Width / sampleSize);
                int y = i * (bitmap.Height / sampleSize);

                if (x >= bitmap.Width) x = bitmap.Width - 1;
                if (y >= bitmap.Height) y = bitmap.Height - 1;

                Color pixel = bitmap.GetPixel(x, y);
                int brightness = (pixel.R + pixel.G + pixel.B) / 3;

                if (brightness < 128)
                {
                    hasBlack = true;
                }
                else
                {
                    hasWhite = true;
                }
            }

            return hasBlack && hasWhite;
        }

        /// <summary>
        /// Verifies that a file exists and has valid content
        /// </summary>
        public static bool IsValidImageFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            try
            {
                using (var image = Image.FromFile(filePath))
                {
                    return image.Width > 0 && image.Height > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a test file if it exists
        /// </summary>
        public static void DeleteTestFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
                // Ignore deletion failures
            }
        }
    }
}
