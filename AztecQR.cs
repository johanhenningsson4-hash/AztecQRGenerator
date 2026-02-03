/*=========================================================================================
'  Copyright(C):    Johan Henningsson 
'  
'  Author :         Johan Henningsson
'==========================================================================================*/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using ZXing;
using ZXing.Common;
using BitMatrix = ZXing.Common.BitMatrix;

namespace AztecQR
{
    public partial class MainEMVReaderBin : Form
    {
        private readonly Logger logger = Logger.Instance;
        private Bitmap currentBitmap;
        private string lastGeneratedType;

        public MainEMVReaderBin()
        {
            InitializeComponent();
            logger.Info("Main form initialized");
            
            // Set up event handlers
            this.Load += MainEMVReaderBin_Load;
            this.FormClosing += MainEMVReaderBin_FormClosing;
        }

        private void MainEMVReaderBin_Load(object sender, EventArgs e)
        {
            logger.Info("Main form loaded");
            UpdateStatusLabel("Ready");
        }

        private void MainEMVReaderBin_FormClosing(object sender, FormClosingEventArgs e)
        {
            logger.Info("Main form closing");
            if (currentBitmap != null)
            {
                currentBitmap.Dispose();
            }
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            logger.LogMethodEntry("MainEMVReaderBin", "buttonGenerate_Click");

            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(textInputString.Text))
                {
                    UpdateStatusLabel("Error: Please enter a Base64 string");
                    MessageBox.Show("Please enter a Base64 string to encode.", "Input Required", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    logger.Warning("Generate clicked with empty input");
                    return;
                }

                // Get parameters
                string inputData = textInputString.Text.Trim();
                int size = (int)numericSize.Value;
                int errorCorrection = (int)numericErrorCorrection.Value;
                bool isAztec = radioAztec.Checked;
                string codeType = isAztec ? "Aztec" : "QR";

                logger.Info($"Generating {codeType} code - Size: {size}, Error Correction: {errorCorrection}");
                UpdateStatusLabel($"Generating {codeType} code...");
                Application.DoEvents();

                // Clear previous image
                if (currentBitmap != null)
                {
                    currentBitmap.Dispose();
                    currentBitmap = null;
                }
                picturePreview.Image = null;

                // Generate the code
                BitMatrix matrix = GenerateCode(inputData, size, errorCorrection, isAztec);

                if (matrix != null)
                {
                    // Convert to bitmap and display
                    currentBitmap = ConvertBitMatrixToBitmap(matrix);
                    picturePreview.Image = currentBitmap;
                    lastGeneratedType = codeType;

                    buttonSaveAs.Enabled = true;
                    UpdateStatusLabel($"{codeType} code generated successfully ({matrix.Width}x{matrix.Height} pixels)");
                    logger.Info($"{codeType} code generated and displayed successfully");
                }
            }
            catch (ArgumentException ex)
            {
                logger.Error("Invalid input for code generation", ex);
                UpdateStatusLabel("Error: Invalid input");
                MessageBox.Show($"Invalid input: {ex.Message}", "Input Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                logger.Error("Error generating code", ex);
                UpdateStatusLabel("Error: Generation failed");
                MessageBox.Show($"Error generating code: {ex.Message}\n\nCheck the log file for details.", 
                    "Generation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                logger.LogMethodExit("MainEMVReaderBin", "buttonGenerate_Click");
            }
        }

        private BitMatrix GenerateCode(string base64Data, int size, int errorCorrection, bool isAztec)
        {
            try
            {
                // Validate and decode Base64 string
                byte[] data = Convert.FromBase64String(base64Data);
                logger.Debug($"Base64 decoded successfully, data length: {data.Length} bytes");

                string decodedString = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(data);

                var hints = new System.Collections.Generic.Dictionary<EncodeHintType, object>
                {
                    { EncodeHintType.MARGIN, 0 }
                };

                BitMatrix matrix;

                if (isAztec)
                {
                    var writer = new ZXing.Aztec.AztecWriter();
                    matrix = writer.encode(decodedString, BarcodeFormat.AZTEC, size, size, hints);
                    logger.Debug("Aztec code matrix generated");
                }
                else
                {
                    var writer = new ZXing.QrCode.QRCodeWriter();
                    matrix = writer.encode(decodedString, BarcodeFormat.QR_CODE, size, size, hints);
                    logger.Debug("QR code matrix generated");
                }

                return matrix;
            }
            catch (System.FormatException ex)
            {
                logger.Error("Failed to decode Base64 string", ex);
                throw new ArgumentException("Invalid Base64 string format. Please check your input.", ex);
            }
            catch (Exception ex)
            {
                logger.Error("Failed to generate code matrix", ex);
                throw;
            }
        }

        private Bitmap ConvertBitMatrixToBitmap(BitMatrix matrix)
        {
            logger.Debug($"Converting BitMatrix to Bitmap - Size: {matrix.Width}x{matrix.Height}");

            int width = matrix.Width;
            int height = matrix.Height;
            Bitmap bmp = new Bitmap(width, height);

            try
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        bmp.SetPixel(x, y, matrix[x, y] ? Color.Black : Color.White);
                    }
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

        private void buttonSaveAs_Click(object sender, EventArgs e)
        {
            logger.LogMethodEntry("MainEMVReaderBin", "buttonSaveAs_Click");

            if (currentBitmap == null)
            {
                logger.Warning("Save clicked but no image available");
                UpdateStatusLabel("Error: No image to save");
                return;
            }

            try
            {
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    string defaultFileName = $"{lastGeneratedType}Code_{DateTime.Now:yyyyMMddHHmmss}.png";
                    saveDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp|All Files|*.*";
                    saveDialog.Title = "Save Barcode Image";
                    saveDialog.FileName = defaultFileName;
                    saveDialog.DefaultExt = "png";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = saveDialog.FileName;
                        string extension = Path.GetExtension(filePath).ToLower();

                        ImageFormat format = ImageFormat.Png;
                        switch (extension)
                        {
                            case ".jpg":
                            case ".jpeg":
                                format = ImageFormat.Jpeg;
                                break;
                            case ".bmp":
                                format = ImageFormat.Bmp;
                                break;
                            default:
                                format = ImageFormat.Png;
                                break;
                        }

                        logger.Info($"Saving image to: {filePath} (Format: {format})");
                        currentBitmap.Save(filePath, format);
                        
                        UpdateStatusLabel($"Image saved: {Path.GetFileName(filePath)}");
                        logger.Info($"Image saved successfully to: {filePath}");

                        MessageBox.Show($"Image saved successfully to:\n{filePath}", "Save Successful", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error saving image", ex);
                UpdateStatusLabel("Error: Failed to save image");
                MessageBox.Show($"Error saving image: {ex.Message}", "Save Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                logger.LogMethodExit("MainEMVReaderBin", "buttonSaveAs_Click");
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            logger.Info("Clear button clicked");

            try
            {
                // Clear input
                textInputString.Clear();

                // Clear image
                if (currentBitmap != null)
                {
                    currentBitmap.Dispose();
                    currentBitmap = null;
                }
                picturePreview.Image = null;

                // Reset options to defaults
                radioQR.Checked = true;
                numericSize.Value = 300;
                numericErrorCorrection.Value = 2;

                // Disable save button
                buttonSaveAs.Enabled = false;

                UpdateStatusLabel("Ready");
                logger.Info("Form cleared");
            }
            catch (Exception ex)
            {
                logger.Error("Error clearing form", ex);
            }
        }

        private void UpdateStatusLabel(string message)
        {
            try
            {
                statusLabel.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
            }
            catch (Exception ex)
            {
                logger.Error("Error updating status label", ex);
            }
        }
    }
}
