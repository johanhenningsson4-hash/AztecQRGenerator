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
using System.Text;
using System.Runtime.InteropServices;

namespace AztecQR
{
    /// <summary>
    /// Provides methods for generating QR codes as bitmaps or files.
    /// </summary>
    public class QRGenerator
    {
        private readonly Logger logger = Logger.Instance;

        /// <summary>
        /// Generates a QR code and returns it as a Bitmap object.
        /// </summary>
        /// <param name="qrstring">Base64 encoded string to encode.</param>
        /// <param name="lCorrection">Error correction level.</param>
        /// <param name="lPixelDensity">Size of the QR code in pixels.</param>
        /// <returns>Bitmap containing the QR code.</returns>
        /// <exception cref="ArgumentException">Thrown if input is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown if QR code generation fails.</exception>
        public Bitmap GenerateQRCodeAsBitmap(string qrstring, int lCorrection, int lPixelDensity)
        {
            logger.LogMethodEntry("QRGenerator", "GenerateQRCodeAsBitmap", "Base64 data", lCorrection, lPixelDensity);

            if (string.IsNullOrWhiteSpace(qrstring))
                throw new ArgumentException("QR string cannot be null or empty", nameof(qrstring));
            if (lPixelDensity <= 0)
                throw new ArgumentException("Pixel density must be greater than zero", nameof(lPixelDensity));
            if (lCorrection < 0)
                lCorrection = 2;

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

                return ConvertBitMatrixToBitmap(matrix);
            }
            finally
            {
                logger.LogMethodExit("QRGenerator", "GenerateQRCodeAsBitmap", true);
            }
        }

        /// <summary>
        /// Generates a QR code and saves it to a file with the specified format.
        /// </summary>
        /// <param name="qrstring">Base64 encoded string to encode.</param>
        /// <param name="lCorrection">Error correction level.</param>
        /// <param name="lPixelDensity">Size of the QR code in pixels.</param>
        /// <param name="filePath">Output file path.</param>
        /// <param name="format">Image format (PNG, JPEG, or BMP).</param>
        /// <returns>True if successful.</returns>
        /// <exception cref="ArgumentException">Thrown if file path is invalid.</exception>
        /// <exception cref="ArgumentNullException">Thrown if format is null.</exception>
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
        /// </summary>
        /// <param name="qrstring">Base64 encoded string to encode.</param>
        /// <param name="lCorrection">Error correction level.</param>
        /// <param name="lPixelDensity">Size of the QR code in pixels.</param>
        /// <returns>True if successful.</returns>
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
