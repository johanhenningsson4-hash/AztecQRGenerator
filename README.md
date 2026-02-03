# AztecQRGenerator

A .NET Framework 4.7.2 library for generating QR codes and Aztec codes from Base64-encoded data, supporting PNG, JPEG, and BMP output. Includes robust error handling, logging, and a flexible API for both in-memory and file-based barcode generation.

[![CI - Windows Build and Test](https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions/workflows/windows-msbuild-test.yml/badge.svg)](https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions/workflows/windows-msbuild-test.yml)

## Features
- Standards-compliant QR and Aztec code generation
- Output as Bitmap or directly to file
- PNG, JPEG, BMP support (PNG recommended)
- Configurable size and error correction
- ISO-8859-1 encoding for full Latin-1 support
- Thread-safe logging
- .NET Framework 4.7.2 compatible
- **Logging activated by default**
- **Comprehensive CI/CD with GitHub Actions**
- **Code coverage reporting and test automation**

## Quick Start

### Generate QR Code as Bitmap
```csharp
using AztecQR;
using System.Drawing;

var generator = new QRGenerator();
Bitmap qrCode = generator.GenerateQRCodeAsBitmap(
    qrstring: "SGVsbG8gV29ybGQh",  // Base64 encoded data
    lCorrection: 2,                 // Error correction level (0-10)
    lPixelDensity: 300              // Size in pixels
);
// Use qrCode (e.g., display in UI)
qrCode.Dispose();
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

## Requirements
- .NET Framework 4.7.2 or higher
- ZXing.Net 0.16.11 (NuGet)
- System.Drawing (included)

## Best Practices
- Use PNG for all production barcodes
- Always dispose Bitmaps to prevent memory leaks
- Validate Base64 input before generation
- Test scanability with real barcode scanners

## Logging
- Logger is a thread-safe singleton
- Logs are written to `{AppData}/AztecQRGenerator/Logs/AztecQR_yyyyMMdd.log`
- Set log level: `Logger.Instance.SetMinimumLogLevel(LogLevel.Debug);`
- **Logging is activated by default at Debug level**

## Continuous Integration

The project includes a robust CI/CD pipeline with GitHub Actions that automatically:
- Builds the solution using MSBuild (compatible with classic .NET Framework projects)
- Runs all unit tests with comprehensive reporting
- Generates code coverage reports (HTML + Cobertura formats)  
- Uploads test results and coverage artifacts
- Automatically retries with test shims if dependencies are missing

### Local Development

Use the provided PowerShell script for consistent local builds:

```powershell
.\build_and_test.ps1 -Solution AztecQRGenerator.sln -Configuration Debug
```

The script automatically:
- Locates MSBuild and vstest.console via Visual Studio installer
- Restores packages and builds the solution
- Discovers and runs all test assemblies
- Generates TRX and coverage files under `TestResults/`

### Test Shims (Optional)

For environments without test framework assemblies, enable conditional test shims by defining the `USE_TEST_SHIMS` compilation symbol:

```
msbuild AztecQRGenerator.sln /t:Restore,Build /p:Configuration=Debug;DefineConstants=USE_TEST_SHIMS /m
```

Alternatively, use the script's built-in flag:

```powershell
.\build_and_test.ps1 -UseTestShims
```

The CI workflow automatically enables shims as a fallback if the initial build fails.

## Version History

### Version 1.4.0 (Current)
- **CI/CD**: Added comprehensive GitHub Actions workflow with MSBuild + vstest
- **Testing**: Enhanced test coverage with proper NUnit assertions and conditional MSTest shims
- **Build Tools**: Added `build_and_test.ps1` script for consistent local/CI builds
- **Code Coverage**: Integrated ReportGenerator for HTML and Cobertura coverage reports
- **Code Quality**: Fixed numerous StyleCop warnings and improved code consistency
- **Documentation**: Updated README with CI information and development workflows

### Version 1.3.0
- Fixed: Removed duplicate file save operations in GenerateQRBitmap() and GenerateAztecBitmap()
- Now generates only one file instead of two identical files
- Improved efficiency and reduced disk I/O

### Version 1.2.0
- Added support for multiple image formats (PNG, JPEG, BMP)
- Added GenerateQRCodeToFile() and GenerateAztecCodeToFile() with format selection
- Added GenerateQRCodeAsBitmap() and GenerateAztecCodeAsBitmap() for in-memory generation
- Comprehensive logging support
- Full backward compatibility maintained

## License
MIT License

## Author
Johan Henningsson

---
For more details, see the NuGet and test project READMEs in the repository.

