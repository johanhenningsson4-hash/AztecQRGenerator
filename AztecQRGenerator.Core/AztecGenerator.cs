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
using System.Runtime.InteropServices;

namespace AztecQR
{
    /// <summary>
    /// Provides methods for generating Aztec codes as bitmaps or files.
    /// </summary>
    public class AztecGenerator
    {
        private readonly Logger logger = Logger.Instance;

        /// <summary>
        /// Generates an Aztec code and returns it as a Bitmap object.
        /// </summary>
        /// <param name="aztecstring">Base64 encoded string to encode.</param>
        /// <param name="lCorrection">Error correction level.</param>
        /// <param name="lPixelDensity">Size of the Aztec code in pixels.</param>
        /// <returns>Bitmap containing the Aztec code.</returns>
        /// <exception cref="ArgumentException">Thrown if input is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown if Aztec code generation fails.</exception>
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
        /// Generates an Aztec code and saves it to a file with the specified format.
        /// </summary>
        /// <param name="aztecstring">Base64 encoded string to encode.</param>
        /// <param name="lCorrection">Error correction level.</param>
        /// <param name="lPixelDensity">Size of the Aztec code in pixels.</param>
        /// <param name="filePath">Output file path.</param>
        /// <param name="format">Image format (PNG, JPEG, or BMP).</param>
        /// <returns>True if successful.</returns>
        /// <exception cref="ArgumentException">Thrown if file path is invalid.</exception>
        /// <exception cref="ArgumentNullException">Thrown if format is null.</exception>
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
        /// </summary>
        /// <param name="aztecstring">Base64 encoded string to encode.</param>
        /// <param name="lCorrection">Error correction level.</param>
        /// <param name="lPixelDensity">Size of the Aztec code in pixels.</param>
        /// <returns>True if successful.</returns>
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
    }
}
