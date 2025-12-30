# AztecQRGenerator

A .NET Framework-based Windows Forms application for generating QR codes and Aztec codes from Base64-encoded data. The application supports both GUI mode and command-line mode for batch operations, with comprehensive logging and error handling.

## Features

- **QR Code Generation**: Generate QR codes from Base64-encoded data
- **Aztec Code Generation**: Generate Aztec codes from Base64-encoded data
- **Dual Mode Operation**: 
  - GUI mode for interactive use
  - Command-line mode for batch processing and automation
- **Customizable Output**:
  - Configurable pixel density (size)
  - Adjustable error correction levels
  - PNG output format
- **ISO-8859-1 Encoding**: Supports Latin-1 character encoding
- **Comprehensive Logging**: File-based logging with automatic rotation and multiple log levels
- **Robust Error Handling**: Input validation, exception handling, and meaningful error messages

## Requirements

- .NET Framework 4.7.2 or higher
- C# 7.3
- Dependencies:
  - ZXing.Net (barcode generation library)
  - System.Drawing (image processing)

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/AztecQRGenerator.git
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

## Usage

### GUI Mode

Run the application without arguments to launch the Windows Forms interface:

```bash
AztecQRGenerator.exe
```

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

The application generates two PNG files for each code:
- `QRCode_{timestamp}.png` or `AztecCode_{timestamp}.png` - Original size
- `QRCode_Scaled_{timestamp}.png` or `AztecCode_Scaled_{timestamp}.png` - Scaled version

Timestamps are formatted as: `yyyyMMddHHmmssfff`

## Project Structure

```
AztecQRGenerator/
??? QRGenerator.cs         # QR code generation logic with error handling
??? AztecGenerator.cs      # Aztec code generation logic with error handling
??? Logger.cs              # Logging utility with file rotation
??? Program.cs             # Main entry point, CLI handler, and global exception handling
??? AztecQR.cs            # Windows Forms main form
??? AztecQR.Designer.cs   # Form designer code
??? Properties/           # Assembly and resource files
??? Logs/                 # Auto-generated log files (created at runtime)
```

## Technical Details

### QR Code Generation
- Uses ZXing.QrCode.QRCodeWriter
- Supports ISO-8859-1 character encoding
- Zero margin configuration for compact output
- Configurable pixel density and error correction
- Full input validation and error handling
- Detailed logging at each processing stage

### Aztec Code Generation
- Uses ZXing.Aztec.AztecWriter
- Supports ISO-8859-1 character encoding
- Zero margin configuration for compact output
- Configurable pixel density and error correction
- Full input validation and error handling
- Detailed logging at each processing stage

### Logger Implementation
- Thread-safe singleton pattern
- Automatic log file rotation (5 MB threshold)
- Daily log file organization
- Multiple log levels (Debug, Info, Warning, Error)
- Exception details with inner exceptions and stack traces
- Method entry/exit tracking for debugging

## License

Copyright (C) Johan Henningsson

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

## Known Issues

- The `outputfile` parameter in command-line mode is currently not used; files are saved with auto-generated timestamped names
- Both generators create two files (original and scaled) with the same content

## Future Enhancements

- Honor the outputfile parameter in CLI mode
- Add support for different image formats (JPEG, BMP, etc.)
- Implement different scaling options
- Add support for color customization
- Batch processing from file input
- Configurable log levels via configuration file
- Log viewer utility
