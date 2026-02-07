using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AztecQR;
using System.Linq;

namespace AztecQR.Examples
{
    /// <summary>
    /// Demonstrates usage of the new flexible input APIs in AztecQRGenerator.
    /// </summary>
    public class FlexibleInputExamples
    {
        private static readonly QRGenerator qrGenerator = new QRGenerator();

        /// <summary>
        /// Example of generating QR codes from plain text strings.
        /// </summary>
        public static async Task TextInputExamples()
        {
            Console.WriteLine("=== Text Input Examples ===");
            
            try
            {
                // Simple text
                string message = "Hello, World!";
                
                // Sync version
                using (Bitmap qr = qrGenerator.GenerateQRCodeFromText(message))
                {
                    Console.WriteLine($"✓ Generated QR from text (sync): {qr.Width}x{qr.Height}");
                }
                
                // Async version
                using (Bitmap qr = await qrGenerator.GenerateQRCodeFromTextAsync(message))
                {
                    Console.WriteLine($"✓ Generated QR from text (async): {qr.Width}x{qr.Height}");
                }

                // With custom encoding
                string unicodeMessage = "Hello 世界! 🚀";
                using (Bitmap qr = qrGenerator.GenerateQRCodeFromText(unicodeMessage, encoding: Encoding.UTF8))
                {
                    Console.WriteLine($"✓ Generated QR from Unicode text: {qr.Width}x{qr.Height}");
                }

                // Direct to file
                bool success = await qrGenerator.GenerateQRCodeFromTextToFileAsync(
                    message, "text_example.png", ImageFormat.Png);
                Console.WriteLine($"✓ Saved text QR to file: {success}");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Example of generating QR codes from byte arrays.
        /// </summary>
        public static async Task ByteArrayExamples()
        {
            Console.WriteLine("\n=== Byte Array Examples ===");
            
            try
            {
                // Binary data
                byte[] binaryData = { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x42, 0x79, 0x74, 0x65, 0x73 }; // "Hello Bytes"
                
                // Sync version
                using (Bitmap qr = qrGenerator.GenerateQRCodeFromBytes(binaryData))
                {
                    Console.WriteLine($"✓ Generated QR from bytes (sync): {qr.Width}x{qr.Height}");
                }
                
                // Async version  
                using (Bitmap qr = await qrGenerator.GenerateQRCodeFromBytesAsync(binaryData))
                {
                    Console.WriteLine($"✓ Generated QR from bytes (async): {qr.Width}x{qr.Height}");
                }

                // Save to file
                bool success = await qrGenerator.GenerateQRCodeFromBytesToFileAsync(
                    binaryData, "bytes_example.png", ImageFormat.Png);
                Console.WriteLine($"✓ Saved bytes QR to file: {success}");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Example comparing different input methods for the same data.
        /// </summary>
        public static async Task ComparisonExample()
        {
            Console.WriteLine("\n=== Input Method Comparison ===");
            
            try
            {
                string originalText = "Hello, flexible input!";
                
                // Method 1: Direct text (new)
                using (Bitmap qr1 = qrGenerator.GenerateQRCodeFromText(originalText))
                {
                    Console.WriteLine($"✓ Method 1 - Direct text: {qr1.Width}x{qr1.Height}");
                }
                
                // Method 2: Byte array (new)
                byte[] textBytes = Encoding.UTF8.GetBytes(originalText);
                using (Bitmap qr2 = qrGenerator.GenerateQRCodeFromBytes(textBytes))
                {
                    Console.WriteLine($"✓ Method 2 - Byte array: {qr2.Width}x{qr2.Height}");
                }
                
                // Method 3: Base64 (original)
                string base64Text = Convert.ToBase64String(textBytes);
                using (Bitmap qr3 = qrGenerator.GenerateQRCodeAsBitmap(base64Text, 2, 300))
                {
                    Console.WriteLine($"✓ Method 3 - Base64 (original): {qr3.Width}x{qr3.Height}");
                }

                Console.WriteLine("All methods should produce identical QR codes!");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Example of batch processing with different input types.
        /// </summary>
        public static async Task MixedBatchExample()
        {
            Console.WriteLine("\n=== Mixed Batch Processing ===");
            
            try
            {
                var tasks = new List<Task<Bitmap>>();
                
                // Mix of different input types
                tasks.Add(qrGenerator.GenerateQRCodeFromTextAsync("Text message 1"));
                tasks.Add(qrGenerator.GenerateQRCodeFromTextAsync("Text message 2"));
                tasks.Add(qrGenerator.GenerateQRCodeFromBytesAsync(Encoding.UTF8.GetBytes("Bytes message")));

                // Wait for all to complete
                var results = await Task.WhenAll(tasks);
                
                Console.WriteLine($"✓ Generated {results.Length} QR codes from mixed inputs");
                
                // Cleanup
                foreach (var bitmap in results)
                {
                    bitmap?.Dispose();
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Example of advanced usage patterns.
        /// </summary>
        public static async Task AdvancedExamples()
        {
            Console.WriteLine("\n=== Advanced Usage Patterns ===");
            
            try
            {
                // Different encodings
                string multilingual = "English, Español, 中文, العربية, Русский";
                
                using (var qr1 = qrGenerator.GenerateQRCodeFromText(multilingual, encoding: Encoding.UTF8))
                using (var qr2 = qrGenerator.GenerateQRCodeFromText(multilingual, encoding: Encoding.Unicode))
                {
                    Console.WriteLine($"✓ UTF-8 encoding: {qr1.Width}x{qr1.Height}");
                    Console.WriteLine($"✓ Unicode encoding: {qr2.Width}x{qr2.Height}");
                }

                // Custom error correction and size
                using (var qr = qrGenerator.GenerateQRCodeFromText(
                    "High quality QR code", lCorrection: 3, lPixelDensity: 500))
                {
                    Console.WriteLine($"✓ High-quality QR: {qr.Width}x{qr.Height}");
                }

                // With cancellation
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    using (var qr = await qrGenerator.GenerateQRCodeFromTextAsync(
                        "Cancellable operation", cancellationToken: cts.Token))
                    {
                        Console.WriteLine($"✓ Generated with cancellation support: {qr.Width}x{qr.Height}");
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Main entry point demonstrating all flexible input examples.
        /// </summary>
        public static async Task Main(string[] args)
        {
            Console.WriteLine("AztecQRGenerator Flexible Input Examples");
            Console.WriteLine("========================================");

            try
            {
                await TextInputExamples();
                await ByteArrayExamples();
                await ComparisonExample();
                await MixedBatchExample();
                await AdvancedExamples();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Unexpected error: {ex}");
            }

            Console.WriteLine("\n=== All Examples Completed ===");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
