# AztecQRGenerator.Core

A robust .NET Framework 4.7.2 library for generating QR codes and Aztec codes from Base64-encoded data. Built for production use with comprehensive error handling, logging, and flexible output options.

[![CI - Windows Build and Test](https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions/workflows/windows-msbuild-test.yml/badge.svg)](https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions/workflows/windows-msbuild-test.yml)

## ✨ Features

- **Standards-compliant** QR and Aztec code generation
- **Multiple output formats**: PNG, JPEG, BMP (PNG recommended)
- **Flexible API**: Generate as Bitmap or save directly to file
- **Configurable**: Size and error correction levels
- **ISO-8859-1 encoding** for full Latin-1 character support
- **Thread-safe logging** with configurable levels
- **Production-ready** with comprehensive error handling
- **CI/CD tested** with automated test coverage

## 🚀 Quick Start

### Generate QR Code as Bitmap
```csharp
using AztecQR;
using System.Drawing;

var generator = new QRGenerator();
using (Bitmap qrCode = generator.GenerateQRCodeAsBitmap(
    qrstring: "SGVsbG8gV29ybGQh",  // Base64 encoded data
    lCorrection: 2,                 // Error correction level
    lPixelDensity: 300              // Size in pixels
)) {
    // Use qrCode (display, save, etc.)
}
```

### Save Aztec Code to File
```csharp
using AztecQR;
using System.Drawing.Imaging;

var generator = new AztecGenerator();
bool success = generator.GenerateAztecCodeToFile(
    aztecstring: "SGVsbG8gV29ybGQh",
    lCorrection: 2,
    lPixelDensity: 300,
    filePath: "aztec.png",
    format: ImageFormat.Png
);
```

### Save with Timestamp (Convenience Method)
```csharp
var generator = new QRGenerator();
// Saves to Documents/AztecQRGenerator/Output/ with timestamp
bool success = generator.GenerateQRBitmap(
    "SGVsbG8gV29ybGQh", 2, 300
);
```

## 📋 Requirements

- **.NET Framework 4.7.2** or higher
- **ZXing.Net 0.16.11** (automatically installed)
- **System.Drawing** (included with .NET Framework)

## 🎯 Best Practices

- **Use PNG format** for production barcodes (best quality)
- **Always dispose Bitmaps** to prevent memory leaks
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
