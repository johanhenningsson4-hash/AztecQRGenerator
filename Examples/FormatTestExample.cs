using System;
using System.Drawing;
using System.Drawing.Imaging;
using AztecQR;

namespace AztecQRFormatTest
{
    /// <summary>
    /// Example program demonstrating the new image format support
    /// Tests PNG, JPEG, and BMP output formats
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== AztecQRGenerator - Image Format Support Test ===\n");

            // Sample Base64 encoded data
            string testData = "SGVsbG8gV29ybGQh"; // "Hello World!" in Base64

            // Test QR Generator
            Console.WriteLine("Testing QR Code Generation...");
            TestQRGenerator(testData);

            Console.WriteLine("\n" + new string('-', 50) + "\n");

            // Test Aztec Generator
            Console.WriteLine("Testing Aztec Code Generation...");
            TestAztecGenerator(testData);

            Console.WriteLine("\n=== All Tests Complete ===");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static void TestQRGenerator(string data)
        {
            var generator = new QRGenerator();

            Console.WriteLine("\n1. Testing PNG Format (Recommended):");
            try
            {
                bool success = generator.GenerateQRCodeToFile(
                    qrstring: data,
                    lCorrection: 2,
                    lPixelDensity: 300,
                    filePath: "test_qr.png",
                    format: ImageFormat.Png
                );
                Console.WriteLine($"   Result: {(success ? "? SUCCESS" : "? FAILED")}");
                if (success)
                {
                    var fileInfo = new System.IO.FileInfo("test_qr.png");
                    Console.WriteLine($"   File Size: {fileInfo.Length:N0} bytes");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n2. Testing JPEG Format (Not Recommended):");
            try
            {
                bool success = generator.GenerateQRCodeToFile(
                    qrstring: data,
                    lCorrection: 2,
                    lPixelDensity: 300,
                    filePath: "test_qr.jpg",
                    format: ImageFormat.Jpeg
                );
                Console.WriteLine($"   Result: {(success ? "? SUCCESS" : "? FAILED")}");
                if (success)
                {
                    var fileInfo = new System.IO.FileInfo("test_qr.jpg");
                    Console.WriteLine($"   File Size: {fileInfo.Length:N0} bytes");
                    Console.WriteLine("   ? Warning: JPEG compression may affect scanability");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n3. Testing BMP Format (Large Files):");
            try
            {
                bool success = generator.GenerateQRCodeToFile(
                    qrstring: data,
                    lCorrection: 2,
                    lPixelDensity: 300,
                    filePath: "test_qr.bmp",
                    format: ImageFormat.Bmp
                );
                Console.WriteLine($"   Result: {(success ? "? SUCCESS" : "? FAILED")}");
                if (success)
                {
                    var fileInfo = new System.IO.FileInfo("test_qr.bmp");
                    Console.WriteLine($"   File Size: {fileInfo.Length:N0} bytes");
                    Console.WriteLine($"   Note: BMP files are much larger than PNG");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n4. Testing Bitmap Return Method:");
            try
            {
                using (Bitmap bitmap = generator.GenerateQRCodeAsBitmap(data, 2, 300))
                {
                    Console.WriteLine($"   Result: ? SUCCESS");
                    Console.WriteLine($"   Bitmap Size: {bitmap.Width}x{bitmap.Height}");
                    
                    // Save in all formats from single bitmap
                    bitmap.Save("test_qr_from_bitmap.png", ImageFormat.Png);
                    bitmap.Save("test_qr_from_bitmap.jpg", ImageFormat.Jpeg);
                    bitmap.Save("test_qr_from_bitmap.bmp", ImageFormat.Bmp);
                    Console.WriteLine($"   Saved bitmap in all 3 formats");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }
        }

        static void TestAztecGenerator(string data)
        {
            var generator = new AztecGenerator();

            Console.WriteLine("\n1. Testing PNG Format (Recommended):");
            try
            {
                bool success = generator.GenerateAztecCodeToFile(
                    aztecstring: data,
                    lCorrection: 2,
                    lPixelDensity: 300,
                    filePath: "test_aztec.png",
                    format: ImageFormat.Png
                );
                Console.WriteLine($"   Result: {(success ? "? SUCCESS" : "? FAILED")}");
                if (success)
                {
                    var fileInfo = new System.IO.FileInfo("test_aztec.png");
                    Console.WriteLine($"   File Size: {fileInfo.Length:N0} bytes");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n2. Testing JPEG Format (Not Recommended):");
            try
            {
                bool success = generator.GenerateAztecCodeToFile(
                    aztecstring: data,
                    lCorrection: 2,
                    lPixelDensity: 300,
                    filePath: "test_aztec.jpg",
                    format: ImageFormat.Jpeg
                );
                Console.WriteLine($"   Result: {(success ? "? SUCCESS" : "? FAILED")}");
                if (success)
                {
                    var fileInfo = new System.IO.FileInfo("test_aztec.jpg");
                    Console.WriteLine($"   File Size: {fileInfo.Length:N0} bytes");
                    Console.WriteLine("   ? Warning: JPEG compression may affect scanability");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n3. Testing BMP Format (Large Files):");
            try
            {
                bool success = generator.GenerateAztecCodeToFile(
                    aztecstring: data,
                    lCorrection: 2,
                    lPixelDensity: 300,
                    filePath: "test_aztec.bmp",
                    format: ImageFormat.Bmp
                );
                Console.WriteLine($"   Result: {(success ? "? SUCCESS" : "? FAILED")}");
                if (success)
                {
                    var fileInfo = new System.IO.FileInfo("test_aztec.bmp");
                    Console.WriteLine($"   File Size: {fileInfo.Length:N0} bytes");
                    Console.WriteLine($"   Note: BMP files are much larger than PNG");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n4. Testing Bitmap Return Method:");
            try
            {
                using (Bitmap bitmap = generator.GenerateAztecCodeAsBitmap(data, 2, 300))
                {
                    Console.WriteLine($"   Result: ? SUCCESS");
                    Console.WriteLine($"   Bitmap Size: {bitmap.Width}x{bitmap.Height}");
                    
                    // Save in all formats from single bitmap
                    bitmap.Save("test_aztec_from_bitmap.png", ImageFormat.Png);
                    bitmap.Save("test_aztec_from_bitmap.jpg", ImageFormat.Jpeg);
                    bitmap.Save("test_aztec_from_bitmap.bmp", ImageFormat.Bmp);
                    Console.WriteLine($"   Saved bitmap in all 3 formats");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }
        }

        static void CompareSizes()
        {
            Console.WriteLine("\n=== File Size Comparison ===\n");

            var files = new[]
            {
                "test_qr.png", "test_qr.jpg", "test_qr.bmp",
                "test_aztec.png", "test_aztec.jpg", "test_aztec.bmp"
            };

            Console.WriteLine($"{"File",-30} {"Size",10}");
            Console.WriteLine(new string('-', 45));

            foreach (var file in files)
            {
                if (System.IO.File.Exists(file))
                {
                    var info = new System.IO.FileInfo(file);
                    Console.WriteLine($"{file,-30} {info.Length,10:N0} bytes");
                }
            }
        }
    }
}
