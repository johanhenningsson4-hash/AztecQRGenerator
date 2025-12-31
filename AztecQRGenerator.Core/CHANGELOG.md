# Changelog

All notable changes to AztecQRGenerator.Core will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.2] - 2025-12-31

### Fixed
- Added README.md to NuGet package for better discoverability on NuGet.org
- Package now displays comprehensive documentation when viewing on NuGet.org
- Fixed package metadata for improved NuGet.org display

### Technical
- Created `.nuspec` file for better control over package contents
- README file size increased package from 10.5 KB to 16 KB

---

## [1.2.1] - 2025-12-31

### Changed
- Removed unused `lTaNmbrqr` parameter from `GenerateAztecBitmap()` method
- Removed unused `lTaNmbrqr` parameter from `GenerateQRBitmap()` method
- Updated AssemblyInfo.cs to match NuGet package metadata

### Fixed
- Improved code consistency across generator classes
- Better alignment with clean code principles

### Breaking Changes
- ?? **Method signatures changed:**
  - `AztecGenerator.GenerateAztecBitmap(int lTaNmbrqr, string aztecstring, int lCorrection, int lPixelDensity)` 
    ? `GenerateAztecBitmap(string aztecstring, int lCorrection, int lPixelDensity)`
  - `QRGenerator.GenerateQRBitmap(int lTaNmbrqr, string qrstring, int lCorrection, int lPixelDensity)` 
    ? `GenerateQRBitmap(string qrstring, int lCorrection, int lPixelDensity)`

### Migration Guide
**Before (v1.2.0):**
```csharp
var aztecGenerator = new AztecGenerator();
aztecGenerator.GenerateAztecBitmap(1, base64String, 2, 300);

var qrGenerator = new QRGenerator();
qrGenerator.GenerateQRBitmap(1, base64String, 2, 300);
```

**After (v1.2.1):**
```csharp
var aztecGenerator = new AztecGenerator();
aztecGenerator.GenerateAztecBitmap(base64String, 2, 300);

var qrGenerator = new QRGenerator();
qrGenerator.GenerateQRBitmap(base64String, 2, 300);
```

---

## [1.2.0] - 2025-XX-XX

### Added
- Support for multiple image formats (PNG, JPEG, BMP)
- `GenerateQRCodeToFile()` method with format selection
- `GenerateAztecCodeToFile()` method with format selection
- `GenerateQRCodeAsBitmap()` for in-memory QR code generation
- `GenerateAztecCodeAsBitmap()` for in-memory Aztec code generation
- Comprehensive logging infrastructure
- Detailed error handling and validation
- XML documentation comments for all public methods

### Changed
- Refactored bitmap conversion logic to reduce code duplication
- Improved error messages and exception handling
- Enhanced logging with method entry/exit tracking

### Fixed
- Better memory management with proper resource disposal
- More robust Base64 validation
- Consistent parameter validation across all methods

---

## [1.1.0] - 2025-XX-XX (Assumed Previous Version)

### Added
- Basic QR code generation
- Basic Aztec code generation
- ISO-8859-1 encoding support
- PNG file output

### Dependencies
- ZXing.Net 0.16.9
- .NET Framework 4.7.2

---

## [1.0.0] - 2025-XX-XX (Initial Release)

### Added
- Initial project structure
- Core barcode generation functionality

---

## Versioning Strategy

This project follows [Semantic Versioning](https://semver.org/):
- **MAJOR** version (X.0.0): Incompatible API changes
- **MINOR** version (0.X.0): New functionality (backward compatible)
- **PATCH** version (0.0.X): Bug fixes (backward compatible)

## Support

- **GitHub Issues:** https://github.com/johanhenningsson4-hash/AztecQRGenerator/issues
- **Documentation:** https://github.com/johanhenningsson4-hash/AztecQRGenerator#readme
- **NuGet Package:** https://www.nuget.org/packages/AztecQRGenerator.Core/

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
