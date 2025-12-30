# Image Format Support - Usage Guide

## Overview
The AztecQRGenerator now supports saving barcodes in multiple image formats: PNG, JPEG, and BMP. This provides flexibility for different use cases and file size requirements.

---

## Supported Image Formats

| Format | Extension | Pros | Cons | Best For |
|--------|-----------|------|------|----------|
| **PNG** | .png | Lossless, supports transparency, excellent for barcodes | Larger file size than JPEG | QR codes, archival, web use |
| **JPEG** | .jpg, .jpeg | Smaller file size, widely supported | Lossy compression, can blur edges | Photos, not recommended for barcodes |
| **BMP** | .bmp | Simple format, lossless | Very large file size, limited support | Legacy systems, raw image data |

### Recommendation
**PNG is the recommended format** for QR and Aztec codes due to its lossless compression and sharp edges. JPEG should generally be avoided for barcodes as the compression can make them harder to scan.

---

## API Methods

### QRGenerator

#### 1. GenerateQRCodeToFile() - Save with Specific Format
```csharp
public bool GenerateQRCodeToFile(
    string qrstring,        // Base64 encoded data
    int lCorrection,        // Error correction level (0-10)
    int lPixelDensity,      // Size in pixels
    string filePath,        // Output file path
    ImageFormat format      // PNG, JPEG, or BMP
)
```

#### 2. GenerateQRCodeAsBitmap() - Get Bitmap (unchanged)
```csharp
public Bitmap GenerateQRCodeAsBitmap(
    string qrstring,
    int lCorrection,
    int lPixelDensity
)
```

### AztecGenerator

#### 1. GenerateAztecCodeToFile() - Save with Specific Format
```csharp
public bool GenerateAztecCodeToFile(
    string aztecstring,     // Base64 encoded data
    int lCorrection,        // Error correction level (0-10)
    int lPixelDensity,      // Size in pixels
    string filePath,        // Output file path
    ImageFormat format      // PNG, JPEG, or BMP
)
```

#### 2. GenerateAztecCodeAsBitmap() - Get Bitmap (unchanged)
```csharp
public Bitmap GenerateAztecCodeAsBitmap(
    string aztecstring,
    int lCorrection,
    int lPixelDensity
)
```

---

## Usage Examples

### Example 1: Save QR Code as PNG

```csharp
using System.Drawing.Imaging;
using AztecQR;

var qrGenerator = new QRGenerator();
string base64Data = "SGVsbG8gV29ybGQh"; // "Hello World!"

bool success = qrGenerator.GenerateQRCodeToFile(
    qrstring: base64Data,
    lCorrection: 2,
    lPixelDensity: 300,
    filePath: "qrcode.png",
    format: ImageFormat.Png
);

if (success)
{
    Console.WriteLine("QR code saved as PNG successfully!");
}
```

### Example 2: Save Aztec Code as JPEG

```csharp
using System.Drawing.Imaging;
using AztecQR;

var aztecGenerator = new AztecGenerator();
string base64Data = "SGVsbG8gV29ybGQh";

bool success = aztecGenerator.GenerateAztecCodeToFile(
    aztecstring: base64Data,
    lCorrection: 2,
    lPixelDensity: 300,
    filePath: "azteccode.jpg",
    format: ImageFormat.Jpeg
);

if (success)
{
    Console.WriteLine("Aztec code saved as JPEG successfully!");
}
```

### Example 3: Save as BMP

```csharp
using System.Drawing.Imaging;
using AztecQR;

var qrGenerator = new QRGenerator();
string base64Data = "SGVsbG8gV29ybGQh";

bool success = qrGenerator.GenerateQRCodeToFile(
    qrstring: base64Data,
    lCorrection: 2,
    lPixelDensity: 300,
    filePath: "qrcode.bmp",
    format: ImageFormat.Bmp
);
```

### Example 4: Generate Bitmap and Save in Multiple Formats

```csharp
using System.Drawing;
using System.Drawing.Imaging;
using AztecQR;

var qrGenerator = new QRGenerator();
string base64Data = "SGVsbG8gV29ybGQh";

// Generate once
using (Bitmap qrBitmap = qrGenerator.GenerateQRCodeAsBitmap(base64Data, 2, 300))
{
    // Save in multiple formats
    qrBitmap.Save("qrcode.png", ImageFormat.Png);
    qrBitmap.Save("qrcode.jpg", ImageFormat.Jpeg);
    qrBitmap.Save("qrcode.bmp", ImageFormat.Bmp);
    
    Console.WriteLine("QR code saved in all formats!");
}
```

### Example 5: Dynamic Format Selection

```csharp
using System;
using System.Drawing.Imaging;
using AztecQR;

var qrGenerator = new QRGenerator();
string base64Data = "SGVsbG8gV29ybGQh";
string outputPath = "qrcode";

// User selects format
Console.WriteLine("Select format: 1=PNG, 2=JPEG, 3=BMP");
string choice = Console.ReadLine();

ImageFormat format;
string extension;

switch (choice)
{
    case "1":
        format = ImageFormat.Png;
        extension = ".png";
        break;
    case "2":
        format = ImageFormat.Jpeg;
        extension = ".jpg";
        break;
    case "3":
        format = ImageFormat.Bmp;
        extension = ".bmp";
        break;
    default:
        format = ImageFormat.Png;
        extension = ".png";
        break;
}

bool success = qrGenerator.GenerateQRCodeToFile(
    base64Data,
    2,
    300,
    outputPath + extension,
    format
);

if (success)
{
    Console.WriteLine($"QR code saved as {extension.ToUpper()} successfully!");
}
```

### Example 6: Batch Generation in Different Formats

```csharp
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using AztecQR;

var qrGenerator = new QRGenerator();

// List of data to encode
var dataList = new List<string>
{
    "SGVsbG8gV29ybGQh",
    "QmFyY29kZSBUZXN0",
    "QW5vdGhlciBURVNU"
};

// Generate in all formats
var formats = new Dictionary<ImageFormat, string>
{
    { ImageFormat.Png, ".png" },
    { ImageFormat.Jpeg, ".jpg" },
    { ImageFormat.Bmp, ".bmp" }
};

int counter = 1;
foreach (var data in dataList)
{
    foreach (var format in formats)
    {
        string fileName = $"qrcode_{counter}{format.Value}";
        
        try
        {
            bool success = qrGenerator.GenerateQRCodeToFile(
                data,
                2,
                300,
                fileName,
                format.Key
            );
            
            if (success)
            {
                Console.WriteLine($"Generated: {fileName}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating {fileName}: {ex.Message}");
        }
    }
    counter++;
}
```

### Example 7: Windows Forms with Format Selection

```csharp
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using AztecQR;

public class BarcodeForm : Form
{
    private ComboBox comboFormat;
    private Button btnGenerate;
    private TextBox txtInput;
    private PictureBox pictureBox;

    public BarcodeForm()
    {
        InitializeForm();
        btnGenerate.Click += GenerateBarcode;
    }

    private void InitializeForm()
    {
        // Setup controls...
        comboFormat = new ComboBox();
        comboFormat.Items.AddRange(new string[] { "PNG", "JPEG", "BMP" });
        comboFormat.SelectedIndex = 0;
        // Add other controls...
    }

    private void GenerateBarcode(object sender, EventArgs e)
    {
        try
        {
            var qrGenerator = new QRGenerator();
            string base64Data = txtInput.Text;

            // Generate bitmap for display
            Bitmap qrBitmap = qrGenerator.GenerateQRCodeAsBitmap(base64Data, 2, 300);
            pictureBox.Image = qrBitmap;

            // Save with selected format
            ImageFormat format = GetSelectedFormat();
            string extension = GetExtension(format);
            string fileName = $"qrcode_{DateTime.Now:yyyyMMddHHmmss}{extension}";

            qrGenerator.GenerateQRCodeToFile(
                base64Data,
                2,
                300,
                fileName,
                format
            );

            MessageBox.Show($"Saved as {fileName}", "Success");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error");
        }
    }

    private ImageFormat GetSelectedFormat()
    {
        switch (comboFormat.SelectedItem.ToString())
        {
            case "JPEG":
                return ImageFormat.Jpeg;
            case "BMP":
                return ImageFormat.Bmp;
            default:
                return ImageFormat.Png;
        }
    }

    private string GetExtension(ImageFormat format)
    {
        if (format.Equals(ImageFormat.Jpeg))
            return ".jpg";
        if (format.Equals(ImageFormat.Bmp))
            return ".bmp";
        return ".png";
    }
}
```

---

## Format-Specific Considerations

### PNG Format
```csharp
// Best quality for barcodes
qrGenerator.GenerateQRCodeToFile(data, 2, 300, "output.png", ImageFormat.Png);
```
- **Pros:** Lossless, sharp edges, moderate file size
- **Cons:** Larger than JPEG
- **Use When:** Barcode quality is critical (recommended)

### JPEG Format
```csharp
// Smaller file size (NOT RECOMMENDED for barcodes)
qrGenerator.GenerateQRCodeToFile(data, 2, 300, "output.jpg", ImageFormat.Jpeg);
```
- **Pros:** Small file size
- **Cons:** Lossy compression causes edge artifacts, can affect scanability
- **Use When:** File size is absolutely critical and quality loss is acceptable
- **Warning:** May reduce barcode scan success rate

### BMP Format
```csharp
// Legacy compatibility
qrGenerator.GenerateQRCodeToFile(data, 2, 300, "output.bmp", ImageFormat.Bmp);
```
- **Pros:** Simple format, lossless, maximum compatibility
- **Cons:** Very large file size
- **Use When:** Working with legacy systems or need raw pixel data

---

## File Size Comparison

Approximate file sizes for a 300x300 pixel QR code:

| Format | File Size | Compression | Quality |
|--------|-----------|-------------|---------|
| PNG | ~2-5 KB | Lossless | Excellent |
| JPEG | ~3-8 KB | Lossy | Fair (artifacts) |
| BMP | ~270 KB | None | Excellent |

*Note: JPEG may be smaller but quality suffers significantly for barcodes.*

---

## Error Handling

```csharp
using System;
using System.Drawing.Imaging;
using AztecQR;

try
{
    var qrGenerator = new QRGenerator();
    
    bool success = qrGenerator.GenerateQRCodeToFile(
        "SGVsbG8gV29ybGQh",
        2,
        300,
        "output.png",
        ImageFormat.Png
    );
    
    if (success)
    {
        Console.WriteLine("Generated successfully!");
    }
}
catch (ArgumentException ex)
{
    // Invalid parameters (null data, empty path, etc.)
    Console.WriteLine($"Invalid input: {ex.Message}");
}
catch (ArgumentNullException ex)
{
    // Null format
    Console.WriteLine($"Format cannot be null: {ex.Message}");
}
catch (System.IO.IOException ex)
{
    // File system error
    Console.WriteLine($"File error: {ex.Message}");
}
catch (InvalidOperationException ex)
{
    // Generation failed
    Console.WriteLine($"Generation failed: {ex.Message}");
}
```

---

## Migration from Old API

### Old Method (PNG only)
```csharp
// Old way - always saves as PNG
bool success = qrGenerator.GenerateQRBitmap(1, base64Data, 2, 300);
```

### New Method (Any format)
```csharp
// New way - specify format
bool success = qrGenerator.GenerateQRCodeToFile(
    base64Data,
    2,
    300,
    "output.png",
    ImageFormat.Png
);
```

**Note:** The old `GenerateQRBitmap()` method still works and maintains backward compatibility.

---

## Best Practices

1. **Use PNG for barcodes** - Best quality-to-size ratio
2. **Avoid JPEG for barcodes** - Compression artifacts reduce scanability
3. **Use BMP only when necessary** - File sizes are very large
4. **Validate file paths** - Check write permissions before generation
5. **Handle exceptions** - Always wrap in try-catch blocks
6. **Test scanability** - Always test generated barcodes with scanners
7. **Consider file size** - PNG is usually the best compromise

---

## Performance Considerations

### Format Impact on Generation Speed

```csharp
// Generation time is primarily affected by barcode complexity, not format
// Format affects only the final save operation

Bitmap generation:  95% of time
Format conversion:   5% of time

PNG:  Fastest to save (optimized compression)
BMP:  Fast (no compression)
JPEG: Moderate (compression calculation)
```

### Memory Usage

```csharp
// All formats use same memory during generation
// Memory is affected by pixel density, not output format

300x300 pixels = ~270 KB in memory (uncompressed)
500x500 pixels = ~750 KB in memory (uncompressed)
1000x1000 pixels = ~3 MB in memory (uncompressed)
```

---

## See Also

- [USAGE_EXAMPLES.md](USAGE_EXAMPLES.md) - General usage examples
- [README.md](README.md) - Project overview
- [IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md) - Technical details
