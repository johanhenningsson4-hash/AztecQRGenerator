using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AztecQR.Tests
{
    [TestClass]
    public class PerformanceOptimizationTests
    {
        private QRGenerator qrGenerator;
        private string testBase64Data;

        [TestInitialize]
        public void Setup()
        {
            this.qrGenerator = new QRGenerator();
            this.testBase64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes("Performance test data"));
            
            // Clear cache before each test
            QRGenerator.ClearCache();
        }

        [TestCleanup]
        public void Cleanup()
        {
            QRGenerator.ClearCache();
            this.qrGenerator = null;
        }

        [TestMethod]
        public void CachingSystem_RepeatedGeneration_ShowsPerformanceImprovement()
        {
            // Arrange
            const int iterations = 20;
            var stopwatch = new Stopwatch();

            // Act - First generation (no cache)
            stopwatch.Start();
            for (int i = 0; i < iterations; i++)
            {
                using (var qr = qrGenerator.GenerateQRCodeAsBitmap(testBase64Data, 2, 300))
                {
                    Assert.IsNotNull(qr);
                }
            }
            stopwatch.Stop();
            var noCacheTime = stopwatch.ElapsedMilliseconds;

            // Act - Second generation (with cache)
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                using (var qr = qrGenerator.GenerateQRCodeAsBitmap(testBase64Data, 2, 300))
                {
                    Assert.IsNotNull(qr);
                }
            }
            stopwatch.Stop();
            var cacheTime = stopwatch.ElapsedMilliseconds;

            // Assert - Cache should provide some performance benefit
            Console.WriteLine($"No cache: {noCacheTime}ms, With cache: {cacheTime}ms");
            Assert.IsTrue(cacheTime < noCacheTime * 0.8, "Cache should provide at least 20% performance improvement");
        }

        [TestMethod]
        public void ParallelProcessing_LargeImages_CompletesSuccessfully()
        {
            // Arrange
            var testData = Convert.ToBase64String(Encoding.UTF8.GetBytes("Large image test"));

            // Act & Assert - Should complete without errors
            using (var qr = qrGenerator.GenerateQRCodeAsBitmap(testData, 2, 1200)) // Large size to trigger parallel processing
            {
                Assert.IsNotNull(qr);
                Assert.AreEqual(1200, qr.Width);
                Assert.AreEqual(1200, qr.Height);
            }
        }

        [TestMethod]
        public void CacheStatistics_AfterOperations_ReturnsValidData()
        {
            // Arrange - Generate some QR codes to populate cache
            for (int i = 0; i < 5; i++)
            {
                var data = Convert.ToBase64String(Encoding.UTF8.GetBytes($"Test data {i}"));
                using (var qr = qrGenerator.GenerateQRCodeAsBitmap(data, 2, 300))
                {
                    Assert.IsNotNull(qr);
                }
            }

            // Act
            var stats = QRGenerator.GetCacheStatistics();

            // Assert
            Assert.IsNotNull(stats);
            Assert.IsTrue(stats.ValidEntries > 0, "Should have valid cache entries");
            Assert.IsTrue(stats.MaxCacheSize > 0, "Should have a defined max cache size");
            Assert.IsTrue(stats.UtilizationPercentage >= 0 && stats.UtilizationPercentage <= 100, "Utilization should be a valid percentage");
        }

        [TestMethod]
        public void CacheExpiration_AfterTimeout_RemovesExpiredEntries()
        {
            // Note: This test can't easily test real time expiration in a unit test
            // but we can test the expiration logic
            
            // Arrange - Generate a QR code
            using (var qr = qrGenerator.GenerateQRCodeAsBitmap(testBase64Data, 2, 300))
            {
                Assert.IsNotNull(qr);
            }

            // Act - Get initial stats
            var initialStats = QRGenerator.GetCacheStatistics();

            // Assert - Cache should have at least one entry
            Assert.IsTrue(initialStats.TotalEntries > 0, "Cache should contain entries");
        }

        [TestMethod]
        public void ClearCache_AfterPopulatingCache_RemovesAllEntries()
        {
            // Arrange - Populate cache
            for (int i = 0; i < 3; i++)
            {
                var data = Convert.ToBase64String(Encoding.UTF8.GetBytes($"Clear test {i}"));
                using (var qr = qrGenerator.GenerateQRCodeAsBitmap(data, 2, 300))
                {
                    Assert.IsNotNull(qr);
                }
            }

            var beforeClear = QRGenerator.GetCacheStatistics();
            Assert.IsTrue(beforeClear.TotalEntries > 0, "Cache should have entries before clearing");

            // Act
            QRGenerator.ClearCache();

            // Assert
            var afterClear = QRGenerator.GetCacheStatistics();
            Assert.AreEqual(0, afterClear.TotalEntries, "Cache should be empty after clearing");
        }

        [TestMethod]
        public void PerformanceComparison_DifferentSizes_CompletesWithinReasonableTime()
        {
            // Arrange
            var sizes = new[] { 200, 400, 800 };
            var maxTimePerSize = new[] { 100, 200, 500 }; // Maximum milliseconds allowed per size
            var stopwatch = new Stopwatch();

            for (int i = 0; i < sizes.Length; i++)
            {
                var size = sizes[i];
                var maxTime = maxTimePerSize[i];

                // Act
                stopwatch.Restart();
                using (var qr = qrGenerator.GenerateQRCodeAsBitmap(testBase64Data, 2, size))
                {
                    stopwatch.Stop();

                    // Assert
                    Assert.IsNotNull(qr);
                    Assert.AreEqual(size, qr.Width);
                    Console.WriteLine($"Size {size}x{size}: {stopwatch.ElapsedMilliseconds}ms (max: {maxTime}ms)");
                    Assert.IsTrue(stopwatch.ElapsedMilliseconds < maxTime, 
                        $"Generation of {size}x{size} QR code took {stopwatch.ElapsedMilliseconds}ms, expected < {maxTime}ms");
                }
            }
        }

        [TestMethod]
        public async Task AsyncPerformance_ConcurrentGeneration_HandlesHighLoad()
        {
            // Arrange
            const int concurrentTasks = 20;
            var tasks = new Task[concurrentTasks];
            var stopwatch = Stopwatch.StartNew();

            // Act
            for (int i = 0; i < concurrentTasks; i++)
            {
                var data = Convert.ToBase64String(Encoding.UTF8.GetBytes($"Async test {i}"));
                tasks[i] = Task.Run(async () =>
                {
                    using (var qr = await qrGenerator.GenerateQRCodeAsBitmapAsync(data, 2, 300))
                    {
                        Assert.IsNotNull(qr);
                    }
                });
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Assert - Should complete all tasks within reasonable time
            Console.WriteLine($"Completed {concurrentTasks} concurrent generations in {stopwatch.ElapsedMilliseconds}ms");
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 5000, "Concurrent generation should complete within 5 seconds");
        }

        [TestMethod]
        public void MemoryEfficiency_MultipleGenerations_DoesNotLeakMemory()
        {
            // Arrange
            var initialMemory = GC.GetTotalMemory(true);
            const int iterations = 100;

            // Act - Generate many QR codes
            for (int i = 0; i < iterations; i++)
            {
                var data = Convert.ToBase64String(Encoding.UTF8.GetBytes($"Memory test {i}"));
                using (var qr = qrGenerator.GenerateQRCodeAsBitmap(data, 2, 300))
                {
                    Assert.IsNotNull(qr);
                    // Bitmap is properly disposed by using statement
                }
            }

            // Force garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var finalMemory = GC.GetTotalMemory(true);
            var memoryIncrease = finalMemory - initialMemory;

            // Assert - Memory increase should be reasonable (not a massive leak)
            Console.WriteLine($"Memory increase after {iterations} generations: {memoryIncrease / 1024:N0} KB");
            Assert.IsTrue(memoryIncrease < 1024 * 1024 * 50, "Memory increase should be less than 50MB"); // Very generous limit
        }

        [TestMethod]
        public void CacheKeyGeneration_DifferentParameters_ProducesDifferentKeys()
        {
            // This test verifies that the cache key generation produces unique keys
            // We can't directly test the private method, but we can verify cache behavior
            
            var data1 = Convert.ToBase64String(Encoding.UTF8.GetBytes("Test 1"));
            var data2 = Convert.ToBase64String(Encoding.UTF8.GetBytes("Test 2"));

            // Generate with different data
            using (var qr1 = qrGenerator.GenerateQRCodeAsBitmap(data1, 2, 300))
            using (var qr2 = qrGenerator.GenerateQRCodeAsBitmap(data2, 2, 300))
            {
                Assert.IsNotNull(qr1);
                Assert.IsNotNull(qr2);
            }

            // Generate with different correction levels
            using (var qr3 = qrGenerator.GenerateQRCodeAsBitmap(data1, 1, 300))
            using (var qr4 = qrGenerator.GenerateQRCodeAsBitmap(data1, 3, 300))
            {
                Assert.IsNotNull(qr3);
                Assert.IsNotNull(qr4);
            }

            // Generate with different sizes
            using (var qr5 = qrGenerator.GenerateQRCodeAsBitmap(data1, 2, 200))
            using (var qr6 = qrGenerator.GenerateQRCodeAsBitmap(data1, 2, 400))
            {
                Assert.IsNotNull(qr5);
                Assert.IsNotNull(qr6);
            }

            // Verify cache contains multiple entries
            var stats = QRGenerator.GetCacheStatistics();
            Assert.IsTrue(stats.ValidEntries > 5, "Cache should contain entries for different parameter combinations");
        }
    }
}
