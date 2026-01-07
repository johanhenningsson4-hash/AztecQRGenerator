# Usage Examples for QR and Aztec Code Generation

This document provides examples of how to use the QR and Aztec code generators to return bitmaps.

## QRGenerator - Returning Bitmap

### Example 1: Generate QR Code as Bitmap

```csharp
using System.Drawing;
using AztecQR;

// Create a QR generator instance
var qrGenerator = new QRGenerator();

// Base64 encoded data
string base64Data = "SGVsbG8gV29ybGQh"; // "Hello World!" encoded

// Generate QR code as Bitmap (without saving to file)
try
{
    Bitmap qrCodeBitmap = qrGenerator.GenerateQRCodeAsBitmap(
        qrstring: base64Data,
        lCorrection: 2,           // Error correction level (0-10)
        lPixelDensity: 300        // Size in pixels
    );
    
    // Use the bitmap (e.g., display in PictureBox, process, etc.)
    pictureBox1.Image = qrCodeBitmap;
    
    // Don't forget to dispose when done
    // qrCodeBitmap.Dispose();
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid input: {ex.Message}");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Generation failed: {ex.Message}");
}
```

### Example 2: Generate and Save QR Code

```csharp
// This method generates and saves to file automatically
var qrGenerator = new QRGenerator();
string base64Data = "SGVsbG8gV29ybGQh";

bool success = qrGenerator.GenerateQRBitmap(
    qrstring: base64Data,
    lCorrection: 2,
    lPixelDensity: 300
);

if (success)
{
    Console.WriteLine("QR code saved to file successfully");
}
```

## AztecGenerator - Returning Bitmap

### Example 1: Generate Aztec Code as Bitmap

```csharp
using System.Drawing;
using AztecQR;

// Create an Aztec generator instance
var aztecGenerator = new AztecGenerator();

// Base64 encoded data
string base64Data = "SGVsbG8gV29ybGQh"; // "Hello World!" encoded

// Generate Aztec code as Bitmap (without saving to file)
try
{
    Bitmap aztecCodeBitmap = aztecGenerator.GenerateAztecCodeAsBitmap(
        aztecstring: base64Data,
        lCorrection: 2,           // Error correction level (0-10)
        lPixelDensity: 300        // Size in pixels
    );
    
    // Use the bitmap (e.g., display in PictureBox, process, etc.)
    pictureBox1.Image = aztecCodeBitmap;
    
    // Don't forget to dispose when done
    // aztecCodeBitmap.Dispose();
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid input: {ex.Message}");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Generation failed: {ex.Message}");
}
```

### Example 2: Generate and Save Aztec Code

```csharp
// This method generates and saves to file automatically
var aztecGenerator = new AztecGenerator();
string base64Data = "SGVsbG8gV29ybGQh";

bool success = aztecGenerator.GenerateAztecBitmap(
    aztecstring: base64Data,
    lCorrection: 2,
    lPixelDensity: 300
);

if (success)
{
    Console.WriteLine("Aztec code saved to file successfully");
}
```

## Comparison of Methods

### Methods that Return Bitmap (New)
- `QRGenerator.GenerateQRCodeAsBitmap()` - Returns Bitmap object
- `AztecGenerator.GenerateAztecCodeAsBitmap()` - Returns Bitmap object
- **Advantages:** 
  - No automatic file saving
  - Full control over the bitmap
  - Can display directly in UI
  - Can save manually with custom filename/format
  - Memory efficient (caller controls disposal)

### Methods that Save to File (Existing)
- `QRGenerator.GenerateQRBitmap()` - Returns bool, saves 1 PNG file
- `AztecGenerator.GenerateAztecBitmap()` - Returns bool, saves 1 PNG file
- **Advantages:**
  - Simple batch processing
  - Automatic file naming with timestamp
  - Saves to Documents folder automatically

## Full Example: Using in Windows Forms

```csharp
using System;
using System.Drawing;
using System.Windows.Forms;
using AztecQR;

public class BarcodeForm : Form
{
    private PictureBox pictureBox;
    private TextBox textBoxInput;
    private Button btnGenerateQR;
    private Button btnGenerateAztec;
    private Bitmap currentBitmap;

    public BarcodeForm()
    {
        // Initialize controls...
        
        btnGenerateQR.Click += (s, e) => GenerateQRCode();
        btnGenerateAztec.Click += (s, e) => GenerateAztecCode();
    }

    private void GenerateQRCode()
    {
        try
        {
            // Clean up previous bitmap
            if (currentBitmap != null)
            {
                currentBitmap.Dispose();
            }

            var generator = new QRGenerator();
            currentBitmap = generator.GenerateQRCodeAsBitmap(
                qrstring: textBoxInput.Text,
                lCorrection: 2,
                lPixelDensity: 400
            );

            pictureBox.Image = currentBitmap;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Generation Failed");
        }
    }

    private void GenerateAztecCode()
    {
        try
        {
            // Clean up previous bitmap
            if (currentBitmap != null)
            {
                currentBitmap.Dispose();
            }

            var generator = new AztecGenerator();
            currentBitmap = generator.GenerateAztecCodeAsBitmap(
                aztecstring: textBoxInput.Text,
                lCorrection: 2,
                lPixelDensity: 400
            );

            pictureBox.Image = currentBitmap;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Generation Failed");
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (currentBitmap != null)
            {
                currentBitmap.Dispose();
            }
        }
        base.Dispose(disposing);
    }
}
```

## Parameters

### Common Parameters
- **qrstring / aztecstring**: Base64 encoded string to be encoded in the barcode
- **lCorrection**: Error correction level (0-10, default: 2)
  - Higher values = more error correction = larger code size
- **lPixelDensity**: Size of the generated code in pixels
  - Must be greater than 0
  - Typical values: 200-500 pixels

## Error Handling

All methods throw exceptions for invalid inputs:
- `ArgumentException`: Invalid Base64 string, null/empty input, or invalid pixel density
- `InvalidOperationException`: Failed to generate the barcode matrix
- `IOException`: (File-saving methods only) Failed to save the file

## Memory Management

When using the bitmap-returning methods:
1. Store the returned Bitmap in a variable
2. Use it as needed (display, process, save manually)
3. Call `Dispose()` when done to free memory
4. In forms, dispose in the `Dispose()` method or `FormClosing` event

```csharp
// Good practice
Bitmap bitmap = null;
try
{
    bitmap = generator.GenerateQRCodeAsBitmap(data, 2, 300);
    // Use bitmap...
}
finally
{
    if (bitmap != null)
    {
        bitmap.Dispose();
    }
}

// Or with using statement
using (Bitmap bitmap = generator.GenerateQRCodeAsBitmap(data, 2, 300))
{
    // Use bitmap...
}
