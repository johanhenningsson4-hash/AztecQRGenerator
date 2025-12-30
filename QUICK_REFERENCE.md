# Quick Reference - Image Format Support

## ?? Quick Start

### PNG (Recommended) ?
```csharp
using System.Drawing.Imaging;
using AztecQR;

var gen = new QRGenerator();
gen.GenerateQRCodeToFile("base64data", 2, 300, "output.png", ImageFormat.Png);
```

### JPEG (Not Recommended) ??
```csharp
gen.GenerateQRCodeToFile("base64data", 2, 300, "output.jpg", ImageFormat.Jpeg);
// Warning: Compression may reduce scanability
```

### BMP (Large Files) ??
```csharp
gen.GenerateQRCodeToFile("base64data", 2, 300, "output.bmp", ImageFormat.Bmp);
// Note: Files are ~100x larger than PNG
```

---

## ?? Method Signatures

### QR Generator
```csharp
// New method with format support
bool GenerateQRCodeToFile(string qrstring, int lCorrection, 
                          int lPixelDensity, string filePath, 
                          ImageFormat format)

// Existing methods (unchanged)
Bitmap GenerateQRCodeAsBitmap(string qrstring, int lCorrection, int lPixelDensity)
bool GenerateQRBitmap(int lTaNmbrqr, string qrstring, int lCorrection, int lPixelDensity)
```

### Aztec Generator
```csharp
// New method with format support
bool GenerateAztecCodeToFile(string aztecstring, int lCorrection, 
                             int lPixelDensity, string filePath, 
                             ImageFormat format)

// Existing methods (unchanged)
Bitmap GenerateAztecCodeAsBitmap(string aztecstring, int lCorrection, int lPixelDensity)
bool GenerateAztecBitmap(int lTaNmbrqr, string aztecstring, int lCorrection, int lPixelDensity)
```

---

## ?? Format Comparison

| Format | Quality | Size | Speed | Barcode Use |
|--------|---------|------|-------|-------------|
| PNG | ????? | Medium | Fast | ? Excellent |
| JPEG | ??? | Small | Medium | ?? Poor |
| BMP | ????? | Huge | Fast | ? Excellent |

**Recommendation:** Always use PNG for barcodes unless you have a specific reason not to.

---

## ?? File Size Reference (300x300 pixels)

```
PNG:  ~3 KB   ? Recommended
JPEG: ~5 KB   ? Not recommended (compression artifacts)
BMP:  ~270 KB ? Very large
```

---

## ?? Common Patterns

### Pattern 1: Save in Multiple Formats
```csharp
var gen = new QRGenerator();
using (Bitmap bmp = gen.GenerateQRCodeAsBitmap(data, 2, 300))
{
    bmp.Save("qr.png", ImageFormat.Png);
    bmp.Save("qr.jpg", ImageFormat.Jpeg);
    bmp.Save("qr.bmp", ImageFormat.Bmp);
}
```

### Pattern 2: User Choice
```csharp
var gen = new QRGenerator();
ImageFormat format = userWantsPng ? ImageFormat.Png : ImageFormat.Jpeg;
gen.GenerateQRCodeToFile(data, 2, 300, "output.png", format);
```

### Pattern 3: Error Handling
```csharp
try
{
    var gen = new QRGenerator();
    bool success = gen.GenerateQRCodeToFile(
        data, 2, 300, "output.png", ImageFormat.Png
    );
    if (success) Console.WriteLine("Success!");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid input: {ex.Message}");
}
catch (IOException ex)
{
    Console.WriteLine($"File error: {ex.Message}");
}
```

---

## ? Performance Tips

1. **Generation is the bottleneck** - Format choice has minimal impact on speed
2. **PNG is fastest to save** - Optimized compression
3. **BMP is second fastest** - No compression needed
4. **JPEG is slowest** - Compression calculation required
5. **Memory usage is the same** - Format doesn't affect generation memory

---

## ?? Important Notes

### JPEG Warnings
- ? Compression creates artifacts around sharp edges
- ? May reduce barcode scan success rate
- ? Not recommended for production barcodes
- ? Only use if file size is absolutely critical

### BMP Cautions
- ? Excellent quality (lossless)
- ? Files are ~100x larger than PNG
- ? Limited web support
- ? Good for legacy systems

### PNG Benefits
- ? Lossless compression
- ? Sharp edges preserved
- ? Moderate file size
- ? Universal support
- ? **Recommended for all barcode use**

---

## ?? Full Documentation

- **[README.md](README.md)** - Project overview
- **[IMAGE_FORMAT_GUIDE.md](IMAGE_FORMAT_GUIDE.md)** - Comprehensive format guide
- **[USAGE_EXAMPLES.md](USAGE_EXAMPLES.md)** - General usage examples
- **[FORMAT_SUPPORT_SUMMARY.md](FORMAT_SUPPORT_SUMMARY.md)** - Technical details
- **[Examples/FormatTestExample.cs](Examples/FormatTestExample.cs)** - Test program

---

## ?? Troubleshooting

### Problem: "Format cannot be null"
**Solution:** Make sure to pass ImageFormat.Png, ImageFormat.Jpeg, or ImageFormat.Bmp

### Problem: File is too large
**Solution:** Use PNG instead of BMP (100x smaller)

### Problem: Barcode won't scan
**Solution:** Don't use JPEG - use PNG instead

### Problem: "File path cannot be empty"
**Solution:** Provide a valid file path including extension

---

## ? Checklist for Production Use

- [ ] Use PNG format (not JPEG)
- [ ] Handle exceptions properly
- [ ] Test with actual barcode scanners
- [ ] Validate input parameters
- [ ] Dispose bitmaps when done
- [ ] Check write permissions
- [ ] Log errors for debugging

---

**For more details, see [IMAGE_FORMAT_GUIDE.md](IMAGE_FORMAT_GUIDE.md)**
