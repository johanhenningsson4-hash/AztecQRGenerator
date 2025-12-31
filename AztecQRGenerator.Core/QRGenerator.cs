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
    public class QRGenerator
    {
        private readonly Logger logger = Logger.Instance;

        /// <summary>
        /// Generates a QR code and returns it as a Bitmap object
        /// </summary>
        /// <param name="qrstring">Base64 encoded string to encode</param>
        /// <param name="lCorrection">Error correction level</param>
        /// <param name="lPixelDensity">Size of the QR code in pixels</param>
        /// <returns>Bitmap containing the QR code</returns>
        public Bitmap GenerateQRCodeAsBitmap(string qrstring, int lCorrection, int lPixelDensity)
        {
            logger.LogMethodEntry("QRGenerator", "GenerateQRCodeAsBitmap", "Base64 data", lCorrection, lPixelDensity);

            try
            {
                // Validate input parameters
                if (string.IsNullOrWhiteSpace(qrstring))
                {
                    logger.Error("QR string is null or empty");
                    throw new ArgumentException("QR string cannot be null or empty", nameof(qrstring));
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

                logger.Info($"Generating QR code as Bitmap - Size: {lPixelDensity}, Correction: {lCorrection}");

                // Decode Base64 string
                byte[] data;
                try
                {
                    data = Convert.FromBase64String(qrstring);
                    logger.Debug($"Base64 decoded successfully, data length: {data.Length} bytes");
                }
                catch (System.FormatException ex)
                {
                    logger.Error("Failed to decode Base64 string - Invalid format", ex);
                    throw new ArgumentException("Invalid Base64 string format", nameof(qrstring), ex);
                }

                // Create QRCodeWriter and generate BitMatrix
                var writer = new QRCodeWriter();
                var hints = new Dictionary<EncodeHintType, object>
                {
                    { EncodeHintType.MARGIN, 0 }
                };

                BitMatrix matrix;
                try
                {
                    string decodedString = Encoding.GetEncoding("ISO-8859-1").GetString(data);
                    logger.Debug($"Encoding QR code data with ISO-8859-1");
                    
                    matrix = writer.encode(
                        decodedString,
                        BarcodeFormat.QR_CODE,
                        lPixelDensity,
                        lPixelDensity,
                        hints
                    );
                    
                    logger.Info($"QR code matrix generated successfully - Dimensions: {matrix.Width}x{matrix.Height}");
                }
                catch (Exception ex)
                {
                    logger.Error("Failed to generate QR code matrix", ex);
                    throw new InvalidOperationException("Failed to generate QR code matrix", ex);
                }

                // Convert BitMatrix to Bitmap
                Bitmap bitmap = ConvertBitMatrixToBitmap(matrix);
                logger.Info("QR code Bitmap created successfully");
                logger.LogMethodExit("QRGenerator", "GenerateQRCodeAsBitmap", true);
                
                return bitmap;
            }
            catch (Exception ex)
            {
                logger.Error($"QR code bitmap generation failed", ex);
                logger.LogMethodExit("QRGenerator", "GenerateQRCodeAsBitmap", false);
                throw;
            }
        }

        /// <summary>
        /// Generates a QR code and saves it to a file with the specified format
        /// </summary>
        /// <param name="qrstring">Base64 encoded string to encode</param>
        /// <param name="lCorrection">Error correction level</param>
        /// <param name="lPixelDensity">Size of the QR code in pixels</param>
        /// <param name="filePath">Output file path</param>
        /// <param name="format">Image format (PNG, JPEG, or BMP)</param>
        /// <returns>True if successful</returns>
        public bool GenerateQRCodeToFile(string qrstring, int lCorrection, int lPixelDensity, string filePath, ImageFormat format)
        {
            logger.LogMethodEntry("QRGenerator", "GenerateQRCodeToFile", "Base64 data", lCorrection, lPixelDensity, filePath, format.ToString());

            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    logger.Error("File path is null or empty");
                    throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
                }

                if (format == null)
                {
                    logger.Error("Image format is null");
                    throw new ArgumentNullException(nameof(format), "Image format cannot be null");
                }

                logger.Info($"Generating QR code to file: {filePath}, Format: {format}");

                // Generate the QR code as bitmap
                using (Bitmap bitmap = GenerateQRCodeAsBitmap(qrstring, lCorrection, lPixelDensity))
                {
                    SaveBitmap(bitmap, filePath, format);
                    logger.Info($"QR code saved successfully to: {filePath}");
                }

                logger.LogMethodExit("QRGenerator", "GenerateQRCodeToFile", true);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error($"QR code file generation failed", ex);
                logger.LogMethodExit("QRGenerator", "GenerateQRCodeToFile", false);
                throw;
            }
        }

        public bool GenerateQRBitmap(string qrstring, int lCorrection, int lPixelDensity)
        {
            logger.LogMethodEntry("QRGenerator", "GenerateQRBitmap", "Base64 data", lCorrection, lPixelDensity);

            try
            {
                // Generate the QR code as bitmap
                using (Bitmap bitmap = GenerateQRCodeAsBitmap(qrstring, lCorrection, lPixelDensity))
                {
                    // Save as PNG
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    string fileName = $"QRCode_{timestamp}.png";
                    string scaledFileName = $"QRCode_Scaled_{timestamp}.png";

                    try
                    {
                        SaveBitmap(bitmap, fileName, ImageFormat.Png);
                        logger.Info($"QR code saved successfully: {fileName}");

                        SaveBitmap(bitmap, scaledFileName, ImageFormat.Png);
                        logger.Info($"Scaled QR code saved successfully: {scaledFileName}");
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Failed to save QR code to file", ex);
                        throw new IOException("Failed to save QR code image", ex);
                    }
                }

                logger.LogMethodExit("QRGenerator", "GenerateQRBitmap", true);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error($"QR code generation failed", ex);
                logger.LogMethodExit("QRGenerator", "GenerateQRBitmap", false);
                throw;
            }
        }

        /// <summary>
        /// Converts a BitMatrix to a Bitmap object
        /// </summary>
        private Bitmap ConvertBitMatrixToBitmap(BitMatrix matrix)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException(nameof(matrix), "BitMatrix cannot be null");
            }

            logger.Debug($"Converting BitMatrix to Bitmap - Size: {matrix.Width}x{matrix.Height}");

            int width = matrix.Width;
            int height = matrix.Height;
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            try
            {
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

                logger.Debug("BitMatrix converted to Bitmap successfully");
                return bmp;
            }
            catch (Exception ex)
            {
                logger.Error("Failed to convert BitMatrix to Bitmap", ex);
                bmp.Dispose();
                throw;
            }
        }

        private void SaveBitMatrixAsPng(BitMatrix matrix, string filePath)
        {
            logger.Debug($"Saving BitMatrix to: {filePath}");

            if (matrix == null)
            {
                throw new ArgumentNullException(nameof(matrix), "BitMatrix cannot be null");
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            }

            try
            {
                using (var bmp = ConvertBitMatrixToBitmap(matrix))
                {
                    SaveBitmap(bmp, filePath, ImageFormat.Png);
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Failed to save image to {filePath}", ex);
                throw;
            }
        }

        /// <summary>
        /// Saves a Bitmap to a file with the specified format
        /// </summary>
        /// <param name="bitmap">Bitmap to save</param>
        /// <param name="filePath">Output file path</param>
        /// <param name="format">Image format (PNG, JPEG, or BMP)</param>
        private void SaveBitmap(Bitmap bitmap, string filePath, ImageFormat format)
        {
            logger.Debug($"Saving Bitmap to: {filePath}, Format: {format}");

            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap), "Bitmap cannot be null");
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            }

            if (format == null)
            {
                throw new ArgumentNullException(nameof(format), "Image format cannot be null");
            }

            try
            {
                string directory = Path.GetDirectoryName(filePath);
                
                // If path is relative or no directory specified, use Documents folder
                if (string.IsNullOrEmpty(directory) || !Path.IsPathRooted(filePath))
                {
                    string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string outputDir = Path.Combine(documentsPath, "AztecQRGenerator", "Output");
                    
                    if (!Directory.Exists(outputDir))
                    {
                        logger.Info($"Creating output directory: {outputDir}");
                        Directory.CreateDirectory(outputDir);
                    }
                    
                    string fileName = Path.GetFileName(filePath);
                    filePath = Path.Combine(outputDir, fileName);
                    logger.Info($"Using safe output path: {filePath}");
                }
                else if (!Directory.Exists(directory))
                {
                    try
                    {
                        logger.Info($"Creating directory: {directory}");
                        Directory.CreateDirectory(directory);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // If we don't have permission, fall back to Documents
                        logger.Warning($"No write access to {directory}, using Documents folder");
                        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        string outputDir = Path.Combine(documentsPath, "AztecQRGenerator", "Output");
                        
                        if (!Directory.Exists(outputDir))
                        {
                            Directory.CreateDirectory(outputDir);
                        }
                        
                        string fileName = Path.GetFileName(filePath);
                        filePath = Path.Combine(outputDir, fileName);
                        logger.Info($"Using fallback path: {filePath}");
                    }
                }

                bitmap.Save(filePath, format);
                logger.Info($"Image saved successfully: {filePath} ({bitmap.Width}x{bitmap.Height}, Format: {format})");
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.Error($"Access denied when saving to {filePath}", ex);
                throw new IOException($"Access denied. File will be saved to Documents folder instead. Check log for details.", ex);
            }
            catch (Exception ex)
            {
                logger.Error($"Failed to save image to {filePath}", ex);
                throw;
            }
        }

        /// <summary>
        /// Saves a Bitmap to a PNG file (deprecated - use SaveBitmap instead)
        /// </summary>
        private void SaveBitmapAsPng(Bitmap bitmap, string filePath)
        {
            SaveBitmap(bitmap, filePath, ImageFormat.Png);
        }
    }
}
