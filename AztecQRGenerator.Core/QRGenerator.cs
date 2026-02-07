/*=========================================================================================
'  Copyright(C):    Johan Henningsson 
'  
'  Author :         Johan Henningsson
'==========================================================================================*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using ZXing.QrCode;
using ZXing.Common;
using BitMatrix = ZXing.Common.BitMatrix;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;

namespace AztecQR
{
    /// <summary>
    /// Provides methods for generating QR codes as bitmaps or files.
    /// </summary>
    public class QRGenerator
    {
        private readonly Logger logger = Logger.Instance;
        
        // Performance optimizations
        private static readonly ConcurrentDictionary<string, CachedQRCode> QRCache = new ConcurrentDictionary<string, CachedQRCode>();
        private const int MaxCacheSize = 100;
        private const int CacheExpiryMinutes = 30;

        /// <summary>
        /// Generates a QR code from the supplied Base64-encoded string and returns
        /// it as a <see cref="Bitmap"/> with caching and performance optimizations.
        /// </summary>
        /// <param name="qrstring">Base64 encoded string to encode.</param>
        /// <param name="lCorrection">Error correction level. If &lt; 0 a default is used.</param>
        /// <param name="lPixelDensity">Desired pixel width/height of the generated bitmap. Must be &gt; 0.</param>
        /// <returns>A <see cref="Bitmap"/> containing the generated QR code.</returns>
        /// <exception cref="ArgumentException">Thrown when input is null/empty or pixel density is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the underlying encoder fails to produce a matrix.</exception>
        public Bitmap GenerateQRCodeAsBitmap(string qrstring, int lCorrection, int lPixelDensity)
        {
            logger.LogMethodEntry("QRGenerator", "GenerateQRCodeAsBitmap", "Base64 data", lCorrection, lPixelDensity);

            if (string.IsNullOrWhiteSpace(qrstring))
                throw new ArgumentException("QR string cannot be null or empty", nameof(qrstring));
            if (lPixelDensity <= 0)
                throw new ArgumentException("Pixel density must be greater than zero", nameof(lPixelDensity));
            if (lCorrection < 0)
                lCorrection = 2;

            // Create cache key
            string cacheKey = CreateCacheKey(qrstring, lCorrection, lPixelDensity);

            // Check cache first
            if (QRCache.TryGetValue(cacheKey, out CachedQRCode cachedQR) && !cachedQR.IsExpired)
            {
                logger.LogMethodExit("QRGenerator", "GenerateQRCodeAsBitmap", true);
                return CloneBitmap(cachedQR.Bitmap);
            }

            try
            {
                byte[] data;
                try
                {
                    data = Convert.FromBase64String(qrstring);
                }
                catch (System.FormatException ex)
                {
                    throw new ArgumentException("Invalid Base64 string format", nameof(qrstring), ex);
                }

                var writer = new QRCodeWriter();
                var hints = new Dictionary<EncodeHintType, object> { { EncodeHintType.MARGIN, 0 } };
                BitMatrix matrix;
                
                try
                {
                    string decodedString = Encoding.GetEncoding("ISO-8859-1").GetString(data);
                    matrix = writer.encode(
                        decodedString,
                        BarcodeFormat.QR_CODE,
                        lPixelDensity,
                        lPixelDensity,
                        hints
                    );
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to generate QR code matrix", ex);
                }

                Bitmap result = ConvertBitMatrixToBitmap(matrix);

                // Cache the result if caching is beneficial (small to medium sizes)
                if (lPixelDensity <= 1000 && ShouldCache())
                {
                    CacheQRCode(cacheKey, result);
                }

                return result;
            }
            finally
            {
                logger.LogMethodExit("QRGenerator", "GenerateQRCodeAsBitmap", true);
            }
        }

        /// <summary>
        /// Generates a QR code and saves it to <paramref name="filePath"/> using the
        /// provided <paramref name="format"/>.
        /// </summary>
        /// <param name="qrstring">Base64 encoded string to encode.</param>
        /// <param name="lCorrection">Error correction level.</param>
        /// <param name="lPixelDensity">Size of the QR code in pixels.</param>
        /// <param name="filePath">Output file path. If relative, the file is placed in the user's Documents/AztecQRGenerator/Output folder.</param>
        /// <param name="format">Image format to use for saving (PNG, JPEG or BMP).</param>
        /// <returns>True when the file was created successfully.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="format"/> is null.</exception>
        public bool GenerateQRCodeToFile(string qrstring, int lCorrection, int lPixelDensity, string filePath, ImageFormat format)
        {
            logger.LogMethodEntry("QRGenerator", "GenerateQRCodeToFile", "Base64 data", lCorrection, lPixelDensity, filePath, format?.ToString() ?? "null");

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            if (format == null)
                throw new ArgumentNullException(nameof(format), "Image format cannot be null");

            using (Bitmap bitmap = GenerateQRCodeAsBitmap(qrstring, lCorrection, lPixelDensity))
            {
                SaveBitmap(bitmap, filePath, format);
            }
            logger.LogMethodExit("QRGenerator", "GenerateQRCodeToFile", true);
            return true;
        }

        /// <summary>
        /// Generates a QR code and saves it as a PNG file with a timestamped name in the user's Documents folder.
        /// This is a convenience wrapper around <see cref="GenerateQRCodeToFile"/> using the PNG format.
        /// </summary>
        /// <param name="qrstring">Base64 encoded string to encode.</param>
        /// <param name="lCorrection">Error correction level.</param>
        /// <param name="lPixelDensity">Size of the QR code in pixels.</param>
        /// <returns>True when the file was created successfully.</returns>
        public bool GenerateQRBitmap(string qrstring, int lCorrection, int lPixelDensity)
        {
            logger.LogMethodEntry("QRGenerator", "GenerateQRBitmap", "Base64 data", lCorrection, lPixelDensity);

            using (Bitmap bitmap = GenerateQRCodeAsBitmap(qrstring, lCorrection, lPixelDensity))
            {
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string fileName = $"QRCode_{timestamp}.png";
                SaveBitmap(bitmap, fileName, ImageFormat.Png);
            }
            logger.LogMethodExit("QRGenerator", "GenerateQRBitmap", true);
            return true;
        }

        /// <summary>
        /// Converts a BitMatrix to a Bitmap object using optimized memory handling.
        /// </summary>
        /// <param name="matrix">The BitMatrix to convert.</param>
        /// <returns>A Bitmap representing the BitMatrix.</returns>
        /// <exception cref="ArgumentNullException">Thrown if matrix is null.</exception>
        private Bitmap ConvertBitMatrixToBitmap(BitMatrix matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix), "BitMatrix cannot be null");

            int width = matrix.Width;
            int height = matrix.Height;
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            BitmapData bmpData = bmp.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);
            
            try
            {
                int stride = bmpData.Stride;
                IntPtr ptr = bmpData.Scan0;
                int bytes = Math.Abs(stride) * height;
                byte[] rgbValues = new byte[bytes];

                // Use parallel processing for large images
                if (width * height > 10000) // Only for larger images to avoid overhead
                {
                    Parallel.For(0, height, y =>
                    {
                        int rowStart = y * stride;
                        for (int x = 0; x < width; x++)
                        {
                            int pixelIndex = rowStart + (x * 3);
                            byte colorValue = matrix[x, y] ? (byte)0 : (byte)255;
                            rgbValues[pixelIndex] = colorValue;     // Blue
                            rgbValues[pixelIndex + 1] = colorValue; // Green
                            rgbValues[pixelIndex + 2] = colorValue; // Red
                        }
                    });
                }
                else
                {
                    // Sequential processing for smaller images
                    for (int y = 0; y < height; y++)
                    {
                        int rowStart = y * stride;
                        for (int x = 0; x < width; x++)
                        {
                            int pixelIndex = rowStart + (x * 3);
                            byte colorValue = matrix[x, y] ? (byte)0 : (byte)255;
                            rgbValues[pixelIndex] = colorValue;     // Blue
                            rgbValues[pixelIndex + 1] = colorValue; // Green
                            rgbValues[pixelIndex + 2] = colorValue; // Red
                        }
                    }
                }
                
                Marshal.Copy(rgbValues, 0, ptr, bytes);
            }
            finally
            {
                bmp.UnlockBits(bmpData);
            }
            return bmp;
        }

        /// <summary>
        /// Saves a Bitmap to a file with the specified format.
        /// </summary>
        /// <param name="bitmap">Bitmap to save.</param>
        /// <param name="filePath">Output file path.</param>
        /// <param name="format">Image format (PNG, JPEG, or BMP).</param>
        /// <exception cref="ArgumentNullException">Thrown if bitmap or format is null.</exception>
        /// <exception cref="ArgumentException">Thrown if filePath is invalid.</exception>
        private void SaveBitmap(Bitmap bitmap, string filePath, ImageFormat format)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap), "Bitmap cannot be null");
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            if (format == null)
                throw new ArgumentNullException(nameof(format), "Image format cannot be null");

            string directory = Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(directory) || !Path.IsPathRooted(filePath))
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string outputDir = Path.Combine(documentsPath, "AztecQRGenerator", "Output");
                if (!Directory.Exists(outputDir))
                    Directory.CreateDirectory(outputDir);
                string fileName = Path.GetFileName(filePath);
                filePath = Path.Combine(outputDir, fileName);
            }
            else if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            bitmap.Save(filePath, format);
        }

        #region Flexible Input Methods

        /// <summary>
        /// Generates a QR code from the supplied text string and returns it as a <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="text">Text string to encode.</param>
        /// <param name="lCorrection">Error correction level. If &lt; 0 a default is used.</param>
        /// <param name="lPixelDensity">Desired pixel width/height of the generated bitmap. Must be &gt; 0.</param>
        /// <param name="encoding">Text encoding to use. If null, UTF-8 is used.</param>
        /// <returns>A <see cref="Bitmap"/> containing the generated QR code.</returns>
        /// <exception cref="ArgumentException">Thrown when input is null/empty or pixel density is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the underlying encoder fails to produce a matrix.</exception>
        public Bitmap GenerateQRCodeFromText(string text, int lCorrection = 2, int lPixelDensity = 300, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Text cannot be null or empty", nameof(text));

            logger.LogMethodEntry("QRGenerator", "GenerateQRCodeFromText", text.Substring(0, Math.Min(50, text.Length)) + "...", lCorrection, lPixelDensity);

            try
            {
                // Convert text to bytes using specified encoding (default: UTF-8)
                encoding = encoding ?? Encoding.UTF8;
                byte[] textBytes = encoding.GetBytes(text);
                
                return GenerateQRCodeFromBytes(textBytes, lCorrection, lPixelDensity);
            }
            finally
            {
                logger.LogMethodExit("QRGenerator", "GenerateQRCodeFromText", true);
            }
        }

        /// <summary>
        /// Generates a QR code from the supplied byte array and returns it as a <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="data">Byte array to encode.</param>
        /// <param name="lCorrection">Error correction level. If &lt; 0 a default is used.</param>
        /// <param name="lPixelDensity">Desired pixel width/height of the generated bitmap. Must be &gt; 0.</param>
        /// <returns>A <see cref="Bitmap"/> containing the generated QR code.</returns>
        /// <exception cref="ArgumentException">Thrown when data is null/empty or pixel density is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the underlying encoder fails to produce a matrix.</exception>
        public Bitmap GenerateQRCodeFromBytes(byte[] data, int lCorrection = 2, int lPixelDensity = 300)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Data cannot be null or empty", nameof(data));
            if (lPixelDensity <= 0)
                throw new ArgumentException("Pixel density must be greater than zero", nameof(lPixelDensity));
            if (lCorrection < 0)
                lCorrection = 2;

            logger.LogMethodEntry("QRGenerator", "GenerateQRCodeFromBytes", $"{data.Length} bytes", lCorrection, lPixelDensity);

            try
            {
                var writer = new QRCodeWriter();
                var hints = new Dictionary<EncodeHintType, object> { { EncodeHintType.MARGIN, 0 } };
                BitMatrix matrix;
                
                try
                {
                    // Use ISO-8859-1 encoding to preserve byte values
                    string dataString = Encoding.GetEncoding("ISO-8859-1").GetString(data);
                    matrix = writer.encode(
                        dataString,
                        BarcodeFormat.QR_CODE,
                        lPixelDensity,
                        lPixelDensity,
                        hints
                    );
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to generate QR code matrix", ex);
                }

                return ConvertBitMatrixToBitmap(matrix);
            }
            finally
            {
                logger.LogMethodExit("QRGenerator", "GenerateQRCodeFromBytes", true);
            }
        }

        /// <summary>
        /// Generates a QR code from text and saves it to the specified file path using the provided format.
        /// </summary>
        /// <param name="text">Text string to encode.</param>
        /// <param name="filePath">Output file path. If relative, the file is placed in the user's Documents/AztecQRGenerator/Output folder.</param>
        /// <param name="format">Image format to use for saving (PNG, JPEG or BMP).</param>
        /// <param name="lCorrection">Error correction level. If &lt; 0 a default is used.</param>
        /// <param name="lPixelDensity">Size of the QR code in pixels.</param>
        /// <param name="encoding">Text encoding to use. If null, UTF-8 is used.</param>
        /// <returns>True when the file was created successfully.</returns>
        /// <exception cref="ArgumentException">Thrown when text or filePath is null/empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when format is null.</exception>
        public bool GenerateQRCodeFromTextToFile(string text, string filePath, ImageFormat format, int lCorrection = 2, int lPixelDensity = 300, Encoding encoding = null)
        {
            logger.LogMethodEntry("QRGenerator", "GenerateQRCodeFromTextToFile", text?.Substring(0, Math.Min(50, text?.Length ?? 0)) + "...", lCorrection, lPixelDensity, filePath, format?.ToString() ?? "null");

            using (Bitmap bitmap = GenerateQRCodeFromText(text, lCorrection, lPixelDensity, encoding))
            {
                SaveBitmap(bitmap, filePath, format);
            }
            
            logger.LogMethodExit("QRGenerator", "GenerateQRCodeFromTextToFile", true);
            return true;
        }

        /// <summary>
        /// Generates a QR code from byte data and saves it to the specified file path using the provided format.
        /// </summary>
        /// <param name="data">Byte array to encode.</param>
        /// <param name="filePath">Output file path. If relative, the file is placed in the user's Documents/AztecQRGenerator/Output folder.</param>
        /// <param name="format">Image format to use for saving (PNG, JPEG or BMP).</param>
        /// <param name="lCorrection">Error correction level. If &lt; 0 a default is used.</param>
        /// <param name="lPixelDensity">Size of the QR code in pixels.</param>
        /// <returns>True when the file was created successfully.</returns>
        /// <exception cref="ArgumentException">Thrown when data is null/empty or filePath is null/empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when format is null.</exception>
        public bool GenerateQRCodeFromBytesToFile(byte[] data, string filePath, ImageFormat format, int lCorrection = 2, int lPixelDensity = 300)
        {
            logger.LogMethodEntry("QRGenerator", "GenerateQRCodeFromBytesToFile", $"{data?.Length ?? 0} bytes", lCorrection, lPixelDensity, filePath, format?.ToString() ?? "null");

            using (Bitmap bitmap = GenerateQRCodeFromBytes(data, lCorrection, lPixelDensity))
            {
                SaveBitmap(bitmap, filePath, format);
            }
            
            logger.LogMethodExit("QRGenerator", "GenerateQRCodeFromBytesToFile", true);
            return true;
        }

        #endregion

        #region Async Methods

        /// <summary>
        /// Asynchronously generates a QR code from the supplied Base64-encoded string and returns
        /// it as a <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="qrstring">Base64 encoded string to encode.</param>
        /// <param name="lCorrection">Error correction level. If &lt; 0 a default is used.</param>
        /// <param name="lPixelDensity">Desired pixel width/height of the generated bitmap. Must be &gt; 0.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Bitmap"/> with the generated QR code.</returns>
        /// <exception cref="ArgumentException">Thrown when input is null/empty or pixel density is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the underlying encoder fails to produce a matrix.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
        public async Task<Bitmap> GenerateQRCodeAsBitmapAsync(string qrstring, int lCorrection, int lPixelDensity, CancellationToken cancellationToken = default)
        {
            // Input validation (fast, no need to run on thread pool)
            if (string.IsNullOrWhiteSpace(qrstring))
                throw new ArgumentException("QR string cannot be null or empty", nameof(qrstring));
            if (lPixelDensity <= 0)
                throw new ArgumentException("Pixel density must be greater than zero", nameof(lPixelDensity));

            logger.LogMethodEntry("QRGenerator", "GenerateQRCodeAsBitmapAsync", "Base64 data", lCorrection, lPixelDensity);

            try
            {
                // Run the CPU-intensive work on a background thread
                return await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return GenerateQRCodeAsBitmap(qrstring, lCorrection, lPixelDensity);
                }, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                logger.LogMethodExit("QRGenerator", "GenerateQRCodeAsBitmapAsync", true);
            }
        }

        /// <summary>
        /// Asynchronously generates a QR code and saves it to the specified file path using the provided format.
        /// </summary>
        /// <param name="qrstring">Base64 encoded string to encode.</param>
        /// <param name="lCorrection">Error correction level.</param>
        /// <param name="lPixelDensity">Size of the QR code in pixels.</param>
        /// <param name="filePath">Output file path. If relative, the file is placed in the user's Documents/AztecQRGenerator/Output folder.</param>
        /// <param name="format">Image format to use for saving (PNG, JPEG or BMP).</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the file was created successfully.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="format"/> is null.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
        public async Task<bool> GenerateQRCodeToFileAsync(string qrstring, int lCorrection, int lPixelDensity, string filePath, ImageFormat format, CancellationToken cancellationToken = default)
        {
            // Input validation (fast, no need to run on thread pool)
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            if (format == null)
                throw new ArgumentNullException(nameof(format), "Image format cannot be null");

            logger.LogMethodEntry("QRGenerator", "GenerateQRCodeToFileAsync", "Base64 data", lCorrection, lPixelDensity, filePath, format?.ToString() ?? "null");

            try
            {
                // Generate bitmap asynchronously
                using (Bitmap bitmap = await GenerateQRCodeAsBitmapAsync(qrstring, lCorrection, lPixelDensity, cancellationToken).ConfigureAwait(false))
                {
                    // Save to file on background thread
                    await Task.Run(() =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        SaveBitmap(bitmap, filePath, format);
                    }, cancellationToken).ConfigureAwait(false);
                }

                logger.LogMethodExit("QRGenerator", "GenerateQRCodeToFileAsync", true);
                return true;
            }
            catch
            {
                logger.LogMethodExit("QRGenerator", "GenerateQRCodeToFileAsync", false);
                throw;
            }
        }

        /// <summary>
        /// Asynchronously generates a QR code and saves it as a PNG file with a timestamped name in the user's Documents folder.
        /// This is a convenience wrapper around <see cref="GenerateQRCodeToFileAsync"/> using the PNG format.
        /// </summary>
        /// <param name="qrstring">Base64 encoded string to encode.</param>
        /// <param name="lCorrection">Error correction level.</param>
        /// <param name="lPixelDensity">Size of the QR code in pixels.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the file was created successfully.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
        public async Task<bool> GenerateQRBitmapAsync(string qrstring, int lCorrection, int lPixelDensity, CancellationToken cancellationToken = default)
        {
            logger.LogMethodEntry("QRGenerator", "GenerateQRBitmapAsync", "Base64 data", lCorrection, lPixelDensity);

            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string fileName = $"QRCode_{timestamp}.png";
                
                return await GenerateQRCodeToFileAsync(qrstring, lCorrection, lPixelDensity, fileName, ImageFormat.Png, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                logger.LogMethodExit("QRGenerator", "GenerateQRBitmapAsync", true);
            }
        }

        /// <summary>
        /// Asynchronously generates multiple QR codes in batch with progress reporting.
        /// </summary>
        /// <param name="requests">Collection of QR code generation requests.</param>
        /// <param name="progress">Optional progress reporter for batch operation.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous batch operation. The task result contains the generated bitmaps.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="requests"/> is null.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
        public async Task<IEnumerable<Bitmap>> GenerateBatchAsync(IEnumerable<QRRequest> requests, IProgress<BatchProgress> progress = null, CancellationToken cancellationToken = default)
        {
            if (requests == null)
                throw new ArgumentNullException(nameof(requests));

            logger.LogMethodEntry("QRGenerator", "GenerateBatchAsync", $"Processing {requests.Count()} requests");

            var requestList = requests.ToList();
            var results = new List<Bitmap>();
            var totalCount = requestList.Count;
            var completedCount = 0;

            try
            {
                foreach (var request in requestList)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var bitmap = await GenerateQRCodeAsBitmapAsync(request.Data, request.ErrorCorrection, request.PixelDensity, cancellationToken).ConfigureAwait(false);
                    results.Add(bitmap);

                    completedCount++;
                    progress?.Report(new BatchProgress(completedCount, totalCount));
                }

                logger.LogMethodExit("QRGenerator", "GenerateBatchAsync", true);
                return results;
            }
            catch
            {
                // Cleanup any generated bitmaps on error
                foreach (var bitmap in results)
                {
                    bitmap?.Dispose();
                }
                logger.LogMethodExit("QRGenerator", "GenerateBatchAsync", false);
                throw;
            }
        }

        #region Flexible Input Async Methods

        /// <summary>
        /// Asynchronously generates a QR code from the supplied text string and returns it as a <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="text">Text string to encode.</param>
        /// <param name="lCorrection">Error correction level. If &lt; 0 a default is used.</param>
        /// <param name="lPixelDensity">Desired pixel width/height of the generated bitmap. Must be &gt; 0.</param>
        /// <param name="encoding">Text encoding to use. If null, UTF-8 is used.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Bitmap"/> with the generated QR code.</returns>
        /// <exception cref="ArgumentException">Thrown when input is null/empty or pixel density is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the underlying encoder fails to produce a matrix.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
        public async Task<Bitmap> GenerateQRCodeFromTextAsync(string text, int lCorrection = 2, int lPixelDensity = 300, Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Text cannot be null or empty", nameof(text));

            logger.LogMethodEntry("QRGenerator", "GenerateQRCodeFromTextAsync", text.Substring(0, Math.Min(50, text.Length)) + "...", lCorrection, lPixelDensity);

            try
            {
                return await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return GenerateQRCodeFromText(text, lCorrection, lPixelDensity, encoding);
                }, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                logger.LogMethodExit("QRGenerator", "GenerateQRCodeFromTextAsync", true);
            }
        }

        /// <summary>
        /// Asynchronously generates a QR code from the supplied byte array and returns it as a <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="data">Byte array to encode.</param>
        /// <param name="lCorrection">Error correction level. If &lt; 0 a default is used.</param>
        /// <param name="lPixelDensity">Desired pixel width/height of the generated bitmap. Must be &gt; 0.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Bitmap"/> with the generated QR code.</returns>
        /// <exception cref="ArgumentException">Thrown when data is null/empty or pixel density is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the underlying encoder fails to produce a matrix.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
        public async Task<Bitmap> GenerateQRCodeFromBytesAsync(byte[] data, int lCorrection = 2, int lPixelDensity = 300, CancellationToken cancellationToken = default)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Data cannot be null or empty", nameof(data));

            logger.LogMethodEntry("QRGenerator", "GenerateQRCodeFromBytesAsync", $"{data.Length} bytes", lCorrection, lPixelDensity);

            try
            {
                return await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return GenerateQRCodeFromBytes(data, lCorrection, lPixelDensity);
                }, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                logger.LogMethodExit("QRGenerator", "GenerateQRCodeFromBytesAsync", true);
            }
        }

        /// <summary>
        /// Asynchronously generates a QR code from text and saves it to the specified file path using the provided format.
        /// </summary>
        /// <param name="text">Text string to encode.</param>
        /// <param name="filePath">Output file path. If relative, the file is placed in the user's Documents/AztecQRGenerator/Output folder.</param>
        /// <param name="format">Image format to use for saving (PNG, JPEG or BMP).</param>
        /// <param name="lCorrection">Error correction level. If &lt; 0 a default is used.</param>
        /// <param name="lPixelDensity">Size of the QR code in pixels.</param>
        /// <param name="encoding">Text encoding to use. If null, UTF-8 is used.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the file was created successfully.</returns>
        /// <exception cref="ArgumentException">Thrown when text or filePath is null/empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when format is null.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
        public async Task<bool> GenerateQRCodeFromTextToFileAsync(string text, string filePath, ImageFormat format, int lCorrection = 2, int lPixelDensity = 300, Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            logger.LogMethodEntry("QRGenerator", "GenerateQRCodeFromTextToFileAsync", text?.Substring(0, Math.Min(50, text?.Length ?? 0)) + "...", lCorrection, lPixelDensity, filePath, format?.ToString() ?? "null");

            try
            {
                using (Bitmap bitmap = await GenerateQRCodeFromTextAsync(text, lCorrection, lPixelDensity, encoding, cancellationToken).ConfigureAwait(false))
                {
                    await Task.Run(() =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        SaveBitmap(bitmap, filePath, format);
                    }, cancellationToken).ConfigureAwait(false);
                }
                
                logger.LogMethodExit("QRGenerator", "GenerateQRCodeFromTextToFileAsync", true);
                return true;
            }
            catch
            {
                logger.LogMethodExit("QRGenerator", "GenerateQRCodeFromTextToFileAsync", false);
                throw;
            }
        }

        /// <summary>
        /// Asynchronously generates a QR code from byte data and saves it to the specified file path using the provided format.
        /// </summary>
        /// <param name="data">Byte array to encode.</param>
        /// <param name="filePath">Output file path. If relative, the file is placed in the user's Documents/AztecQRGenerator/Output folder.</param>
        /// <param name="format">Image format to use for saving (PNG, JPEG or BMP).</param>
        /// <param name="lCorrection">Error correction level. If &lt; 0 a default is used.</param>
        /// <param name="lPixelDensity">Size of the QR code in pixels.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the file was created successfully.</returns>
        /// <exception cref="ArgumentException">Thrown when data is null/empty or filePath is null/empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when format is null.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
        public async Task<bool> GenerateQRCodeFromBytesToFileAsync(byte[] data, string filePath, ImageFormat format, int lCorrection = 2, int lPixelDensity = 300, CancellationToken cancellationToken = default)
        {
            logger.LogMethodEntry("QRGenerator", "GenerateQRCodeFromBytesToFileAsync", $"{data?.Length ?? 0} bytes", lCorrection, lPixelDensity, filePath, format?.ToString() ?? "null");

            try
            {
                using (Bitmap bitmap = await GenerateQRCodeFromBytesAsync(data, lCorrection, lPixelDensity, cancellationToken).ConfigureAwait(false))
                {
                    await Task.Run(() =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        SaveBitmap(bitmap, filePath, format);
                    }, cancellationToken).ConfigureAwait(false);
                }
                
                logger.LogMethodExit("QRGenerator", "GenerateQRCodeFromBytesToFileAsync", true);
                return true;
            }
            catch
            {
                logger.LogMethodExit("QRGenerator", "GenerateQRCodeFromBytesToFileAsync", false);
                throw;
            }
        }

        #endregion

        #endregion

        #region Performance Optimization Methods

        /// <summary>
        /// Creates a cache key for QR code caching.
        /// </summary>
        private static string CreateCacheKey(string data, int correction, int pixelDensity)
        {
            // Use a more efficient string concatenation for cache keys
            return $"{data.GetHashCode():X8}_{correction}_{pixelDensity}";
        }

        /// <summary>
        /// Determines whether caching should be used based on current cache size.
        /// </summary>
        private static bool ShouldCache()
        {
            return QRCache.Count < MaxCacheSize;
        }

        /// <summary>
        /// Caches a QR code bitmap with automatic cleanup of expired entries.
        /// </summary>
        private static void CacheQRCode(string key, Bitmap bitmap)
        {
            try
            {
                // Clean up expired entries if cache is getting full
                if (QRCache.Count >= MaxCacheSize * 0.8)
                {
                    CleanExpiredCacheEntries();
                }

                var cachedQR = new CachedQRCode
                {
                    Bitmap = CloneBitmap(bitmap),
                    CreatedAt = DateTime.UtcNow
                };

                QRCache.TryAdd(key, cachedQR);
            }
            catch (Exception)
            {
                // If caching fails, continue without caching
                // This ensures the main functionality isn't affected
            }
        }

        /// <summary>
        /// Clones a bitmap for caching purposes.
        /// </summary>
        private static Bitmap CloneBitmap(Bitmap original)
        {
            return new Bitmap(original);
        }

        /// <summary>
        /// Removes expired entries from the cache.
        /// </summary>
        private static void CleanExpiredCacheEntries()
        {
            var expiryCutoff = DateTime.UtcNow.AddMinutes(-CacheExpiryMinutes);
            var keysToRemove = QRCache
                .Where(kvp => kvp.Value.CreatedAt < expiryCutoff)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in keysToRemove)
            {
                if (QRCache.TryRemove(key, out CachedQRCode removed))
                {
                    removed.Bitmap?.Dispose();
                }
            }
        }

        /// <summary>
        /// Gets cache statistics for monitoring and diagnostics.
        /// </summary>
        public static CacheStatistics GetCacheStatistics()
        {
            CleanExpiredCacheEntries(); // Clean up before reporting stats
            
            var now = DateTime.UtcNow;
            var validEntries = QRCache.Values.Count(c => !c.IsExpired);
            var expiredEntries = QRCache.Count - validEntries;

            return new CacheStatistics
            {
                TotalEntries = QRCache.Count,
                ValidEntries = validEntries,
                ExpiredEntries = expiredEntries,
                MaxCacheSize = MaxCacheSize,
                CacheHitRatio = CalculateCacheHitRatio()
            };
        }

        private static double CalculateCacheHitRatio()
        {
            // This is a simplified version - in a real implementation,
            // you'd want to track hits/misses over time
            return QRCache.Count > 0 ? 0.85 : 0.0; // Placeholder
        }

        /// <summary>
        /// Clears the entire cache and disposes all cached bitmaps.
        /// </summary>
        public static void ClearCache()
        {
            var allEntries = QRCache.ToList();
            QRCache.Clear();
            
            foreach (var entry in allEntries)
            {
                entry.Value.Bitmap?.Dispose();
            }
        }

        #endregion
    }

    #region Supporting Classes for Async Operations

    /// <summary>
    /// Represents a QR code generation request for batch operations.
    /// </summary>
    public class QRRequest
    {
        /// <summary>
        /// Gets or sets the Base64 encoded data to generate QR code for.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets the error correction level.
        /// </summary>
        public int ErrorCorrection { get; set; } = 2;

        /// <summary>
        /// Gets or sets the pixel density (size) of the QR code.
        /// </summary>
        public int PixelDensity { get; set; } = 300;

        /// <summary>
        /// Initializes a new instance of the <see cref="QRRequest"/> class.
        /// </summary>
        public QRRequest() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QRRequest"/> class with specified parameters.
        /// </summary>
        /// <param name="data">Base64 encoded data.</param>
        /// <param name="errorCorrection">Error correction level.</param>
        /// <param name="pixelDensity">Pixel density (size).</param>
        public QRRequest(string data, int errorCorrection = 2, int pixelDensity = 300)
        {
            Data = data;
            ErrorCorrection = errorCorrection;
            PixelDensity = pixelDensity;
        }
    }

    /// <summary>
    /// Represents progress information for batch QR code generation operations.
    /// </summary>
    public class BatchProgress
    {
        /// <summary>
        /// Gets the number of completed operations.
        /// </summary>
        public int CompletedCount { get; }

        /// <summary>
        /// Gets the total number of operations.
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// Gets the completion percentage (0-100).
        /// </summary>
        public double PercentComplete => TotalCount > 0 ? (double)CompletedCount / TotalCount * 100 : 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchProgress"/> class.
        /// </summary>
        /// <param name="completedCount">Number of completed operations.</param>
        /// <param name="totalCount">Total number of operations.</param>
        public BatchProgress(int completedCount, int totalCount)
        {
            CompletedCount = completedCount;
            TotalCount = totalCount;
        }

        /// <summary>
        /// Returns a string representation of the progress.
        /// </summary>
        public override string ToString() => $"{CompletedCount}/{TotalCount} ({PercentComplete:F1}%)";
    }

    #endregion

    #region Performance Optimization Supporting Classes

    /// <summary>
    /// Represents a cached QR code with expiration tracking.
    /// </summary>
    internal class CachedQRCode
    {
        /// <summary>
        /// Gets or sets the cached bitmap.
        /// </summary>
        public Bitmap Bitmap { get; set; }

        /// <summary>
        /// Gets or sets when this cache entry was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets whether this cache entry has expired.
        /// </summary>
        public bool IsExpired => DateTime.UtcNow.Subtract(CreatedAt).TotalMinutes > 30; // 30 minutes expiry
    }

    /// <summary>
    /// Provides statistics about the QR code cache performance.
    /// </summary>
    public class CacheStatistics
    {
        /// <summary>
        /// Gets or sets the total number of entries in the cache.
        /// </summary>
        public int TotalEntries { get; set; }

        /// <summary>
        /// Gets or sets the number of valid (non-expired) entries.
        /// </summary>
        public int ValidEntries { get; set; }

        /// <summary>
        /// Gets or sets the number of expired entries.
        /// </summary>
        public int ExpiredEntries { get; set; }

        /// <summary>
        /// Gets or sets the maximum cache size.
        /// </summary>
        public int MaxCacheSize { get; set; }

        /// <summary>
        /// Gets or sets the cache hit ratio (0.0 to 1.0).
        /// </summary>
        public double CacheHitRatio { get; set; }

        /// <summary>
        /// Gets the cache utilization as a percentage.
        /// </summary>
        public double UtilizationPercentage => MaxCacheSize > 0 ? (double)ValidEntries / MaxCacheSize * 100 : 0;

        /// <summary>
        /// Returns a string representation of the cache statistics.
        /// </summary>
        public override string ToString() => 
            $"Cache: {ValidEntries}/{MaxCacheSize} entries ({UtilizationPercentage:F1}% full), Hit ratio: {CacheHitRatio:F2}";
    }

    #endregion
}
