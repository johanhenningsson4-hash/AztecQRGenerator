using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AztecQR.Tests
{
    [TestClass]
    public class AsyncApiTests
    {
        private QRGenerator qrGenerator;
        private AztecGenerator aztecGenerator;
        private string testBase64Data;

        [TestInitialize]
        public void Setup()
        {
            this.qrGenerator = new QRGenerator();
            this.aztecGenerator = new AztecGenerator();
            this.testBase64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes("Test Data"));
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.qrGenerator = null;
            this.aztecGenerator = null;
        }

        // Region: QR Generator Async Tests

        [TestMethod]
        public async Task GenerateQRCodeAsBitmapAsync_ValidInput_ReturnsBitmap()
        {
            // Act
            using (Bitmap result = await this.qrGenerator.GenerateQRCodeAsBitmapAsync(this.testBase64Data, 2, 300))
            {
                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(300, result.Width);
                Assert.AreEqual(300, result.Height);
            }
        }

        [TestMethod]
        public async Task GenerateQRCodeAsBitmapAsync_NullInput_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await this.qrGenerator.GenerateQRCodeAsBitmapAsync(null, 2, 300));
        }

        [TestMethod]
        public async Task GenerateQRCodeAsBitmapAsync_EmptyInput_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await this.qrGenerator.GenerateQRCodeAsBitmapAsync("", 2, 300));
        }

        [TestMethod]
        public async Task GenerateQRCodeAsBitmapAsync_ZeroPixelDensity_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await this.qrGenerator.GenerateQRCodeAsBitmapAsync(this.testBase64Data, 2, 0));
        }

        [TestMethod]
        public async Task GenerateQRCodeAsBitmapAsync_NegativePixelDensity_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await this.qrGenerator.GenerateQRCodeAsBitmapAsync(this.testBase64Data, 2, -100));
        }

        [TestMethod]
        public async Task GenerateQRCodeAsBitmapAsync_WithCancellationToken_CanBeCancelled()
        {
            // Arrange
            using (var cts = new CancellationTokenSource())
            {
                cts.Cancel(); // Cancel immediately

                // Act & Assert
                await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () =>
                    await this.qrGenerator.GenerateQRCodeAsBitmapAsync(this.testBase64Data, 2, 300, cts.Token));
            }
        }

        [TestMethod]
        public async Task GenerateQRCodeToFileAsync_ValidInput_ReturnsTrue()
        {
            // Arrange
            string filePath = $"test_async_{DateTime.Now:yyyyMMddHHmmssfff}.png";

            try
            {
                // Act
                bool result = await this.qrGenerator.GenerateQRCodeToFileAsync(
                    this.testBase64Data, 2, 300, filePath, ImageFormat.Png);

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
        public async Task GenerateQRCodeToFileAsync_NullFilePath_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await this.qrGenerator.GenerateQRCodeToFileAsync(this.testBase64Data, 2, 300, null, ImageFormat.Png));
        }

        [TestMethod]
        public async Task GenerateQRCodeToFileAsync_NullFormat_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                await this.qrGenerator.GenerateQRCodeToFileAsync(this.testBase64Data, 2, 300, "test.png", null));
        }

        [TestMethod]
        public async Task GenerateQRBitmapAsync_ValidInput_ReturnsTrue()
        {
            // Act
            bool result = await this.qrGenerator.GenerateQRBitmapAsync(this.testBase64Data, 2, 300);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task GenerateBatchAsync_ValidRequests_ReturnsAllBitmaps()
        {
            // Arrange
            var requests = new[]
            {
                new QRRequest(this.testBase64Data, 2, 200),
                new QRRequest(Convert.ToBase64String(Encoding.UTF8.GetBytes("Test 2")), 2, 200),
                new QRRequest(Convert.ToBase64String(Encoding.UTF8.GetBytes("Test 3")), 2, 200)
            };

            int progressCallCount = 0;
            var progress = new Progress<BatchProgress>(p => progressCallCount++);

            // Act
            var results = await this.qrGenerator.GenerateBatchAsync(requests, progress);

            // Assert
            Assert.AreEqual(3, results.Count());
            Assert.IsTrue(progressCallCount > 0, "Progress should be reported");

            // Cleanup
            foreach (var bitmap in results)
            {
                bitmap?.Dispose();
            }
        }

        [TestMethod]
        public async Task GenerateBatchAsync_NullRequests_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                await this.qrGenerator.GenerateBatchAsync(null));
        }

        [TestMethod]
        public async Task GenerateBatchAsync_WithCancellation_ThrowsOperationCanceledException()
        {
            // Arrange
            var requests = new[]
            {
                new QRRequest(this.testBase64Data, 2, 200),
                new QRRequest(Convert.ToBase64String(Encoding.UTF8.GetBytes("Test 2")), 2, 200)
            };

            using (var cts = new CancellationTokenSource())
            {
                cts.Cancel(); // Cancel immediately

                // Act & Assert
                await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () =>
                    await this.qrGenerator.GenerateBatchAsync(requests, null, cts.Token));
            }
        }

        // Region: Aztec Generator Async Tests

        [TestMethod]
        public async Task GenerateAztecCodeAsBitmapAsync_ValidInput_ReturnsBitmap()
        {
            // Act
            using (Bitmap result = await this.aztecGenerator.GenerateAztecCodeAsBitmapAsync(this.testBase64Data, 2, 300))
            {
                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(300, result.Width);
                Assert.AreEqual(300, result.Height);
            }
        }

        [TestMethod]
        public async Task GenerateAztecCodeAsBitmapAsync_NullInput_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await this.aztecGenerator.GenerateAztecCodeAsBitmapAsync(null, 2, 300));
        }

        [TestMethod]
        public async Task GenerateAztecCodeAsBitmapAsync_ZeroPixelDensity_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await this.aztecGenerator.GenerateAztecCodeAsBitmapAsync(this.testBase64Data, 2, 0));
        }

        [TestMethod]
        public async Task GenerateAztecCodeAsBitmapAsync_WithCancellationToken_CanBeCancelled()
        {
            // Arrange
            using (var cts = new CancellationTokenSource())
            {
                cts.Cancel(); // Cancel immediately

                // Act & Assert
                await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () =>
                    await this.aztecGenerator.GenerateAztecCodeAsBitmapAsync(this.testBase64Data, 2, 300, cts.Token));
            }
        }

        [TestMethod]
        public async Task GenerateAztecCodeToFileAsync_ValidInput_ReturnsTrue()
        {
            // Arrange
            string filePath = $"test_aztec_async_{DateTime.Now:yyyyMMddHHmmssfff}.png";

            try
            {
                // Act
                bool result = await this.aztecGenerator.GenerateAztecCodeToFileAsync(
                    this.testBase64Data, 2, 300, filePath, ImageFormat.Png);

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
        public async Task GenerateAztecBitmapAsync_ValidInput_ReturnsTrue()
        {
            // Act
            bool result = await this.aztecGenerator.GenerateAztecBitmapAsync(this.testBase64Data, 2, 300);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task AztecGenerateBatchAsync_ValidRequests_ReturnsAllBitmaps()
        {
            // Arrange
            var requests = new[]
            {
                new AztecRequest(this.testBase64Data, 2, 200),
                new AztecRequest(Convert.ToBase64String(Encoding.UTF8.GetBytes("Aztec Test 2")), 2, 200)
            };

            int progressCallCount = 0;
            var progress = new Progress<BatchProgress>(p => progressCallCount++);

            // Act
            var results = await this.aztecGenerator.GenerateBatchAsync(requests, progress);

            // Assert
            Assert.AreEqual(2, results.Count());
            Assert.IsTrue(progressCallCount > 0, "Progress should be reported");

            // Cleanup
            foreach (var bitmap in results)
            {
                bitmap?.Dispose();
            }
        }

        // Region: Performance and Integration Tests

        [TestMethod]
        public async Task AsyncVsSyncPerformance_CompareExecution()
        {
            // This test demonstrates that async versions don't add significant overhead
            // for single operations, but provide better scalability for multiple operations

            var sw = System.Diagnostics.Stopwatch.StartNew();

            // Sync version
            using (var syncBitmap = this.qrGenerator.GenerateQRCodeAsBitmap(this.testBase64Data, 2, 300))
            {
                // Just ensure it works
                Assert.IsNotNull(syncBitmap);
            }
            var syncTime = sw.ElapsedMilliseconds;

            sw.Restart();

            // Async version
            using (var asyncBitmap = await this.qrGenerator.GenerateQRCodeAsBitmapAsync(this.testBase64Data, 2, 300))
            {
                // Just ensure it works
                Assert.IsNotNull(asyncBitmap);
            }
            var asyncTime = sw.ElapsedMilliseconds;

            // Both should complete in reasonable time (async might be slightly slower due to Task overhead)
            Assert.IsTrue(syncTime < 5000, "Sync version should complete quickly");
            Assert.IsTrue(asyncTime < 5000, "Async version should complete quickly");
        }

        [TestMethod]
        public async Task ParallelAsyncOperations_ExecuteConcurrently()
        {
            // Arrange
            var tasks = new Task<Bitmap>[3];

            // Act - Start multiple async operations in parallel
            for (int i = 0; i < tasks.Length; i++)
            {
                string data = Convert.ToBase64String(Encoding.UTF8.GetBytes($"Parallel test {i}"));
                tasks[i] = this.qrGenerator.GenerateQRCodeAsBitmapAsync(data, 2, 200);
            }

            // Wait for all to complete
            var results = await Task.WhenAll(tasks);

            // Assert
            Assert.AreEqual(3, results.Length);
            foreach (var bitmap in results)
            {
                Assert.IsNotNull(bitmap);
                bitmap.Dispose();
            }
        }

        [TestMethod]
        public async Task MixedQRAndAztecParallel_BothTypesWorkTogether()
        {
            // Act - Generate QR and Aztec codes in parallel
            var qrTask = this.qrGenerator.GenerateQRCodeAsBitmapAsync(this.testBase64Data, 2, 300);
            var aztecTask = this.aztecGenerator.GenerateAztecCodeAsBitmapAsync(this.testBase64Data, 2, 300);

            var results = await Task.WhenAll(qrTask, aztecTask);

            // Assert
            using (var qrBitmap = results[0])
            using (var aztecBitmap = results[1])
            {
                Assert.IsNotNull(qrBitmap);
                Assert.IsNotNull(aztecBitmap);
                Assert.AreEqual(300, qrBitmap.Width);
                Assert.AreEqual(300, aztecBitmap.Width);
            }
        }

        [TestMethod]
        public async Task TimeoutScenario_HandlesLongRunningOperations()
        {
            // Arrange - Use a larger size that takes more time to generate
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
            {
                // Act
                using (var bitmap = await this.qrGenerator.GenerateQRCodeAsBitmapAsync(this.testBase64Data, 2, 1000, cts.Token))
                {
                    // Assert
                    Assert.IsNotNull(bitmap);
                    Assert.AreEqual(1000, bitmap.Width);
                }
            }
        }

        // Region: Supporting Classes Tests

        [TestMethod]
        public void QRRequest_Constructor_SetsPropertiesCorrectly()
        {
            // Act
            var request = new QRRequest("testdata", 3, 400);

            // Assert
            Assert.AreEqual("testdata", request.Data);
            Assert.AreEqual(3, request.ErrorCorrection);
            Assert.AreEqual(400, request.PixelDensity);
        }

        [TestMethod]
        public void QRRequest_DefaultConstructor_SetsDefaultValues()
        {
            // Act
            var request = new QRRequest();

            // Assert
            Assert.AreEqual(2, request.ErrorCorrection);
            Assert.AreEqual(300, request.PixelDensity);
        }

        [TestMethod]
        public void AztecRequest_Constructor_SetsPropertiesCorrectly()
        {
            // Act
            var request = new AztecRequest("aztecdata", 1, 500);

            // Assert
            Assert.AreEqual("aztecdata", request.Data);
            Assert.AreEqual(1, request.ErrorCorrection);
            Assert.AreEqual(500, request.PixelDensity);
        }

        [TestMethod]
        public void BatchProgress_CalculatesPercentageCorrectly()
        {
            // Act
            var progress = new BatchProgress(3, 10);

            // Assert
            Assert.AreEqual(3, progress.CompletedCount);
            Assert.AreEqual(10, progress.TotalCount);
            Assert.AreEqual(30.0, progress.PercentComplete, 0.01);
        }

        [TestMethod]
        public void BatchProgress_ToString_ReturnsFormattedString()
        {
            // Act
            var progress = new BatchProgress(7, 20);
            string result = progress.ToString();

            // Assert
            Assert.AreEqual("7/20 (35.0%)", result);
        }

        [TestMethod]
        public void BatchProgress_ZeroTotal_HandlesCorrectly()
        {
            // Act
            var progress = new BatchProgress(0, 0);

            // Assert
            Assert.AreEqual(0.0, progress.PercentComplete);
        }
    }
}
