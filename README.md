# AztecQRGenerator

A .NET Framework 4.7.2 library for generating QR codes and Aztec codes from Base64-encoded data, supporting PNG, JPEG, and BMP output. Includes robust error handling, logging, and a flexible API for both in-memory and file-based barcode generation.

## Features
- Standards-compliant QR and Aztec code generation
- Output as Bitmap or directly to file
- PNG, JPEG, BMP support (PNG recommended)
- Configurable size and error correction
- ISO-8859-1 encoding for full Latin-1 support
- Thread-safe logging
- .NET Framework 4.7.2 compatible
- **Logging activated by default**

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

## License
MIT License

## Author
Johan Henningsson

---
For more details, see the NuGet and test project READMEs in the repository.

## Enabling test shims in CI

Some test projects include lightweight shims for MSTest attributes to allow compilation in environments that do not have the test framework assemblies available. These shims are disabled by default to avoid conflicting with the real test framework packages when they are present.

To enable the shims in CI, define the `USE_TEST_SHIMS` compilation symbol when building. Example MSBuild invocation:

```
msbuild AztecQRGenerator.sln /t:Restore,Build /p:Configuration=Debug;DefineConstants=USE_TEST_SHIMS /m
```

If you use the included `build_and_test.ps1` script, pass the `DefineConstants` property through MSBuild by editing the script or by setting an environment variable consumed by your CI pipeline. If you want, I can update the workflow to automatically enable the shims when package restore fails.

