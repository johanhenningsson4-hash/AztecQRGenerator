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
    public class AztecGenerator
    {
        private readonly Logger logger = Logger.Instance;

        /// <summary>
        /// Generates an Aztec code and returns it as a Bitmap object
        /// </summary>
        /// <param name="aztecstring">Base64 encoded string to encode</param>
        /// <param name="lCorrection">Error correction level</param>
        /// <param name="lPixelDensity">Size of the Aztec code in pixels</param>
        /// <returns>Bitmap containing the Aztec code</returns>
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

                // Create AztecWriter and generate BitMatrix
                var writer = new AztecWriter();
                var hints = new Dictionary<EncodeHintType, object>
                {
                    { EncodeHintType.MARGIN, 0 }
                };

                BitMatrix matrix;
                try
                {
                    string decodedString = Encoding.GetEncoding("ISO-8859-1").GetString(data);
                    logger.Debug($"Encoding Aztec code data with ISO-8859-1");
                    
                    matrix = writer.encode(
                        decodedString,
                        BarcodeFormat.AZTEC,
                        lPixelDensity,
                        lPixelDensity,
                        hints
                    );
                    
                    logger.Info($"Aztec code matrix generated successfully - Dimensions: {matrix.Width}x{matrix.Height}");
                }
                catch (Exception ex)
                {
                    logger.Error("Failed to generate Aztec code matrix", ex);
                    throw new InvalidOperationException("Failed to generate Aztec code matrix", ex);
                }

                // Convert BitMatrix to Bitmap
                Bitmap bitmap = ConvertBitMatrixToBitmap(matrix);
                logger.Info("Aztec code Bitmap created successfully");
                logger.LogMethodExit("AztecGenerator", "GenerateAztecCodeAsBitmap", true);
                
                return bitmap;
            }
            catch (Exception ex)
            {
                logger.Error($"Aztec code bitmap generation failed", ex);
                logger.LogMethodExit("AztecGenerator", "GenerateAztecCodeAsBitmap", false);
                throw;
            }
        }

        /// <summary>
        /// Generates an Aztec code and saves it to a file with the specified format
        /// </summary>
        /// <param name="aztecstring">Base64 encoded string to encode</param>
        /// <param name="lCorrection">Error correction level</param>
        /// <param name="lPixelDensity">Size of the Aztec code in pixels</param>
        /// <param name="filePath">Output file path</param>
        /// <param name="format">Image format (PNG, JPEG, or BMP)</param>
        /// <returns>True if successful</returns>
        public bool GenerateAztecCodeToFile(string aztecstring, int lCorrection, int lPixelDensity, string filePath, ImageFormat format)
        {
            logger.LogMethodEntry("AztecGenerator", "GenerateAztecCodeToFile", "Base64 data", lCorrection, lPixelDensity, filePath, format.ToString());

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

                logger.Info($"Generating Aztec code to file: {filePath}, Format: {format}");

                // Generate the Aztec code as bitmap
                using (Bitmap bitmap = GenerateAztecCodeAsBitmap(aztecstring, lCorrection, lPixelDensity))
                {
                    SaveBitmap(bitmap, filePath, format);
                    logger.Info($"Aztec code saved successfully to: {filePath}");
                }

                logger.LogMethodExit("AztecGenerator", "GenerateAztecCodeToFile", true);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error($"Aztec code file generation failed", ex);
                logger.LogMethodExit("AztecGenerator", "GenerateAztecCodeToFile", false);
                throw;
            }
        }

        public bool GenerateAztecBitmap(string aztecstring, int lCorrection, int lPixelDensity)
        {
            logger.LogMethodEntry("AztecGenerator", "GenerateAztecBitmap", "Base64 data", lCorrection, lPixelDensity);

            try
            {
                // Generate the Aztec code as bitmap
                using (Bitmap bitmap = GenerateAztecCodeAsBitmap(aztecstring, lCorrection, lPixelDensity))
                {
                    // Save as PNG
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    string fileName = $"AztecCode_{timestamp}.png";
                    string scaledFileName = $"AztecCode_Scaled_{timestamp}.png";

                    try
                    {
                        SaveBitmap(bitmap, fileName, ImageFormat.Png);
                        logger.Info($"Aztec code saved successfully: {fileName}");

                        SaveBitmap(bitmap, scaledFileName, ImageFormat.Png);
                        logger.Info($"Scaled Aztec code saved successfully: {scaledFileName}");
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Failed to save Aztec code to file", ex);
                        throw new IOException("Failed to save Aztec code image", ex);
                    }
                }

                logger.LogMethodExit("AztecGenerator", "GenerateAztecBitmap", true);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error($"Aztec code generation failed", ex);
                logger.LogMethodExit("AztecGenerator", "GenerateAztecBitmap", false);
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
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    logger.Info($"Creating directory: {directory}");
                    Directory.CreateDirectory(directory);
                }

                bitmap.Save(filePath, format);
                logger.Debug($"Image saved successfully: {filePath} ({bitmap.Width}x{bitmap.Height}, Format: {format})");
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
