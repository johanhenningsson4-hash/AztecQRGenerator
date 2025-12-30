/*=========================================================================================
'  Copyright(C):    Johan Henningsson 
'  
'  Author :         Johan Henningsson
'==========================================================================================*/

using System;
using System.Windows.Forms;

namespace AztecQR
{
    static class Program
    {
        private static readonly Logger logger = Logger.Instance;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Set up global exception handlers
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Application.ThreadException += OnThreadException;

            logger.Info("=== Application Starting ===");
            logger.Info($"Version: {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}");
            logger.Info($"Command line args count: {args.Length}");

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Check if command line arguments are provided for batch generation
                if (args.Length > 0)
                {
                    logger.Info("Running in command-line mode");
                    int exitCode = RunCommandLineMode(args);
                    logger.Info($"Application exiting with code: {exitCode}");
                    Environment.Exit(exitCode);
                }
                else
                {
                    // Run the Windows Forms application
                    logger.Info("Running in GUI mode");
                    Application.Run(new MainEMVReaderBin());
                    logger.Info("GUI application closed normally");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Fatal error in Main", ex);
                MessageBox.Show(
                    $"A fatal error occurred: {ex.Message}\n\nPlease check the log file for details.",
                    "Fatal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Environment.Exit(1);
            }
            finally
            {
                logger.Info("=== Application Shutdown ===");
            }
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            logger.Error("Unhandled exception occurred", ex);
            
            if (e.IsTerminating)
            {
                logger.Error("Application is terminating due to unhandled exception");
            }
        }

        private static void OnThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            logger.Error("Thread exception occurred", e.Exception);
            
            MessageBox.Show(
                $"An unexpected error occurred: {e.Exception.Message}\n\nPlease check the log file for details.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private static int RunCommandLineMode(string[] args)
        {
            try
            {
                logger.Info($"Processing command line arguments: {string.Join(" ", args)}");

                if (args.Length < 3)
                {
                    string usage = "Usage: AztecQRGenerator.exe <type> <data> <outputfile> [size] [errorCorrection]";
                    logger.Warning("Insufficient command line arguments");
                    Console.WriteLine(usage);
                    Console.WriteLine("  type: QR or AZTEC");
                    Console.WriteLine("  data: Base64 encoded string");
                    Console.WriteLine("  outputfile: Output file path (supports .png, .jpg, .bmp)");
                    Console.WriteLine("  size: Pixel density (optional, default: 200)");
                    Console.WriteLine("  errorCorrection: Error correction level (optional, default: 2)");
                    return 1;
                }

                string type = args[0].ToUpper();
                string data = args[1];
                string outputFile = args[2];
                
                int size = 200;
                if (args.Length > 3)
                {
                    if (!int.TryParse(args[3], out size) || size <= 0)
                    {
                        logger.Warning($"Invalid size parameter: {args[3]}. Using default: 200");
                        Console.WriteLine("Warning: Invalid size parameter. Using default: 200");
                        size = 200;
                    }
                }

                int errorCorrection = 2;
                if (args.Length > 4)
                {
                    if (!int.TryParse(args[4], out errorCorrection) || errorCorrection < 0)
                    {
                        logger.Warning($"Invalid error correction parameter: {args[4]}. Using default: 2");
                        Console.WriteLine("Warning: Invalid error correction parameter. Using default: 2");
                        errorCorrection = 2;
                    }
                }

                // Determine image format from file extension
                System.Drawing.Imaging.ImageFormat format = GetImageFormatFromExtension(outputFile);
                string formatName = GetFormatName(format);

                logger.Info($"Parameters - Type: {type}, Output: {outputFile}, Format: {formatName}, Size: {size}, Error Correction: {errorCorrection}");

                bool success = false;
                switch (type)
                {
                    case "QR":
                        logger.Info($"Generating QR code to file: {outputFile}");
                        Console.WriteLine($"Generating QR Code ({formatName})...");
                        var qrGen = new QRGenerator();
                        success = qrGen.GenerateQRCodeToFile(data, errorCorrection, size, outputFile, format);
                        if (success)
                        {
                            Console.WriteLine($"QR Code saved successfully: {outputFile}");
                            logger.Info($"QR Code generation completed successfully: {outputFile}");
                        }
                        break;

                    case "AZTEC":
                        logger.Info($"Generating Aztec code to file: {outputFile}");
                        Console.WriteLine($"Generating Aztec Code ({formatName})...");
                        var aztecGen = new AztecGenerator();
                        success = aztecGen.GenerateAztecCodeToFile(data, errorCorrection, size, outputFile, format);
                        if (success)
                        {
                            Console.WriteLine($"Aztec Code saved successfully: {outputFile}");
                            logger.Info($"Aztec Code generation completed successfully: {outputFile}");
                        }
                        break;

                    default:
                        logger.Warning($"Unknown code type specified: {type}");
                        Console.WriteLine($"Unknown type: {type}. Use QR or AZTEC");
                        return 1;
                }

                return success ? 0 : 1;
            }
            catch (ArgumentException ex)
            {
                logger.Error("Invalid argument provided", ex);
                Console.WriteLine($"Invalid argument: {ex.Message}");
                return 2;
            }
            catch (FormatException ex)
            {
                logger.Error("Format error in input data", ex);
                Console.WriteLine($"Format error: {ex.Message}");
                return 3;
            }
            catch (System.IO.IOException ex)
            {
                logger.Error("File I/O error", ex);
                Console.WriteLine($"File I/O error: {ex.Message}");
                return 4;
            }
            catch (InvalidOperationException ex)
            {
                logger.Error("Code generation failed", ex);
                Console.WriteLine($"Generation failed: {ex.Message}");
                return 5;
            }
            catch (Exception ex)
            {
                logger.Error("Unexpected error in command line mode", ex);
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Check the log file for more details.");
                return 99;
            }
        }

        /// <summary>
        /// Determines the image format from file extension
        /// </summary>
        private static System.Drawing.Imaging.ImageFormat GetImageFormatFromExtension(string filePath)
        {
            string extension = System.IO.Path.GetExtension(filePath).ToLowerInvariant();
            
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    logger.Info("Output format detected: JPEG");
                    return System.Drawing.Imaging.ImageFormat.Jpeg;
                
                case ".bmp":
                    logger.Info("Output format detected: BMP");
                    return System.Drawing.Imaging.ImageFormat.Bmp;
                
                case ".png":
                default:
                    logger.Info("Output format detected: PNG (default)");
                    return System.Drawing.Imaging.ImageFormat.Png;
            }
        }

        /// <summary>
        /// Gets a friendly name for the image format
        /// </summary>
        private static string GetFormatName(System.Drawing.Imaging.ImageFormat format)
        {
            if (format.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                return "JPEG";
            if (format.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
                return "BMP";
            return "PNG";
        }
    }
}
