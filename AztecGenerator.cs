/*=========================================================================================
'  Copyright(C):    Johan Henningsson 
'  
'  Author :         Johan Henningsson
'==========================================================================================*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using ZXing;
using ZXing.Aztec;
using ZXing.Common;

namespace AztecQR
{
    public class AztecGenerator
    {
        public void GenerateAztecBitmap(int lTaNmbrqr, string aztecstring, int lCorrection, int lPixelDensity)
        {
            // Decode Base64 string
            byte[] data = Convert.FromBase64String(aztecstring);

            // Create AztecWriter and generate BitMatrix
            var writer = new AztecWriter();
            var hints = new Dictionary<EncodeHintType, object>
            {
                { EncodeHintType.MARGIN, 0 }
            };

            BitMatrix matrix = writer.encode(
                Encoding.GetEncoding("ISO-8859-1").GetString(data),
                BarcodeFormat.AZTEC,
                lPixelDensity,
                lPixelDensity,
                hints
            );

            // Save as PNG
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string fileName = $"AztecCode_{timestamp}.png";
            SaveBitMatrixAsPng(matrix, fileName);

            // Save scaled version
            string scaledFileName = $"AztecCode_Scaled_{timestamp}.png";
            SaveBitMatrixAsPng(matrix, scaledFileName);
        }

        private void SaveBitMatrixAsPng(BitMatrix matrix, string filePath)
        {
            int width = matrix.Width;
            int height = matrix.Height;
            using (var bmp = new Bitmap(width, height))
            {
                for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    bmp.SetPixel(x, y, matrix[x, y] ? Color.Black : Color.White);

                bmp.Save(filePath, ImageFormat.Png);
            }
        }
    }
}
