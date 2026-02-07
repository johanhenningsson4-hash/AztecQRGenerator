using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AztecQR;

namespace AztecQR.Performance
{
    /// <summary>
    /// Performance benchmarking and optimization examples for AztecQRGenerator.
    /// </summary>
    public class PerformanceBenchmarks
    {
        private static readonly QRGenerator qrGenerator = new QRGenerator();

        /// <summary>
        /// Benchmarks QR code generation performance with different scenarios.
        /// </summary>
        public static async Task RunPerformanceBenchmarks()
        {
            Console.WriteLine("AztecQRGenerator Performance Benchmarks");
            Console.WriteLine("======================================");

            await BenchmarkBasicGeneration();
            await BenchmarkCachingBenefits();
            await BenchmarkBatchProcessing();
            await BenchmarkMemoryUsage();
            BenchmarkCacheStatistics();
        }

        /// <summary>
        /// Benchmarks basic QR code generation performance.
        /// </summary>
        private static async Task BenchmarkBasicGeneration()
        {
            Console.WriteLine("\n=== Basic Generation Performance ===");

            var testData = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Performance test data"));
            var sizes = new[] { 200, 300, 500, 1000 };
            var iterations = 100;

            foreach (var size in sizes)
            {
                var stopwatch = Stopwatch.StartNew();
                
                for (int i = 0; i < iterations; i++)
                {
                    using (var qr = qrGenerator.GenerateQRCodeAsBitmap(testData, 2, size))
                    {
                        // Just generate and dispose
                    }
                }
                
                stopwatch.Stop();
                var avgMs = stopwatch.ElapsedMilliseconds / (double)iterations;
                Console.WriteLine($"Size {size}x{size}: {avgMs:F2} ms average ({iterations} iterations)");
            }

            // Test async performance
            Console.WriteLine("\nAsync Performance:");
            var asyncStopwatch = Stopwatch.StartNew();
            
            var tasks = new List<Task<Bitmap>>();
            for (int i = 0; i < 50; i++)
            {
                tasks.Add(qrGenerator.GenerateQRCodeAsBitmapAsync(testData, 2, 300));
            }
            
            var results = await Task.WhenAll(tasks);
            asyncStopwatch.Stop();
            
            foreach (var result in results)
            {
                result.Dispose();
            }
            
            Console.WriteLine($"50 concurrent generations: {asyncStopwatch.ElapsedMilliseconds} ms total");
        }

        /// <summary>
        /// Benchmarks the caching system benefits.
        /// </summary>
        private static async Task BenchmarkCachingBenefits()
        {
            Console.WriteLine("\n=== Caching Performance ===");

            var testData = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Cache test data"));
            var iterations = 100;

            // Clear cache to start fresh
            QRGenerator.ClearCache();

            // First run - no cache
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                using (var qr = qrGenerator.GenerateQRCodeAsBitmap(testData, 2, 300))
                {
                    // Generate and dispose
                }
            }
            stopwatch.Stop();
            var noCacheTime = stopwatch.ElapsedMilliseconds;

            Console.WriteLine($"No cache: {noCacheTime} ms ({iterations} iterations)");

            // Second run - with cache hits
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                using (var qr = qrGenerator.GenerateQRCodeAsBitmap(testData, 2, 300))
                {
                    // Should hit cache after first generation
                }
            }
            stopwatch.Stop();
            var cacheTime = stopwatch.ElapsedMilliseconds;

            Console.WriteLine($"With cache: {cacheTime} ms ({iterations} iterations)");
            
            var speedup = (double)noCacheTime / cacheTime;
            Console.WriteLine($"Cache speedup: {speedup:F2}x faster");
        }

        /// <summary>
        /// Benchmarks batch processing performance.
        /// </summary>
        private static async Task BenchmarkBatchProcessing()
        {
            Console.WriteLine("\n=== Batch Processing Performance ===");

            // Create test requests
            var requests = new List<QRRequest>();
            for (int i = 0; i < 20; i++)
            {
                var data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"Batch item {i}"));
                requests.Add(new QRRequest(data, 2, 300));
            }

            var progress = new Progress<BatchProgress>(p => 
                Console.WriteLine($"  Progress: {p.CompletedCount}/{p.TotalCount} ({p.PercentComplete:F1}%)"));

            var stopwatch = Stopwatch.StartNew();
            var results = await qrGenerator.GenerateBatchAsync(requests, progress);
            stopwatch.Stop();

            Console.WriteLine($"Batch generation: {stopwatch.ElapsedMilliseconds} ms for {requests.Count} items");
            Console.WriteLine($"Average per item: {stopwatch.ElapsedMilliseconds / (double)requests.Count:F2} ms");

            // Cleanup
            foreach (var result in results)
            {
                result.Dispose();
            }
        }

        /// <summary>
        /// Benchmarks memory usage patterns.
        /// </summary>
        private static async Task BenchmarkMemoryUsage()
        {
            Console.WriteLine("\n=== Memory Usage Analysis ===");

            var initialMemory = GC.GetTotalMemory(true);
            Console.WriteLine($"Initial memory: {initialMemory / 1024:N0} KB");

            // Generate many QR codes to test memory handling
            var testData = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Memory test"));
            var generated = 0;

            for (int i = 0; i < 500; i++)
            {
                using (var qr = qrGenerator.GenerateQRCodeAsBitmap(testData, 2, 300))
                {
                    generated++;
                    
                    // Sample memory usage every 100 generations
                    if (i % 100 == 0)
                    {
                        var currentMemory = GC.GetTotalMemory(false);
                        var memoryIncrease = currentMemory - initialMemory;
                        Console.WriteLine($"After {generated} generations: +{memoryIncrease / 1024:N0} KB");
                    }
                }
            }

            // Force garbage collection and measure final memory
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var finalMemory = GC.GetTotalMemory(true);
            var totalIncrease = finalMemory - initialMemory;
            Console.WriteLine($"Final memory after GC: +{totalIncrease / 1024:N0} KB increase");
            Console.WriteLine($"Memory per generation: {totalIncrease / (double)generated:F2} bytes average");
        }

        /// <summary>
        /// Displays cache statistics and performance metrics.
        /// </summary>
        private static void BenchmarkCacheStatistics()
        {
            Console.WriteLine("\n=== Cache Statistics ===");

            var stats = QRGenerator.GetCacheStatistics();
            Console.WriteLine($"Total cache entries: {stats.TotalEntries}");
            Console.WriteLine($"Valid entries: {stats.ValidEntries}");
            Console.WriteLine($"Expired entries: {stats.ExpiredEntries}");
            Console.WriteLine($"Cache utilization: {stats.UtilizationPercentage:F1}%");
            Console.WriteLine($"Hit ratio: {stats.CacheHitRatio:P1}");
            Console.WriteLine($"Max cache size: {stats.MaxCacheSize}");
        }

        /// <summary>
        /// Tests performance with different input types.
        /// </summary>
        public static async Task BenchmarkInputTypes()
        {
            Console.WriteLine("\n=== Input Type Performance Comparison ===");

            var testText = "Performance comparison test data with some length to it";
            var testBytes = System.Text.Encoding.UTF8.GetBytes(testText);
            var testBase64 = Convert.ToBase64String(testBytes);
            
            var iterations = 100;

            // Benchmark text input
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                using (var qr = qrGenerator.GenerateQRCodeFromText(testText))
                {
                    // Generate and dispose
                }
            }
            stopwatch.Stop();
            var textTime = stopwatch.ElapsedMilliseconds;

            // Benchmark byte array input
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                using (var qr = qrGenerator.GenerateQRCodeFromBytes(testBytes))
                {
                    // Generate and dispose
                }
            }
            stopwatch.Stop();
            var bytesTime = stopwatch.ElapsedMilliseconds;

            // Benchmark Base64 input
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                using (var qr = qrGenerator.GenerateQRCodeAsBitmap(testBase64, 2, 300))
                {
                    // Generate and dispose
                }
            }
            stopwatch.Stop();
            var base64Time = stopwatch.ElapsedMilliseconds;

            Console.WriteLine($"Text input:   {textTime} ms ({iterations} iterations)");
            Console.WriteLine($"Bytes input:  {bytesTime} ms ({iterations} iterations)");
            Console.WriteLine($"Base64 input: {base64Time} ms ({iterations} iterations)");
        }

        /// <summary>
        /// Runs a comprehensive performance stress test.
        /// </summary>
        public static async Task RunStressTest()
        {
            Console.WriteLine("\n=== Stress Test ===");

            var random = new Random();
            var testData = new List<string>();
            
            // Generate varied test data
            for (int i = 0; i < 1000; i++)
            {
                var length = random.Next(10, 200);
                var text = new string(Enumerable.Range(0, length).Select(x => (char)random.Next(32, 126)).ToArray());
                testData.Add(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(text)));
            }

            var stopwatch = Stopwatch.StartNew();
            var tasks = new List<Task>();
            var completedCount = 0;

            foreach (var data in testData)
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        using (var qr = await qrGenerator.GenerateQRCodeAsBitmapAsync(data, 2, 300))
                        {
                            Interlocked.Increment(ref completedCount);
                            
                            if (completedCount % 100 == 0)
                            {
                                Console.WriteLine($"Completed: {completedCount}/1000");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }));
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();

            Console.WriteLine($"Stress test completed: {stopwatch.ElapsedMilliseconds} ms for 1000 varied QR codes");
            Console.WriteLine($"Average: {stopwatch.ElapsedMilliseconds / 1000.0:F2} ms per QR code");
            Console.WriteLine($"Throughput: {1000.0 / (stopwatch.ElapsedMilliseconds / 1000.0):F0} QR codes/second");
        }

        /// <summary>
        /// Main entry point for running all performance benchmarks.
        /// </summary>
        public static async Task Main(string[] args)
        {
            try
            {
                await RunPerformanceBenchmarks();
                await BenchmarkInputTypes();
                await RunStressTest();
                
                Console.WriteLine("\n=== Final Cache Statistics ===");
                var finalStats = QRGenerator.GetCacheStatistics();
                Console.WriteLine(finalStats.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Benchmark error: {ex}");
            }
            finally
            {
                // Clean up cache
                QRGenerator.ClearCache();
                Console.WriteLine("\nCache cleared. Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
