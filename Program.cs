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
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Check if command line arguments are provided for batch generation
            if (args.Length > 0)
            {
                RunCommandLineMode(args);
            }
            else
            {
                // Run the Windows Forms application
                Application.Run(new MainEMVReaderBin());
            }
        }

        private static void RunCommandLineMode(string[] args)
        {
            try
            {
                if (args.Length < 3)
                {
                    Console.WriteLine("Usage: AztecQRGenerator.exe <type> <data> <outputfile> [size] [errorCorrection]");
                    Console.WriteLine("  type: QR or AZTEC");
                    Console.WriteLine("  data: Base64 encoded string");
                    Console.WriteLine("  outputfile: Output PNG file path");
                    Console.WriteLine("  size: Pixel density (optional, default: 200)");
                    Console.WriteLine("  errorCorrection: Error correction level (optional, default: 2)");
                    return;
                }

                string type = args[0].ToUpper();
                string data = args[1];
                string outputFile = args[2];
                int size = args.Length > 3 ? int.Parse(args[3]) : 200;
                int errorCorrection = args.Length > 4 ? int.Parse(args[4]) : 2;

                switch (type)
                {
                    case "QR":
                        var qrGen = new QRGenerator();
                        qrGen.GenerateQRBitmap(1, data, errorCorrection, size);
                        Console.WriteLine($"QR Code generated successfully");
                        break;

                    case "AZTEC":
                        var aztecGen = new AztecGenerator();
                        aztecGen.GenerateAztecBitmap(1, data, errorCorrection, size);
                        Console.WriteLine($"Aztec Code generated successfully");
                        break;

                    default:
                        Console.WriteLine($"Unknown type: {type}. Use QR or AZTEC");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
