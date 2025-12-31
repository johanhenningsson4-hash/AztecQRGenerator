# AztecQRGenerator.Core

A powerful .NET Framework library for generating QR codes and Aztec codes from Base64-encoded data with support for multiple image formats.

## Features

- ? **QR Code Generation** - Standards-compliant QR codes
- ? **Aztec Code Generation** - Compact 2D barcodes
- ? **Multiple Formats** - PNG, JPEG, and BMP output
- ? **Flexible API** - Return as Bitmap or save to file
- ? **Customizable** - Configurable size and error correction
- ? **ISO-8859-1 Encoding** - Latin-1 character support
- ? **Production Ready** - Comprehensive error handling and logging

## Installation

Install via NuGet Package Manager:

```bash
Install-Package AztecQRGenerator.Core
```

Or via .NET CLI:

```bash
dotnet add package AztecQRGenerator.Core
```

## Quick Start

### Generate QR Code as Bitmap

```csharp
using AztecQR;
using System.Drawing;

var generator = new QRGenerator();

// Generate QR code in memory
Bitmap qrCode = generator.GenerateQRCodeAsBitmap(
    qrstring: "SGVsbG8gV29ybGQh",  // Base64 encoded data
    lCorrection: 2,                 // Error correction level (0-10)
    lPixelDensity: 300              // Size in pixels
);

// Use the bitmap (e.g., display in UI)
pictureBox.Image = qrCode;

// Remember to dispose when done
qrCode.Dispose();
```

### Save QR Code to File

```csharp
using AztecQR;
using System.Drawing.Imaging;

var generator = new QRGenerator();

// Save as PNG (recommended)
bool success = generator.GenerateQRCodeToFile(
    qrstring: "SGVsbG8gV29ybGQh",
    lCorrection: 2,
    lPixelDensity: 300,
    filePath: "qrcode.png",
    format: ImageFormat.Png
);
```

### Generate Aztec Code

```csharp
using AztecQR;
using System.Drawing;

var generator = new AztecGenerator();

// Generate Aztec code in memory
Bitmap aztecCode = generator.GenerateAztecCodeAsBitmap(
    aztecstring: "SGVsbG8gV29ybGQh",
    lCorrection: 2,
    lPixelDensity: 300
);

// Or save directly to file
generator.GenerateAztecCodeToFile(
    aztecstring: "SGVsbG8gV29ybGQh",
    lCorrection: 2,
    lPixelDensity: 300,
    filePath: "aztec.png",
    format: ImageFormat.Png
);
```

## Supported Image Formats

| Format | Extension | Quality | File Size | Recommended For |
|--------|-----------|---------|-----------|-----------------|
| **PNG** | .png | Excellent (lossless) | Moderate | ? QR/Aztec codes (default) |
| **JPEG** | .jpg, .jpeg | Fair (lossy) | Small | ?? Not recommended for barcodes |
| **BMP** | .bmp | Excellent (lossless) | Very Large | ?? Legacy systems only |

> **Recommendation:** Use PNG format for barcodes. JPEG compression can cause artifacts that reduce scanability.

## API Reference

### QRGenerator Class

#### Methods

**`GenerateQRCodeAsBitmap(string qrstring, int lCorrection, int lPixelDensity)`**
- Returns: `Bitmap` - QR code as a bitmap object
- Use for: In-memory generation, UI display

**`GenerateQRCodeToFile(string qrstring, int lCorrection, int lPixelDensity, string filePath, ImageFormat format)`**
- Returns: `bool` - True if successful
- Use for: Saving directly to file with format selection

### AztecGenerator Class

#### Methods

**`GenerateAztecCodeAsBitmap(string aztecstring, int lCorrection, int lPixelDensity)`**
- Returns: `Bitmap` - Aztec code as a bitmap object
- Use for: In-memory generation, UI display

**`GenerateAztecCodeToFile(string aztecstring, int lCorrection, int lPixelDensity, string filePath, ImageFormat format)`**
- Returns: `bool` - True if successful
- Use for: Saving directly to file with format selection

### Parameters

- **qrstring / aztecstring**: Base64 encoded string to encode in the barcode
- **lCorrection**: Error correction level (0-10, default: 2) - Higher = more redundancy
- **lPixelDensity**: Size in pixels (must be > 0, typical: 200-500)
- **filePath**: Output file path (including extension)
- **format**: `ImageFormat.Png`, `ImageFormat.Jpeg`, or `ImageFormat.Bmp`

## Advanced Usage

### Multiple Formats from One Generation

```csharp
using (Bitmap qr = generator.GenerateQRCodeAsBitmap("SGVsbG8=", 2, 300))
{
    qr.Save("output.png", ImageFormat.Png);
    qr.Save("output.jpg", ImageFormat.Jpeg);
    qr.Save("output.bmp", ImageFormat.Bmp);
}
```

### Error Handling

```csharp
try
{
    var generator = new QRGenerator();
    bool success = generator.GenerateQRCodeToFile(
        "SGVsbG8gV29ybGQh", 2, 300, "output.png", ImageFormat.Png
    );
    
    if (success)
    {
        Console.WriteLine("QR code generated successfully!");
    }
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid input: {ex.Message}");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Generation failed: {ex.Message}");
}
catch (IOException ex)
{
    Console.WriteLine($"File error: {ex.Message}");
}
```

### Logging

The library includes built-in logging to help with debugging:

```csharp
// Set minimum log level (default: Info)
Logger.Instance.SetMinimumLogLevel(LogLevel.Debug);

// Logs are written to: {AppDirectory}/Logs/AztecQR_yyyyMMdd.log
```

## Requirements

- .NET Framework 4.7.2 or higher
- System.Drawing (included)
- ZXing.Net 0.16.9 (automatically installed)

## Performance

- **Generation Speed**: ~10-50ms per barcode (depends on size)
- **Memory Usage**: ~1-5 MB per generation (proportional to pixel density)
- **Thread Safety**: Logger is thread-safe, generators are not (create per thread)

## Best Practices

1. ? **Use PNG format** for all production barcodes
2. ? **Dispose bitmaps** properly to prevent memory leaks
3. ? **Validate Base64** input before generation
4. ? **Test scanability** with real barcode scanners
5. ?? **Avoid JPEG** unless file size is absolutely critical
6. ?? **Use appropriate sizes** (300-500px recommended for printing)

## Examples

### Windows Forms Application

```csharp
private void GenerateQRCode()
{
    var generator = new QRGenerator();
    string base64Data = Convert.ToBase64String(
        Encoding.UTF8.GetBytes(textBox.Text)
    );
    
    Bitmap qrCode = generator.GenerateQRCodeAsBitmap(base64Data, 2, 300);
    pictureBox.Image = qrCode;
}
```

### ASP.NET Web Application

```csharp
public ActionResult GenerateQRCode(string data)
{
    var generator = new QRGenerator();
    using (Bitmap qrCode = generator.GenerateQRCodeAsBitmap(data, 2, 300))
    {
        using (MemoryStream ms = new MemoryStream())
        {
            qrCode.Save(ms, ImageFormat.Png);
            return File(ms.ToArray(), "image/png");
        }
    }
}
```

### Batch Processing

```csharp
var generator = new QRGenerator();
var dataList = new List<string> { "data1", "data2", "data3" };

foreach (var data in dataList)
{
    string fileName = $"qr_{DateTime.Now.Ticks}.png";
    generator.GenerateQRCodeToFile(data, 2, 300, fileName, ImageFormat.Png);
}
```

## Troubleshooting

### Common Issues

**Invalid Base64 String**
- Ensure your input is properly Base64-encoded
- Use `Convert.ToBase64String()` to encode data

**File Access Denied**
- Check write permissions in the output directory
- Ensure the path is valid

**Low Scan Success Rate**
- Use PNG format instead of JPEG
- Increase pixel density (try 400-500)
- Increase error correction level
- Test with multiple scanner apps

## License

MIT License - See [LICENSE](https://github.com/johanhenningsson4-hash/AztecQRGenerator/blob/main/LICENSE) for details

## Links

- **GitHub Repository**: https://github.com/johanhenningsson4-hash/AztecQRGenerator
- **Documentation**: https://github.com/johanhenningsson4-hash/AztecQRGenerator#readme
- **Report Issues**: https://github.com/johanhenningsson4-hash/AztecQRGenerator/issues

## Author

Johan Henningsson

---

**Version 1.2.0** - Multiple format support, in-memory generation, comprehensive logging
