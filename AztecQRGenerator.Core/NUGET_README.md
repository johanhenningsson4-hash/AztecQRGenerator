# AztecQRGenerator.Core

A robust .NET Framework 4.7.2 library for generating QR codes and Aztec codes from Base64-encoded data. Built for production use with comprehensive error handling, logging, and flexible output options.

[![CI - Windows Build and Test](https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions/workflows/windows-msbuild-test.yml/badge.svg)](https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions/workflows/windows-msbuild-test.yml)

## ✨ Features

- **Standards-compliant** QR and Aztec code generation
- **Multiple input types**: Direct text, byte arrays, and Base64 strings
- **Multiple output formats**: PNG, JPEG, BMP (PNG recommended)
- **Flexible API**: Generate as Bitmap or save directly to file
- **Async/await support**: Modern asynchronous APIs with cancellation support
- **Batch processing**: Generate multiple codes with progress reporting
- **Configurable**: Size and error correction levels
- **Multiple encodings**: UTF-8, Unicode, ISO-8859-1, and custom encodings
- **Thread-safe logging** with configurable levels
- **Production-ready** with comprehensive error handling
- **CI/CD tested** with automated test coverage

## 🚀 Quick Start

### Generate QR Code from Text (New & Recommended)
```csharp
using AztecQR;
using System.Drawing;

var generator = new QRGenerator();

// Direct text input (new!)
using (Bitmap qrCode = generator.GenerateQRCodeFromText("Hello, World!"))
{
    // Use qrCode (display, save, etc.)
}

// Async version
using (Bitmap qrCode = await generator.GenerateQRCodeFromTextAsync("Hello, World!"))
{
    // Use qrCode (display, save, etc.)
}
```

### Generate QR Code from Byte Array (New)
```csharp
// Binary data input
byte[] data = System.IO.File.ReadAllBytes("myfile.pdf");
using (Bitmap qrCode = generator.GenerateQRCodeFromBytes(data))
{
    // Use qrCode
}

// Async version
using (Bitmap qrCode = await generator.GenerateQRCodeFromBytesAsync(data))
{
    // Use qrCode
}
```

### Generate QR Code from Base64 (Original API)
```csharp
// Base64 encoded data (backwards compatible)
string base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes("Hello, World!"));
using (Bitmap qrCode = generator.GenerateQRCodeAsBitmap(base64Data, 2, 300))
{
    // Use qrCode
}
```

### Save Directly to File (All Input Types)
```csharp
// Text to file
bool success = await generator.GenerateQRCodeFromTextToFileAsync(
    "Hello, World!", "output.png", ImageFormat.Png);

// Bytes to file
bool success = await generator.GenerateQRCodeFromBytesToFileAsync(
    data, "output.png", ImageFormat.Png);
```

### Batch Processing with Progress Reporting
```csharp
var requests = new[]
{
    new QRRequest("SGVsbG8gV29ybGQh", 2, 300),
    new QRRequest("QWRkaXRpb25hbCBkYXRh", 2, 300),  
    new QRRequest("TW9yZSB0ZXN0IGRhdGE=", 2, 300)
};

var progress = new Progress<BatchProgress>(p => 
    Console.WriteLine($"Progress: {p.PercentComplete:F1}%"));

var bitmaps = await generator.GenerateBatchAsync(requests, progress);
foreach (var bitmap in bitmaps)
{
    // Process bitmap
    bitmap.Dispose();
}
```

## 📋 Requirements

- **.NET Framework 4.7.2** or higher
- **ZXing.Net 0.16.11** (automatically installed)
- **System.Drawing** (included with .NET Framework)

## 🎯 Best Practices

- **Use async methods** for better application responsiveness
- **Use PNG format** for production barcodes (best quality)
- **Always dispose Bitmaps** to prevent memory leaks  
- **Handle cancellation** properly in long-running operations
- **Use batch processing** for multiple QR codes to improve performance
- **Validate Base64 input** before generation
- **Test scanability** with real barcode scanners
- **Handle exceptions** appropriately in your application

## 🔧 Configuration

### Error Correction Levels
- `0` = Low (~7% recovery)
- `1` = Medium (~15% recovery)  
- `2` = Quartile (~25% recovery) - **Default**
- `3` = High (~30% recovery)

### Pixel Density
- Minimum: `50` pixels
- Recommended: `300-500` pixels
- Maximum: Limited by available memory

## 📝 Logging

The library includes built-in logging that's **activated by default**:

```csharp
// Configure logging level
Logger.Instance.SetMinimumLogLevel(LogLevel.Info);

// Get log file location
string logPath = Logger.Instance.GetLogFilePath();
```

**Log location**: `%APPDATA%\AztecQRGenerator\Logs\AztecQR_yyyyMMdd.log`

## 🚀 What's New in v1.4.0

- **🔄 CI/CD Pipeline**: Comprehensive GitHub Actions workflow
- **🧪 Enhanced Testing**: Proper NUnit assertions + conditional MSTest shims
- **⚙️ Build Automation**: Added `build_and_test.ps1` script  
- **📊 Code Coverage**: HTML + Cobertura reporting
- **✨ Code Quality**: StyleCop cleanup + consistency improvements
- **📖 Documentation**: Updated with CI information and workflows
- **⚡ Async/Await Support**: Full async API with cancellation tokens
- **📦 Batch Processing**: Generate multiple codes with progress reporting
- **🔧 Better Error Handling**: Structured exceptions for async operations

## 🔗 Links

- **GitHub Repository**: [AztecQRGenerator](https://github.com/johanhenningsson4-hash/AztecQRGenerator)
- **Issues & Support**: [GitHub Issues](https://github.com/johanhenningsson4-hash/AztecQRGenerator/issues)
- **CI/CD Status**: [GitHub Actions](https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions)
- **Documentation**: [README.md](https://github.com/johanhenningsson4-hash/AztecQRGenerator/blob/main/README.md)

## 📄 License

MIT License - See [LICENSE](https://github.com/johanhenningsson4-hash/AztecQRGenerator/blob/main/LICENSE) file for details.

## 👤 Author

**Johan Henningsson**

---

*Built with ❤️ for the .NET community*
