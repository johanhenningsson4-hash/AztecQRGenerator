using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AztecQR;

namespace AztecQR.Examples
{
    /// <summary>
    /// Demonstrates usage of the new async APIs in AztecQRGenerator.
    /// </summary>
    public class AsyncApiExamples
    {
        private static readonly QRGenerator qrGenerator = new QRGenerator();
        private static readonly AztecGenerator aztecGenerator = new AztecGenerator();

        /// <summary>
        /// Example of basic async QR code generation.
        /// </summary>
        public static async Task BasicAsyncQRExample()
        {
            Console.WriteLine("=== Basic Async QR Code Generation ===");
            
            try
            {
                // Convert text to Base64 as required by the API
                string text = "Hello, Async World!";
                string base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));

                // Generate QR code asynchronously
                using (Bitmap qrCode = await qrGenerator.GenerateQRCodeAsBitmapAsync(base64Data, 2, 300))
                {
                    Console.WriteLine($"✓ Generated QR code: {qrCode.Width}x{qrCode.Height}");
                    
                    // Save to file asynchronously
                    bool success = await qrGenerator.GenerateQRCodeToFileAsync(
                        base64Data, 2, 300, "async_qr.png", ImageFormat.Png);
                    
                    Console.WriteLine($"✓ Saved to file: {success}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Example of async QR code generation with cancellation support.
        /// </summary>
        public static async Task CancellationExample()
        {
            Console.WriteLine("\n=== Async QR Code with Cancellation ===");
            
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                try
                {
                    string text = "This operation can be cancelled";
                    string base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));

                    // Generate with cancellation token
                    using (Bitmap qrCode = await qrGenerator.GenerateQRCodeAsBitmapAsync(
                        base64Data, 2, 500, cts.Token))
                    {
                        Console.WriteLine("✓ QR code generated successfully (not cancelled)");
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("⚠ Operation was cancelled");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Example of batch processing with progress reporting.
        /// </summary>
        public static async Task BatchProcessingExample()
        {
            Console.WriteLine("\n=== Batch QR Code Generation with Progress ===");

            // Create multiple requests
            var requests = new List<QRRequest>();
            for (int i = 1; i <= 5; i++)
            {
                string text = $"Batch item {i}";
                string base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
                requests.Add(new QRRequest(base64Data, 2, 200));
            }

            // Progress reporter
            var progress = new Progress<BatchProgress>(p =>
                Console.WriteLine($"  Progress: {p}"));

            try
            {
                using (var cts = new CancellationTokenSource())
                {
                    // Generate batch with progress reporting
                    var bitmaps = await qrGenerator.GenerateBatchAsync(requests, progress, cts.Token);
                    
                    Console.WriteLine($"✓ Generated {bitmaps.Count()} QR codes");
                    
                    // Dispose all bitmaps
                    foreach (var bitmap in bitmaps)
                    {
                        bitmap?.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Batch error: {ex.Message}");
            }
        }

        /// <summary>
        /// Example of async Aztec code generation.
        /// </summary>
        public static async Task AsyncAztecExample()
        {
            Console.WriteLine("\n=== Async Aztec Code Generation ===");
            
            try
            {
                string text = "Aztec codes are cool!";
                string base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));

                // Generate Aztec code asynchronously
                using (Bitmap aztecCode = await aztecGenerator.GenerateAztecCodeAsBitmapAsync(base64Data, 2, 300))
                {
                    Console.WriteLine($"✓ Generated Aztec code: {aztecCode.Width}x{aztecCode.Height}");
                    
                    // Save with timestamp
                    bool success = await aztecGenerator.GenerateAztecBitmapAsync(base64Data, 2, 300);
                    Console.WriteLine($"✓ Saved timestamped file: {success}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Example showing parallel generation of QR and Aztec codes.
        /// </summary>
        public static async Task ParallelGenerationExample()
        {
            Console.WriteLine("\n=== Parallel QR and Aztec Generation ===");
            
            try
            {
                string text = "Parallel processing!";
                string base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));

                // Start both operations in parallel
                var qrTask = qrGenerator.GenerateQRCodeAsBitmapAsync(base64Data, 2, 300);
                var aztecTask = aztecGenerator.GenerateAztecCodeAsBitmapAsync(base64Data, 2, 300);

                // Wait for both to complete
                var results = await Task.WhenAll(qrTask, aztecTask);
                
                using (Bitmap qrCode = results[0])
                using (Bitmap aztecCode = results[1])
                {
                    Console.WriteLine($"✓ QR Code: {qrCode.Width}x{qrCode.Height}");
                    Console.WriteLine($"✓ Aztec Code: {aztecCode.Width}x{aztecCode.Height}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Example of robust error handling with async operations.
        /// </summary>
        public static async Task ErrorHandlingExample()
        {
            Console.WriteLine("\n=== Error Handling Examples ===");

            // Test with invalid input
            try
            {
                await qrGenerator.GenerateQRCodeAsBitmapAsync("", 2, 300);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"✓ Caught expected argument error: {ex.Message}");
            }

            // Test with invalid pixel density
            try
            {
                string base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes("test"));
                await qrGenerator.GenerateQRCodeAsBitmapAsync(base64Data, 2, -100);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"✓ Caught expected pixel density error: {ex.Message}");
            }

            // Test with cancellation
            try
            {
                using (var cts = new CancellationTokenSource())
                {
                    cts.Cancel(); // Cancel immediately
                    
                    string base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes("test"));
                    await qrGenerator.GenerateQRCodeAsBitmapAsync(base64Data, 2, 300, cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("✓ Caught expected cancellation");
            }
        }

        /// <summary>
        /// Advanced example showing timeout handling and retry logic.
        /// </summary>
        public static async Task AdvancedAsyncPatternsExample()
        {
            Console.WriteLine("\n=== Advanced Async Patterns ===");

            string text = "Advanced async patterns";
            string base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));

            // Example 1: Timeout handling
            try
            {
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    var bitmap = await qrGenerator.GenerateQRCodeAsBitmapAsync(base64Data, 2, 1000, cts.Token);
                    bitmap.Dispose();
                    Console.WriteLine("✓ Generated within timeout");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("⚠ Operation timed out");
            }

            // Example 2: Simple retry logic
            int maxRetries = 3;
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var bitmap = await qrGenerator.GenerateQRCodeAsBitmapAsync(base64Data, 2, 300);
                    bitmap.Dispose();
                    Console.WriteLine($"✓ Generated on attempt {attempt}");
                    break;
                }
                catch (Exception ex) when (attempt < maxRetries)
                {
                    Console.WriteLine($"⚠ Attempt {attempt} failed: {ex.Message}, retrying...");
                    await Task.Delay(100); // Brief delay before retry
                }
            }
        }

        /// <summary>
        /// Main entry point demonstrating all async examples.
        /// </summary>
        public static async Task Main(string[] args)
        {
            Console.WriteLine("AztecQRGenerator Async API Examples");
            Console.WriteLine("===================================");

            try
            {
                await BasicAsyncQRExample();
                await CancellationExample();
                await BatchProcessingExample();
                await AsyncAztecExample();
                await ParallelGenerationExample();
                await ErrorHandlingExample();
                await AdvancedAsyncPatternsExample();
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
