# AztecQRGenerator

A .NET Framework-based Windows Forms application and library for generating QR codes and Aztec codes from Base64-encoded data. The application supports both GUI mode and command-line mode for batch operations, with comprehensive logging and error handling. Now includes API methods to return barcodes as Bitmap objects for flexible integration.

## Features

- **QR Code Generation**: Generate QR codes from Base64-encoded data
- **Aztec Code Generation**: Generate Aztec codes from Base64-encoded data
- **Flexible API**:
  - Return as `Bitmap` objects for in-memory use
  - Save directly to files with automatic naming
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

## Requirements

- .NET Framework 4.7.2 or higher
- C# 7.3
- Dependencies:
  - ZXing.Net 0.16.11 (barcode generation library)
  - System.Drawing (image processing)

## Installation

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

#### Generate and Save to File

```csharp
var qrGenerator = new QRGenerator();

// Generates and saves two PNG files automatically with timestamp
bool success = qrGenerator.GenerateQRBitmap(
    lTaNmbrqr: 1,
    qrstring: "SGVsbG8gV29ybGQh",
    lCorrection: 2,
    lPixelDensity: 300
);
```

**See [USAGE_EXAMPLES.md](USAGE_EXAMPLES.md) for more detailed examples and best practices.**

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

**`GenerateQRBitmap(int lTaNmbrqr, string qrstring, int lCorrection, int lPixelDensity)`**
- Returns: `bool` - True if successful
- Throws: `ArgumentException`, `IOException`, `InvalidOperationException`
- Automatically saves two PNG files with timestamps

### AztecGenerator Class

#### Methods

**`GenerateAztecCodeAsBitmap(string aztecstring, int lCorrection, int lPixelDensity)`**
- Returns: `Bitmap` - The generated Aztec code as a bitmap object
- Throws: `ArgumentException`, `InvalidOperationException`
- Use this for in-memory generation without file I/O

**`GenerateAztecBitmap(int lTaNmbrqr, string aztecstring, int lCorrection, int lPixelDensity)`**
- Returns: `bool` - True if successful
- Throws: `ArgumentException`, `IOException`, `InvalidOperationException`
- Automatically saves two PNG files with timestamps

### Parameters

- **qrstring / aztecstring**: Base64 encoded string to be encoded in the barcode
- **lCorrection**: Error correction level (0-10, default: 2)
  - Higher values = more error correction = larger code size
- **lPixelDensity**: Size of the generated code in pixels
  - Must be greater than 0
  - Typical values: 200-500 pixels

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

When using file-saving methods, the application generates two PNG files for each code:
- `QRCode_{timestamp}.png` or `AztecCode_{timestamp}.png` - Original size
- `QRCode_Scaled_{timestamp}.png` or `AztecCode_Scaled_{timestamp}.png` - Scaled version

Timestamps are formatted as: `yyyyMMddHHmmssfff`

When using bitmap-returning methods, no files are created automatically - you have full control.

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

Copyright (c) 2025 Johan Olof Henningsson

## Author

Johan Henningsson

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

## Troubleshooting

### Check the Logs
If you encounter any issues, check the log files in the `Logs` directory for detailed error information.

### Common Issues

1. **Invalid Base64 String**: Ensure your input data is properly Base64-encoded
2. **File Access Denied**: Check write permissions in the application directory
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

## Known Issues

- The `outputfile` parameter in command-line mode is currently not used; files are saved with auto-generated timestamped names
- Both generators create two files (original and scaled) with the same content when using file-saving methods

## Recent Updates

### Version 1.1 (Latest)
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
- **[IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)** - Technical implementation details and architecture

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/johanhenningsson4-hash/AztecQRGenerator).
