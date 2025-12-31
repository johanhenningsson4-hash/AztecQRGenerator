/*=========================================================================================
'  Copyright(C):    Johan Henningsson 
'  
'  Author :         Johan Henningsson
'  Script to create icon.png for NuGet package
'==========================================================================================*/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace AztecQR
{
    /// <summary>
    /// Utility class to create a project icon
    /// Run this once to generate icon.png
    /// </summary>
    public class IconCreator
    {
        public static void CreateProjectIcon(string outputPath = "icon.png")
        {
            const int size = 128;
            const int margin = 10;
            
            using (Bitmap bmp = new Bitmap(size, size))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                
                // Background - gradient
                using (LinearGradientBrush bgBrush = new LinearGradientBrush(
                    new Rectangle(0, 0, size, size),
                    Color.FromArgb(240, 240, 245),
                    Color.FromArgb(250, 250, 255),
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(bgBrush, 0, 0, size, size);
                }
                
                // Draw a stylized QR code pattern
                int cellSize = 6;
                int gridSize = (size - 2 * margin) / cellSize;
                int offsetX = margin;
                int offsetY = margin;
                
                // Create a pattern that looks like a QR code
                Random rand = new Random(12345); // Fixed seed for consistency
                
                using (SolidBrush blackBrush = new SolidBrush(Color.FromArgb(20, 20, 40)))
                using (SolidBrush accentBrush = new SolidBrush(Color.FromArgb(65, 105, 225))) // Royal blue
                {
                    for (int y = 0; y < gridSize; y++)
                    {
                        for (int x = 0; x < gridSize; x++)
                        {
                            // Create QR-like pattern with position markers
                            bool isPositionMarker = 
                                (x < 3 && y < 3) ||           // Top-left
                                (x >= gridSize - 3 && y < 3) || // Top-right
                                (x < 3 && y >= gridSize - 3);   // Bottom-left
                            
                            bool isCornerSquare = 
                                (x < 3 && y < 3 && (x == 0 || y == 0 || x == 2 || y == 2)) ||
                                (x >= gridSize - 3 && y < 3 && (x == gridSize - 1 || y == 0 || x == gridSize - 3 || y == 2)) ||
                                (x < 3 && y >= gridSize - 3 && (x == 0 || y == gridSize - 1 || x == 2 || y == gridSize - 3));
                            
                            bool isCenterSquare = 
                                (x == 1 && y == 1) ||
                                (x == gridSize - 2 && y == 1) ||
                                (x == 1 && y == gridSize - 2);
                            
                            bool shouldFill = false;
                            SolidBrush brush = blackBrush;
                            
                            if (isPositionMarker)
                            {
                                shouldFill = isCornerSquare || isCenterSquare;
                                if (shouldFill) brush = accentBrush;
                            }
                            else
                            {
                                // Random pattern for the rest, but weighted
                                double probability = 0.5;
                                if (Math.Abs(x - gridSize / 2) < 2 && Math.Abs(y - gridSize / 2) < 2)
                                {
                                    probability = 0.7; // Denser in center
                                }
                                shouldFill = rand.NextDouble() < probability;
                            }
                            
                            if (shouldFill)
                            {
                                g.FillRectangle(
                                    brush,
                                    offsetX + x * cellSize,
                                    offsetY + y * cellSize,
                                    cellSize - 1,
                                    cellSize - 1
                                );
                            }
                        }
                    }
                }
                
                // Add a subtle border
                using (Pen borderPen = new Pen(Color.FromArgb(200, 200, 210), 2))
                {
                    g.DrawRectangle(borderPen, 1, 1, size - 3, size - 3);
                }
                
                // Save the icon
                bmp.Save(outputPath, ImageFormat.Png);
                Console.WriteLine($"Icon created successfully: {outputPath}");
            }
        }
    }
}
