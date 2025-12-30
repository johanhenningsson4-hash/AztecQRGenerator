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

namespace AztecQR
{
    public class QRGenerator
    {
        public void GenerateQRBitmap(int lTaNmbrqr, string qrstring, int lCorrection, int lPixelDensity)
        {
            // Decode Base64 string
            byte[] data = Convert.FromBase64String(qrstring);

            // Create QRCodeWriter and generate BitMatrix
            var writer = new QRCodeWriter();
            var hints = new Dictionary<EncodeHintType, object>
            {
                { EncodeHintType.MARGIN, 0 }
            };

            BitMatrix matrix = writer.encode(
                Encoding.GetEncoding("ISO-8859-1").GetString(data),
                BarcodeFormat.QR_CODE,
                lPixelDensity,
                lPixelDensity,
                hints
            );

            // Save as PNG
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string fileName = $"QRCode_{timestamp}.png";
            SaveBitMatrixAsPng(matrix, fileName);

            // Save scaled version
            string scaledFileName = $"QRCode_Scaled_{timestamp}.png";
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
