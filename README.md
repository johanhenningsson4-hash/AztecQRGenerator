# AztecQRGenerator

![CI Build](https://github.com/johanhenningsson4-hash/AztecQRGenerator/workflows/CI%20Build%20and%20Test/badge.svg)
![Code Quality](https://github.com/johanhenningsson4-hash/AztecQRGenerator/workflows/Code%20Quality%20Analysis/badge.svg)
![NuGet Version](https://img.shields.io/nuget/v/AztecQRGenerator.Core.svg)
![Tests](https://img.shields.io/badge/tests-58%20passing-brightgreen)
![License](https://img.shields.io/github/license/johanhenningsson4-hash/AztecQRGenerator.svg)

A .NET Framework-based Windows Forms application and library for generating QR codes and Aztec codes from Base64-encoded data. The application supports both GUI mode and command-line mode for batch operations, with comprehensive logging and error handling. Now includes API methods to return barcodes as Bitmap objects for flexible integration.

**? Now with automated testing and CI/CD pipeline! Every commit is tested with 58 comprehensive unit tests.**

## Features

- **QR Code Generation**: Generate QR codes from Base64-encoded data
- **Aztec Code Generation**: Generate Aztec codes from Base64-encoded data
- **Multiple Image Formats**: Support for PNG, JPEG, and BMP output formats
- **Flexible API**:
  - Return as `Bitmap` objects for in-memory use
  - Save directly to files with automatic naming
  - Save to files with custom format selection
- **Dual Mode Operation**: 
  - GUI mode for interactive use with live preview
  - Command-line mode for batch processing and automation
- **Customizable Output**:
  - Configurable pixel density (size)
  - Adjustable error correction levels
  - PNG, JPG, and BMP output formats (GUI mode)
- **ISO-8859-1 Encoding**: Supports Latin-1 character encoding
- **Comprehensive Logging**: File-based logging with automatic rotation and multiple log levels
- **Robust Error Handling**: Input validation, exception handling, and meaningful error messages
- **? Automated Testing**: 58 unit tests covering all public APIs
- **?? CI/CD Pipeline**: Automated build, test, and deployment to NuGet

## Requirements

- .NET Framework 4.7.2 or higher
- C# 7.3
- Dependencies:
  - ZXing.Net 0.16.9 (barcode generation library)
  - System.Drawing (image processing)

## Quality Assurance

### Automated Testing ?
- **58 comprehensive unit tests**
  - 25 tests for QRGenerator
  - 25 tests for AztecGenerator
  - 8 tests for Logger
- **Test Framework**: NUnit 3.14
- **Coverage**: All public APIs tested
- **Automated execution**: Tests run on every commit

### CI/CD Pipeline ??
- **Continuous Integration**: Automated build and test on every push
- **Automated NuGet Publishing**: Package published automatically on release
- **Weekly Security Scans**: Automated code quality and security analysis
- **Test Reporting**: Visual test results for every build

See [TESTING_CICD_SETUP.md](TESTING_CICD_SETUP.md) for complete testing and CI/CD documentation.

## Installation

### As a NuGet Package (Recommended for Developers)

The core library is available as a NuGet package for easy integration into your projects:

```bash
# Package Manager Console
Install-Package AztecQRGenerator.Core

# .NET CLI
dotnet add package AztecQRGenerator.Core
```

**Package Features:**
- QR and Aztec code generation
- Multiple image format support (PNG, JPEG, BMP)
- In-memory bitmap generation
- Comprehensive error handling and logging
- No GUI dependencies - pure library

See the [AztecQRGenerator.Core](AztecQRGenerator.Core/) directory for more information.

### As a Standalone Application

1. Clone the repository:
   ```bash
   git clone https://github.com/johanhenningsson4-hash/AztecQRGenerator.git
   ```

2. Restore NuGet packages:
   ```bash
   nuget restore
   ```

3. Build the project:
   ```bash
   msbuild AztecQRGenerator.csproj /p:Configuration=Release
   ```

## Usage

### GUI Mode

Run the application without arguments to launch the Windows Forms interface:

```bash
AztecQRGenerator.exe
```

**GUI Features:**
- Enter Base64 data in multiline text box
- Choose between QR Code or Aztec Code
- Adjust size (50-1000 pixels) and error correction (0-10)
- Live preview of generated barcode
- Save as PNG, JPG, or BMP with custom filename
- Clear button to reset form
- Status bar with operation feedback

### API / Library Usage

#### Generate and Return as Bitmap

**QR Code:**
```csharp
using AztecQR;
using System.Drawing;

var qrGenerator = new QRGenerator();

// Generate QR code as Bitmap (in-memory, no file saving)
Bitmap qrBitmap = qrGenerator.GenerateQRCodeAsBitmap(
    qrstring: "SGVsbG8gV29ybGQh",  // Base64 encoded data
    lCorrection: 2,                 // Error correction level (0-10)
    lPixelDensity: 300              // Size in pixels
);

// Use the bitmap (e.g., display in PictureBox)
pictureBox.Image = qrBitmap;

// Remember to dispose when done
// qrBitmap.Dispose();
```

**Aztec Code:**
```csharp
using AztecQR;
using System.Drawing;

var aztecGenerator = new AztecGenerator();

// Generate Aztec code as Bitmap (in-memory, no file saving)
Bitmap aztecBitmap = aztecGenerator.GenerateAztecCodeAsBitmap(
    aztecstring: "SGVsbG8gV29ybGQh",  // Base64 encoded data
    lCorrection: 2,                    // Error correction level (0-10)
    lPixelDensity: 300                 // Size in pixels
);

// Use the bitmap
pictureBox.Image = aztecBitmap;

// Remember to dispose when done
// aztecBitmap.Dispose();
```

#### Generate and Save to File with Format Selection

**QR Code - Save as PNG, JPEG, or BMP:**
```csharp
using AztecQR;
using System.Drawing.Imaging;

var qrGenerator = new QRGenerator();

// Save as PNG (recommended for barcodes)
bool success = qrGenerator.GenerateQRCodeToFile(
    qrstring: "SGVsbG8gV29ybGQh",
    lCorrection: 2,
    lPixelDensity: 300,
    filePath: "qrcode.png",
    format: ImageFormat.Png
);

// Save as JPEG (smaller file, lower quality - not recommended for barcodes)
success = qrGenerator.GenerateQRCodeToFile(
    qrstring: "SGVsbG8gV29ybGQh",
    lCorrection: 2,
    lPixelDensity: 300,
    filePath: "qrcode.jpg",
    format: ImageFormat.Jpeg
);

// Save as BMP (lossless but large file size)
success = qrGenerator.GenerateQRCodeToFile(
    qrstring: "SGVsbG8gV29ybGQh",
    lCorrection: 2,
    lPixelDensity: 300,
    filePath: "qrcode.bmp",
    format: ImageFormat.Bmp
);
```

**Aztec Code - Save with Format:**
```csharp
using AztecQR;
using System.Drawing.Imaging;

var aztecGenerator = new AztecGenerator();

bool success = aztecGenerator.GenerateAztecCodeToFile(
    aztecstring: "SGVsbG8gV29ybGQh",
    lCorrection: 2,
    lPixelDensity: 300,
    filePath: "azteccode.png",
    format: ImageFormat.Png
);
```

#### Generate and Save to File (Legacy Method)

```csharp
var qrGenerator = new QRGenerator();

// Generates and saves a single PNG file with timestamp
bool success = qrGenerator.GenerateQRBitmap(
    qrstring: "SGVsbG8gV29ybGQh",
    lCorrection: 2,
    lPixelDensity: 300
);
```

**See [USAGE_EXAMPLES.md](USAGE_EXAMPLES.md) for more detailed examples and best practices.**
**See [IMAGE_FORMAT_GUIDE.md](IMAGE_FORMAT_GUIDE.md) for comprehensive format selection guidance.**

### Command-Line Mode

For batch processing and automation, use command-line arguments:

```bash
AztecQRGenerator.exe <type> <data> <outputfile> [size] [errorCorrection]
```

#### Parameters:
- `type`: Code type - either `QR` or `AZTEC`
- `data`: Base64-encoded string to encode
- `outputfile`: Output PNG file path (currently generates timestamped files)
- `size` (optional): Pixel density, default is 200
- `errorCorrection` (optional): Error correction level, default is 2

#### Exit Codes:
- `0`: Success
- `1`: Invalid arguments or unknown code type
- `2`: Invalid argument format
- `3`: Format error in input data
- `4`: File I/O error
- `5`: Code generation failed
- `99`: Unexpected error

#### Examples:

Generate a QR code:
```bash
AztecQRGenerator.exe QR "SGVsbG8gV29ybGQ=" output.png 300 2
```

Generate an Aztec code:
```bash
AztecQRGenerator.exe AZTEC "SGVsbG8gV29ybGQ=" output.png 250 2
```

## API Reference

### QRGenerator Class

#### Methods

**`GenerateQRCodeAsBitmap(string qrstring, int lCorrection, int lPixelDensity)`**
- Returns: `Bitmap` - The generated QR code as a bitmap object
- Throws: `ArgumentException`, `InvalidOperationException`
- Use this for in-memory generation without file I/O

**`GenerateQRCodeToFile(string qrstring, int lCorrection, int lPixelDensity, string filePath, ImageFormat format)`** ? NEW
- Returns: `bool` - True if successful
- Parameters:
  - `format`: `ImageFormat.Png`, `ImageFormat.Jpeg`, or `ImageFormat.Bmp`
- Throws: `ArgumentException`, `ArgumentNullException`, `IOException`, `InvalidOperationException`
- Use this to save directly to file with format selection

**`GenerateQRBitmap(string qrstring, int lCorrection, int lPixelDensity)`**
- Returns: `bool` - True if successful
- Throws: `ArgumentException`, `IOException`, `InvalidOperationException`
- Saves a single PNG file with timestamp in the Documents folder

### AztecGenerator Class

#### Methods

**`GenerateAztecCodeAsBitmap(string aztecstring, int lCorrection, int lPixelDensity)`**
- Returns: `Bitmap` - The generated Aztec code as a bitmap object
- Throws: `ArgumentException`, `InvalidOperationException`
- Use this for in-memory generation without file I/O

**`GenerateAztecCodeToFile(string aztecstring, int lCorrection, int lPixelDensity, string filePath, ImageFormat format)`** ? NEW
- Returns: `bool` - True if successful
- Parameters:
  - `format`: `ImageFormat.Png`, `ImageFormat.Jpeg`, or `ImageFormat.Bmp`
- Throws: `ArgumentException`, `ArgumentNullException`, `IOException`, `InvalidOperationException`
- Use this to save directly to file with format selection

**`GenerateAztecBitmap(string aztecstring, int lCorrection, int lPixelDensity)`**
- Returns: `bool` - True if successful
- Throws: `ArgumentException`, `IOException`, `InvalidOperationException`
- Saves a single PNG file with timestamp in the Documents folder

### Parameters

- **qrstring / aztecstring**: Base64 encoded string to be encoded in the barcode
- **lCorrection**: Error correction level (0-10, default: 2)
  - Higher values = more error correction = larger code size
- **lPixelDensity**: Size of the generated code in pixels
  - Must be greater than 0
  - Typical values: 200-500 pixels
- **filePath**: Full or relative path for output file (including extension)
- **format**: Image format from `System.Drawing.Imaging.ImageFormat`
  - `ImageFormat.Png` - Recommended for barcodes (lossless, moderate size)
  - `ImageFormat.Jpeg` - Smaller files but lossy (not recommended for barcodes)
  - `ImageFormat.Bmp` - Lossless but very large files

### Image Format Recommendations

| Format | File Size | Quality | Recommended For |
|--------|-----------|---------|-----------------|
| PNG | Moderate | Excellent | ? QR/Aztec codes (recommended) |
| JPEG | Small | Fair | ?? Not recommended (compression artifacts) |
| BMP | Very Large | Excellent | Legacy systems only |

## Logging

The application automatically creates detailed log files in the `Logs` directory:

### Log Files
- Location: `{ApplicationDirectory}\Logs\`
- Format: `AztecQR_yyyyMMdd.log`
- Automatic rotation when file exceeds 5 MB

### Log Levels
- **Debug**: Detailed diagnostic information (method entry/exit, data processing steps)
- **Info**: General informational messages (operation status, file creation)
- **Warning**: Warning messages (invalid parameters with fallback values)
- **Error**: Error messages with full exception details and stack traces

### Log Entry Format
```
[yyyy-MM-dd HH:mm:ss.fff] [LEVEL] Message
```

Example:
```
[2024-01-15 14:23:45.123] [INFO] Generating QR code - ID: 1, Size: 200, Correction: 2
[2024-01-15 14:23:45.456] [INFO] QR code saved successfully: QRCode_20240115142345456.png
```

## Error Handling

The application includes comprehensive error handling:

### Input Validation
- Validates Base64 string format
- Checks pixel density (must be > 0)
- Validates error correction levels
- Verifies file paths and creates directories as needed

### Exception Handling
- **ArgumentException**: Invalid input parameters
- **FormatException**: Malformed Base64 data
- **IOException**: File system errors
- **InvalidOperationException**: Code generation failures

### Global Exception Handlers
- Unhandled exception handler for application domain
- Thread exception handler for UI threads
- All exceptions are logged with full stack traces

## Output Files

When using the `GenerateQRBitmap()` or `GenerateAztecBitmap()` methods, a single PNG file is created:
- `QRCode_{timestamp}.png` or `AztecCode_{timestamp}.png`

Timestamps are formatted as: `yyyyMMddHHmmssfff`

Files are saved to the Documents folder by default: `Documents\AztecQRGenerator\Output\`

When using bitmap-returning methods (`GenerateQRCodeAsBitmap()`, `GenerateAztecCodeAsBitmap()`), no files are created automatically - you have full control.

## Project Structure

```
AztecQRGenerator/
??? QRGenerator.cs         # QR code generation logic with bitmap API
??? AztecGenerator.cs      # Aztec code generation logic with bitmap API
??? Logger.cs              # Logging utility with file rotation
??? Program.cs             # Main entry point, CLI handler, exception handling
??? AztecQR.cs            # Windows Forms main form
??? AztecQR.Designer.cs   # Form designer code
??? AztecQR.resx          # Form resources
??? Properties/           # Assembly and resource files
??? Logs/                 # Auto-generated log files (created at runtime)
??? AztecQRGenerator.Core/ # NuGet package project (class library)
?   ??? QRGenerator.cs    # Core generator (no GUI)
?   ??? AztecGenerator.cs # Core generator (no GUI)
?   ??? Logger.cs         # Core logger
?   ??? NUGET_README.md   # Package documentation
?   ??? NUGET_PUBLISHING_GUIDE.md # Publishing instructions
??? README.md             # This file
??? LICENSE               # MIT License
??? USAGE_EXAMPLES.md     # Detailed usage examples
??? IMPLEMENTATION_SUMMARY.md  # Technical implementation details
```

## Technical Details

### QR Code Generation
- Uses ZXing.QrCode.QRCodeWriter
- Supports ISO-8859-1 character encoding
- Zero margin configuration for compact output
- Configurable pixel density and error correction
- Full input validation and error handling
- Detailed logging at each processing stage
- Memory-efficient bitmap generation

### Aztec Code Generation
- Uses ZXing.Aztec.AztecWriter
- Supports ISO-8859-1 character encoding
- Zero margin configuration for compact output
- Configurable pixel density and error correction
- Full input validation and error handling
- Detailed logging at each processing stage
- Memory-efficient bitmap generation

### Logger Implementation
- Thread-safe singleton pattern
- Automatic log file rotation (5 MB threshold)
- Daily log file organization
- Multiple log levels (Debug, Info, Warning, Error)
- Exception details with inner exceptions and stack traces
- Method entry/exit tracking for debugging

## License

MIT License - See [LICENSE](LICENSE) file for details

Copyright (c) 2026 Johan Olof Henningsson

## Author

Johan Henningsson

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

### Project Components

1. **AztecQRGenerator** - Windows Forms application with GUI and CLI
2. **AztecQRGenerator.Core** - NuGet package library (no GUI dependencies)

### Building the NuGet Package

See [AztecQRGenerator.Core/NUGET_PUBLISHING_GUIDE.md](AztecQRGenerator.Core/NUGET_PUBLISHING_GUIDE.md) for instructions on building and publishing the NuGet package.

## Troubleshooting

### Check the Logs
If you encounter any issues, check the log files in the `Logs` directory for detailed error information.

### Common Issues

1. **Invalid Base64 String**: Ensure your input data is properly Base64-encoded
2. **File Access Denied**: Check write permissions. Files are saved to `Documents\AztecQRGenerator\Output\` by default
3. **Invalid Parameters**: Review the command-line syntax and parameter values
4. **Memory Issues**: When using bitmap methods, ensure you dispose of bitmaps properly

### Memory Management Best Practices

```csharp
// Option 1: Using statement (recommended)
using (Bitmap qr = generator.GenerateQRCodeAsBitmap(data, 2, 300))
{
    // Use bitmap here
    pictureBox.Image = (Bitmap)qr.Clone(); // Clone if persisting beyond using block
}

// Option 2: Manual disposal
Bitmap qr = generator.GenerateQRCodeAsBitmap(data, 2, 300);
try
{
    // Use bitmap here
}
finally
{
    qr?.Dispose();
}
```

## Recent Updates

### Version 1.3.0 (Latest) - January 2026 ??
- ?? **Added 58 comprehensive unit tests** covering all public APIs
- ?? **Implemented CI/CD pipeline** with GitHub Actions
  - Automated build and test on every push
  - Automated NuGet publishing on release
  - Weekly security and code quality scans
- ?? **Automated test reporting** with visual results
- ?? **Enhanced documentation** for testing and deployment
- ? **Zero breaking changes** - fully backward compatible
- ?? **Production-ready quality assurance**

### Version 1.2.3 - January 2026
- ?? **Bug Fix:** Removed duplicate file save operations in `GenerateQRBitmap()` and `GenerateAztecBitmap()`
- ? Now generates only **one file** instead of two identical files
- ? Improved efficiency and reduced disk I/O by 50%
- ?? Available on NuGet: `Install-Package AztecQRGenerator.Core -Version 1.3.0`

### Version 1.2
- ? Added support for multiple image formats (PNG, JPEG, BMP)
- ? Added `GenerateQRCodeToFile()` method with format selection
- ? Added `GenerateAztecCodeToFile()` method with format selection
- ? Enhanced flexibility for different output requirements
- ? Comprehensive format selection guide added

### Version 1.1
- ? Added `GenerateQRCodeAsBitmap()` method for in-memory QR generation
- ? Added `GenerateAztecCodeAsBitmap()` method for in-memory Aztec generation
- ? Refactored internal code for better reusability
- ? Maintained full backward compatibility
- ? Added comprehensive usage examples and documentation

### Version 1.0
- Initial release with GUI and CLI modes
- File-based generation for QR and Aztec codes
- Comprehensive logging system

## Future Enhancements

- Honor the outputfile parameter in CLI mode
- Add support for color customization (background and foreground colors)
- Implement byte array return methods for web APIs
- Add support for custom margins
- Batch processing from CSV/JSON file input
- Configurable log levels via configuration file
- Log viewer utility
- Unit test suite

## Documentation

- **[USAGE_EXAMPLES.md](USAGE_EXAMPLES.md)** - Comprehensive code examples and usage patterns
- **[IMAGE_FORMAT_GUIDE.md](IMAGE_FORMAT_GUIDE.md)** - Detailed guide for image format selection
- **[IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)** - Technical implementation details and architecture
- **[TESTING_CICD_SETUP.md](TESTING_CICD_SETUP.md)** - Complete testing and CI/CD infrastructure guide
- **[PUBLISH_TO_NUGET.md](PUBLISH_TO_NUGET.md)** - NuGet publishing guide
- **[CICD_VERIFICATION_REPORT.md](CICD_VERIFICATION_REPORT.md)** - CI/CD setup verification report

## For Developers

### Running Tests

```powershell
# Build solution
msbuild AztecQRGenerator.sln /p:Configuration=Release

# Run tests
.\packages\NUnit.ConsoleRunner.3.16.3\tools\nunit3-console.exe `
  AztecQRGenerator.Tests\bin\Release\AztecQRGenerator.Tests.dll
```

Or use Visual Studio Test Explorer:
1. Open Test Explorer (Test ? Test Explorer)
2. Click "Run All Tests"
3. View results and coverage

### CI/CD Pipeline

The project uses GitHub Actions for automated workflows:

- **CI Build**: Runs on every push to `main` or `develop`
  - Builds solution
  - Runs all 58 tests
  - Generates test reports
  - Uploads artifacts

- **NuGet Publish**: Triggered by GitHub releases
  - Builds Core library
  - Creates NuGet package
  - Publishes to NuGet.org automatically

- **Code Quality**: Weekly scans
  - Security analysis
  - Code quality checks
  - SARIF results

See [CICD_VERIFICATION_REPORT.md](CICD_VERIFICATION_REPORT.md) for complete CI/CD documentation.

## Support
