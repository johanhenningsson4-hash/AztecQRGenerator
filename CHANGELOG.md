# Changelog

All notable changes to AztecQRGenerator will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.3.0] - 2025-01-31

### Added
- **Permission-Safe File Locations**: Application now uses safe, user-accessible directories
  - Logs stored in `%LocalAppData%\AztecQRGenerator\Logs\`
  - Output files saved to `%UserProfile%\Documents\AztecQRGenerator\Output\` by default
  - Automatic fallback hierarchy if primary location fails
- **Logger.GetLogFilePath()**: New method to retrieve current log file location
- **Smart File Saving**: Automatic fallback to Documents folder when write permissions are denied
- **Test Script**: Added `Test-PermissionFix.ps1` for automated testing

### Changed
- **Logger Location**: Changed from application directory to user AppData (no admin privileges required)
- **File Save Behavior**: Relative paths now automatically use Documents folder
- **Error Messages**: More informative messages indicating actual save location

### Fixed
- **Permission Issues**: Application no longer requires administrator privileges
- **CLI Output File**: Now properly honors the `outputfile` parameter (functionality was already present but undocumented)
- **Access Denied Errors**: Graceful handling with automatic fallback to safe locations
- **Directory Creation**: Better error handling when creating output directories

### Security
- **No Admin Required**: Application runs safely with standard user permissions
- **Safe Defaults**: All file operations use user-accessible locations by default

## [1.2.0] - 2024-12-30

### Added
- **Multiple Image Formats**: Support for PNG, JPEG, and BMP output formats
- **GenerateQRCodeToFile()**: New method with explicit format parameter
- **GenerateAztecCodeToFile()**: New method with explicit format parameter
- **Format Selection**: Detect format from file extension in CLI mode
- **IMAGE_FORMAT_GUIDE.md**: Comprehensive guide for format selection

### Changed
- **SaveBitmap()**: Refactored to support multiple formats
- **CLI Mode**: Now supports .png, .jpg, and .bmp extensions

## [1.1.0] - 2024-12-15

### Added
- **GenerateQRCodeAsBitmap()**: Return QR code as Bitmap object without file saving
- **GenerateAztecCodeAsBitmap()**: Return Aztec code as Bitmap object without file saving
- **In-Memory Generation**: Full control over bitmap lifecycle for developers
- **USAGE_EXAMPLES.md**: Comprehensive usage documentation
- **IMPLEMENTATION_SUMMARY.md**: Technical implementation details

### Changed
- **Code Refactoring**: Extracted shared methods for better maintainability
- **Backward Compatibility**: All existing methods work unchanged

## [1.0.0] - 2024-12-01

### Added
- Initial release
- **GUI Mode**: Windows Forms interface for interactive code generation
- **CLI Mode**: Command-line interface for batch processing
- **QR Code Generation**: Standards-compliant QR codes
- **Aztec Code Generation**: Compact 2D barcodes
- **ISO-8859-1 Encoding**: Latin-1 character support
- **Comprehensive Logging**: File-based logging with automatic rotation
- **Error Handling**: Robust input validation and exception handling
- **Dual Output**: Saves both original and scaled versions (PNG)

### Dependencies
- .NET Framework 4.7.2
- ZXing.Net 0.16.11
- System.Drawing

---

## Version History Summary

| Version | Release Date | Key Features |
|---------|--------------|--------------|
| 1.3.0   | 2025-01-31   | Permission fixes, safe file locations |
| 1.2.0   | 2024-12-30   | Multiple image formats (PNG/JPEG/BMP) |
| 1.1.0   | 2024-12-15   | In-memory bitmap generation |
| 1.0.0   | 2024-12-01   | Initial release |

## Upgrade Notes

### Upgrading from 1.2.x to 1.3.0
- **Log files** will now be in `%LocalAppData%\AztecQRGenerator\Logs\` instead of application directory
- **Output files** with relative paths will go to Documents folder
- No code changes required - fully backward compatible
- Recommended: Inform users about new file locations

### Upgrading from 1.1.x to 1.2.0
- No breaking changes
- New methods available for format selection
- Existing code continues to work unchanged

### Upgrading from 1.0.x to 1.1.0
- No breaking changes
- New bitmap-returning methods available
- Existing file-saving methods work as before

## Known Issues

### All Versions
- Legacy file-saving methods (`GenerateQRBitmap`, `GenerateAztecBitmap`) create two identical files
- JPEG format not recommended for barcodes due to compression artifacts

## Future Enhancements

### Planned for 1.4.0
- [ ] Color customization (foreground/background colors)
- [ ] Batch processing from CSV/JSON files
- [ ] Custom margins support
- [ ] XML documentation comments on all public methods

### Under Consideration
- [ ] Byte array return methods for web APIs
- [ ] .NET Core / .NET 6+ version
- [ ] Additional barcode formats (Data Matrix, PDF417)
- [ ] GUI improvements and themes
- [ ] Configuration file support

## Support

For issues, questions, or feature requests:
- **GitHub Issues**: https://github.com/johanhenningsson4-hash/AztecQRGenerator/issues
- **Documentation**: https://github.com/johanhenningsson4-hash/AztecQRGenerator#readme

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

Copyright © 2025 Johan Olof Henningsson
