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

namespace AztecQR
{
    public class AztecGenerator
    {
        private readonly Logger logger = Logger.Instance;

        public bool GenerateAztecBitmap(int lTaNmbrqr, string aztecstring, int lCorrection, int lPixelDensity)
        {
            logger.LogMethodEntry("AztecGenerator", "GenerateAztecBitmap", lTaNmbrqr, "Base64 data", lCorrection, lPixelDensity);

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

                logger.Info($"Generating Aztec code - ID: {lTaNmbrqr}, Size: {lPixelDensity}, Correction: {lCorrection}");

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

                // Save as PNG
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string fileName = $"AztecCode_{timestamp}.png";
                string scaledFileName = $"AztecCode_Scaled_{timestamp}.png";

                try
                {
                    SaveBitMatrixAsPng(matrix, fileName);
                    logger.Info($"Aztec code saved successfully: {fileName}");

                    SaveBitMatrixAsPng(matrix, scaledFileName);
                    logger.Info($"Scaled Aztec code saved successfully: {scaledFileName}");
                }
                catch (Exception ex)
                {
                    logger.Error($"Failed to save Aztec code to file", ex);
                    throw new IOException("Failed to save Aztec code image", ex);
                }

                logger.LogMethodExit("AztecGenerator", "GenerateAztecBitmap", true);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error($"Aztec code generation failed for ID: {lTaNmbrqr}", ex);
                logger.LogMethodExit("AztecGenerator", "GenerateAztecBitmap", false);
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
                int width = matrix.Width;
                int height = matrix.Height;

                using (var bmp = new Bitmap(width, height))
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            bmp.SetPixel(x, y, matrix[x, y] ? Color.Black : Color.White);
                        }
                    }

                    string directory = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        logger.Info($"Creating directory: {directory}");
                        Directory.CreateDirectory(directory);
                    }

                    bmp.Save(filePath, ImageFormat.Png);
                    logger.Debug($"Image saved successfully: {filePath} ({width}x{height})");
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Failed to save image to {filePath}", ex);
                throw;
            }
        }
    }
}
