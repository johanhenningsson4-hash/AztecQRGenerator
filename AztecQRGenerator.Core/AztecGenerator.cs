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
using System.Text;
using ZXing;
using ZXing.Aztec;
using ZXing.Common;
using BitMatrix = ZXing.Common.BitMatrix;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace AztecQR
{
    /// <summary>
    /// Provides methods for generating Aztec codes as bitmaps or files.
    /// </summary>
    public class AztecGenerator
    {
        private readonly Logger logger = Logger.Instance;
        
        public AztecGenerator()
        {
            // Ensure logging is activated by default
            logger.SetMinimumLogLevel(LogLevel.Debug);
        }

        /// <summary>
        /// Generates an Aztec code from the supplied Base64-encoded string and returns it
        /// as a <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="aztecstring">Base64 encoded string to encode.</param>
        /// <param name="lCorrection">Error correction level. If &lt; 0 a default is used.</param>
        /// <param name="lPixelDensity">Desired pixel width/height of the generated bitmap. Must be &gt; 0.</param>
        /// <returns>A <see cref="Bitmap"/> containing the generated Aztec code.</returns>
        /// <exception cref="ArgumentException">Thrown when input is null/empty or pixel density is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the underlying encoder fails to produce a matrix.</exception>
        public Bitmap GenerateAztecCodeAsBitmap(string aztecstring, int lCorrection, int lPixelDensity)
        {
            logger.LogMethodEntry("AztecGenerator", "GenerateAztecCodeAsBitmap", "Base64 data", lCorrection, lPixelDensity);

            try
            {
                // Validate input parameters
                if (string.IsNullOrWhiteSpace(aztecstring))
                {
                    logger.Error("Aztec string is null or empty");
                    throw new ArgumentException("Aztec string cannot be null or empty", nameof(aztecstring));
                }

                if (lPixelDensity <= 0)
                {
                    logger.Error($"Invalid pixel density: {lPixelDensity}");
                    throw new ArgumentException("Pixel density must be greater than zero", nameof(lPixelDensity));
                }

                if (lCorrection < 0)
                {
                    logger.Warning($"Invalid correction level: {lCorrection}. Using default value.");
                    lCorrection = 2;
                }

                logger.Info($"Generating Aztec code as Bitmap - Size: {lPixelDensity}, Correction: {lCorrection}");

                // Decode Base64 string
                byte[] data;
                try
                {
                    data = Convert.FromBase64String(aztecstring);
                    logger.Debug($"Base64 decoded successfully, data length: {data.Length} bytes");
                }
                catch (System.FormatException ex)
                {
                    logger.Error("Failed to decode Base64 string - Invalid format", ex);
                    throw new ArgumentException("Invalid Base64 string format", nameof(aztecstring), ex);
                }

                var writer = new AztecWriter();
                var hints = new Dictionary<EncodeHintType, object> { { EncodeHintType.MARGIN, 0 } };
                BitMatrix matrix;
                try
                {
                    string decodedString = Encoding.GetEncoding("ISO-8859-1").GetString(data);
                    matrix = writer.encode(
                        decodedString,
                        BarcodeFormat.AZTEC,
                        lPixelDensity,
                        lPixelDensity,
                        hints
                    );
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to generate Aztec code matrix", ex);
                }

                return ConvertBitMatrixToBitmap(matrix);
            }
            finally
            {
                logger.LogMethodExit("AztecGenerator", "GenerateAztecCodeAsBitmap", true);
            }
        }

        /// <summary>
        /// Generates an Aztec code and saves it to <paramref name="filePath"/> using the
        /// provided <paramref name="format"/>.
        /// </summary>
        /// <param name="aztecstring">Base64 encoded string to encode.</param>
        /// <param name="lCorrection">Error correction level.</param>
        /// <param name="lPixelDensity">Size of the Aztec code in pixels.</param>
        /// <param name="filePath">Output file path. If relative, the file is placed in the user's Documents/AztecQRGenerator/Output folder.</param>
        /// <param name="format">Image format to use for saving (PNG, JPEG or BMP).</param>
        /// <returns>True when the file was created successfully.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="format"/> is null.</exception>
        public bool GenerateAztecCodeToFile(string aztecstring, int lCorrection, int lPixelDensity, string filePath, ImageFormat format)
        {
            logger.LogMethodEntry("AztecGenerator", "GenerateAztecCodeToFile", "Base64 data", lCorrection, lPixelDensity, filePath, format?.ToString() ?? "null");

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            if (format == null)
                throw new ArgumentNullException(nameof(format), "Image format cannot be null");

            using (Bitmap bitmap = GenerateAztecCodeAsBitmap(aztecstring, lCorrection, lPixelDensity))
            {
                SaveBitmap(bitmap, filePath, format);
            }
            logger.LogMethodExit("AztecGenerator", "GenerateAztecCodeToFile", true);
            return true;
        }

        /// <summary>
        /// Generates an Aztec code and saves it as a PNG file with a timestamped name in the user's Documents folder.
        /// This is a convenience wrapper around <see cref="GenerateAztecCodeToFile"/> using the PNG format.
        /// </summary>
        /// <param name="aztecstring">Base64 encoded string to encode.</param>
        /// <param name="lCorrection">Error correction level.</param>
        /// <param name="lPixelDensity">Size of the Aztec code in pixels.</param>
        /// <returns>True when the file was created successfully.</returns>
        public bool GenerateAztecBitmap(string aztecstring, int lCorrection, int lPixelDensity)
        {
            logger.LogMethodEntry("AztecGenerator", "GenerateAztecBitmap", "Base64 data", lCorrection, lPixelDensity);

            using (Bitmap bitmap = GenerateAztecCodeAsBitmap(aztecstring, lCorrection, lPixelDensity))
            {
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string fileName = $"AztecCode_{timestamp}.png";
                SaveBitmap(bitmap, fileName, ImageFormat.Png);
            }
            logger.LogMethodExit("AztecGenerator", "GenerateAztecBitmap", true);
            return true;
        }

        /// <summary>
        /// Converts a BitMatrix to a Bitmap object.
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

        #region Async Methods

        /// <summary>
        /// Asynchronously generates an Aztec code from the supplied Base64-encoded string and returns
        /// it as a <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="aztecstring">Base64 encoded string to encode.</param>
        /// <param name="lCorrection">Error correction level. If &lt; 0 a default is used.</param>
        /// <param name="lPixelDensity">Desired pixel width/height of the generated bitmap. Must be &gt; 0.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Bitmap"/> with the generated Aztec code.</returns>
        /// <exception cref="ArgumentException">Thrown when input is null/empty or pixel density is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the underlying encoder fails to produce a matrix.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
        public async Task<Bitmap> GenerateAztecCodeAsBitmapAsync(string aztecstring, int lCorrection, int lPixelDensity, CancellationToken cancellationToken = default)
        {
            // Input validation (fast, no need to run on thread pool)
            if (string.IsNullOrWhiteSpace(aztecstring))
                throw new ArgumentException("Aztec string cannot be null or empty", nameof(aztecstring));
            if (lPixelDensity <= 0)
                throw new ArgumentException("Pixel density must be greater than zero", nameof(lPixelDensity));

            logger.LogMethodEntry("AztecGenerator", "GenerateAztecCodeAsBitmapAsync", "Base64 data", lCorrection, lPixelDensity);

            try
            {
                // Run the CPU-intensive work on a background thread
                return await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return GenerateAztecCodeAsBitmap(aztecstring, lCorrection, lPixelDensity);
                }, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                logger.LogMethodExit("AztecGenerator", "GenerateAztecCodeAsBitmapAsync", true);
            }
        }

        /// <summary>
        /// Asynchronously generates an Aztec code and saves it to the specified file path using the provided format.
        /// </summary>
        /// <param name="aztecstring">Base64 encoded string to encode.</param>
        /// <param name="lCorrection">Error correction level.</param>
        /// <param name="lPixelDensity">Size of the Aztec code in pixels.</param>
        /// <param name="filePath">Output file path. If relative, the file is placed in the user's Documents/AztecQRGenerator/Output folder.</param>
        /// <param name="format">Image format to use for saving (PNG, JPEG or BMP).</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the file was created successfully.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="format"/> is null.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
        public async Task<bool> GenerateAztecCodeToFileAsync(string aztecstring, int lCorrection, int lPixelDensity, string filePath, ImageFormat format, CancellationToken cancellationToken = default)
        {
            // Input validation (fast, no need to run on thread pool)
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            if (format == null)
                throw new ArgumentNullException(nameof(format), "Image format cannot be null");

            logger.LogMethodEntry("AztecGenerator", "GenerateAztecCodeToFileAsync", "Base64 data", lCorrection, lPixelDensity, filePath, format?.ToString() ?? "null");

            try
            {
                // Generate bitmap asynchronously
                using (Bitmap bitmap = await GenerateAztecCodeAsBitmapAsync(aztecstring, lCorrection, lPixelDensity, cancellationToken).ConfigureAwait(false))
                {
                    // Save to file on background thread
                    await Task.Run(() =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        SaveBitmap(bitmap, filePath, format);
                    }, cancellationToken).ConfigureAwait(false);
                }

                logger.LogMethodExit("AztecGenerator", "GenerateAztecCodeToFileAsync", true);
                return true;
            }
            catch
            {
                logger.LogMethodExit("AztecGenerator", "GenerateAztecCodeToFileAsync", false);
                throw;
            }
        }

        /// <summary>
        /// Asynchronously generates an Aztec code and saves it as a PNG file with a timestamped name in the user's Documents folder.
        /// This is a convenience wrapper around <see cref="GenerateAztecCodeToFileAsync"/> using the PNG format.
        /// </summary>
        /// <param name="aztecstring">Base64 encoded string to encode.</param>
        /// <param name="lCorrection">Error correction level.</param>
        /// <param name="lPixelDensity">Size of the Aztec code in pixels.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the file was created successfully.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
        public async Task<bool> GenerateAztecBitmapAsync(string aztecstring, int lCorrection, int lPixelDensity, CancellationToken cancellationToken = default)
        {
            logger.LogMethodEntry("AztecGenerator", "GenerateAztecBitmapAsync", "Base64 data", lCorrection, lPixelDensity);

            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string fileName = $"AztecCode_{timestamp}.png";
                
                return await GenerateAztecCodeToFileAsync(aztecstring, lCorrection, lPixelDensity, fileName, ImageFormat.Png, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                logger.LogMethodExit("AztecGenerator", "GenerateAztecBitmapAsync", true);
            }
        }

        /// <summary>
        /// Asynchronously generates multiple Aztec codes in batch with progress reporting.
        /// </summary>
        /// <param name="requests">Collection of Aztec code generation requests.</param>
        /// <param name="progress">Optional progress reporter for batch operation.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous batch operation. The task result contains the generated bitmaps.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="requests"/> is null.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
        public async Task<IEnumerable<Bitmap>> GenerateBatchAsync(IEnumerable<AztecRequest> requests, IProgress<BatchProgress> progress = null, CancellationToken cancellationToken = default)
        {
            if (requests == null)
                throw new ArgumentNullException(nameof(requests));

            logger.LogMethodEntry("AztecGenerator", "GenerateBatchAsync", $"Processing {requests.Count()} requests");

            var requestList = requests.ToList();
            var results = new List<Bitmap>();
            var totalCount = requestList.Count;
            var completedCount = 0;

            try
            {
                foreach (var request in requestList)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var bitmap = await GenerateAztecCodeAsBitmapAsync(request.Data, request.ErrorCorrection, request.PixelDensity, cancellationToken).ConfigureAwait(false);
                    results.Add(bitmap);

                    completedCount++;
                    progress?.Report(new BatchProgress(completedCount, totalCount));
                }

                logger.LogMethodExit("AztecGenerator", "GenerateBatchAsync", true);
                return results;
            }
            catch
            {
                // Cleanup any generated bitmaps on error
                foreach (var bitmap in results)
                {
                    bitmap?.Dispose();
                }
                logger.LogMethodExit("AztecGenerator", "GenerateBatchAsync", false);
                throw;
            }
        }

        #endregion
    }

    #region Supporting Classes for Aztec Async Operations

    /// <summary>
    /// Represents an Aztec code generation request for batch operations.
    /// </summary>
    public class AztecRequest
    {
        /// <summary>
        /// Gets or sets the Base64 encoded data to generate Aztec code for.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets the error correction level.
        /// </summary>
        public int ErrorCorrection { get; set; } = 2;

        /// <summary>
        /// Gets or sets the pixel density (size) of the Aztec code.
        /// </summary>
        public int PixelDensity { get; set; } = 300;

        /// <summary>
        /// Initializes a new instance of the <see cref="AztecRequest"/> class.
        /// </summary>
        public AztecRequest() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AztecRequest"/> class with specified parameters.
        /// </summary>
        /// <param name="data">Base64 encoded data.</param>
        /// <param name="errorCorrection">Error correction level.</param>
        /// <param name="pixelDensity">Pixel density (size).</param>
        public AztecRequest(string data, int errorCorrection = 2, int pixelDensity = 300)
        {
            Data = data;
            ErrorCorrection = errorCorrection;
            PixelDensity = pixelDensity;
        }
    }

    #endregion
}
