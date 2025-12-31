# AztecQRGenerator.Core NuGet Package - Setup Complete ?

## Summary

Successfully created a separate class library project ready for NuGet publication. The project is now structured to support both:

1. **Standalone Windows Forms Application** - The original project with GUI and CLI
2. **NuGet Package Library** - Clean class library without GUI dependencies

---

## What Was Created

### New Files in `AztecQRGenerator.Core/`

| File | Purpose |
|------|---------|
| `AztecQRGenerator.Core.csproj` | Project file with NuGet packaging configuration |
| `QRGenerator.cs` | Core QR code generator (copy from main project) |
| `AztecGenerator.cs` | Core Aztec code generator (copy from main project) |
| `Logger.cs` | Logging utility (copy from main project) |
| `Properties/AssemblyInfo.cs` | Assembly version and metadata |
| `NUGET_README.md` | Package documentation (shown on nuget.org) |
| `NUGET_PUBLISHING_GUIDE.md` | Complete step-by-step publishing guide |
| `README.md` | Quick reference for the Core project |
| `icon-instructions.txt` | Instructions for creating package icon |

### Updated Files

| File | Changes |
|------|---------|
| `README.md` | Added NuGet installation instructions and project structure |

---

## Package Details

### Metadata

- **Package ID**: `AztecQRGenerator.Core`
- **Version**: `1.2.0`
- **Author**: Johan Henningsson
- **License**: MIT
- **Target**: .NET Framework 4.7.2
- **Dependencies**: ZXing.Net 0.16.9

### Features

? QR Code Generation  
? Aztec Code Generation  
? Multiple Image Formats (PNG, JPEG, BMP)  
? In-Memory Bitmap Generation  
? File Saving with Format Selection  
? Comprehensive Error Handling  
? Built-in Logging  
? ISO-8859-1 Encoding Support  

---

## Next Steps

### 1. Create Package Icon (Optional but Recommended)

Create a 128x128 PNG image and save as `AztecQRGenerator.Core/icon.png`

**Quick PowerShell Script:**
```powershell
Add-Type -AssemblyName System.Drawing
$bmp = New-Object System.Drawing.Bitmap 128, 128
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.Clear([System.Drawing.Color]::White)
$g.FillRectangle([System.Drawing.Brushes]::Black, 10, 10, 30, 30)
$g.FillRectangle([System.Drawing.Brushes]::Black, 88, 10, 30, 30)
$g.FillRectangle([System.Drawing.Brushes]::Black, 10, 88, 30, 30)
$bmp.Save("AztecQRGenerator.Core\icon.png", [System.Drawing.Imaging.ImageFormat]::Png)
$g.Dispose()
$bmp.Dispose()
Write-Host "Icon created successfully!"
```

### 2. Build the Package

**Option A: Visual Studio**
1. Open `AztecQRGenerator.Core.csproj` (NOT the solution!)
2. Select **Release** configuration
3. Build ? **Rebuild Project**
4. Find `.nupkg` at: `AztecQRGenerator.Core\bin\Release\AztecQRGenerator.Core.1.2.0.nupkg`

**Option B: Command Line**
```bash
cd C:\Jobb\AztecQRGenerator
msbuild AztecQRGenerator.Core\AztecQRGenerator.Core.csproj /p:Configuration=Release /t:Rebuild
```

### 3. Test Locally

```bash
# Install in a test project
dotnet add package AztecQRGenerator.Core --version 1.2.0 --source "C:\Jobb\AztecQRGenerator\AztecQRGenerator.Core\bin\Release"
```

### 4. Publish to NuGet.org

1. Get API key from https://www.nuget.org/account/apikeys
2. Run:
   ```bash
   cd AztecQRGenerator.Core\bin\Release
   nuget push AztecQRGenerator.Core.1.2.0.nupkg YOUR_API_KEY -Source https://api.nuget.org/v3/index.json
   ```
3. Wait 15-30 minutes for package to appear at:
   https://www.nuget.org/packages/AztecQRGenerator.Core

---

## Documentation

### For Package Users

- **Installation**: See main README.md
- **Usage Examples**: See NUGET_README.md
- **API Reference**: Main README.md

### For Maintainers

- **Building**: See NUGET_PUBLISHING_GUIDE.md
- **Publishing**: See NUGET_PUBLISHING_GUIDE.md
- **Versioning**: Update `.csproj` and `AssemblyInfo.cs`

---

## Important Notes

### Building

?? **Build the Core project separately**, not as part of the main solution. This avoids duplicate definition errors.

**Correct:**
```bash
msbuild AztecQRGenerator.Core\AztecQRGenerator.Core.csproj /p:Configuration=Release
```

**Incorrect:**
```bash
msbuild AztecQRGenerator.sln  # This will cause CS0101 errors
```

### Project Independence

The Core library and main application are **independent**:

- **Main Project**: Contains full application with GUI
- **Core Library**: Contains only generator classes (no GUI)

Both have their own copies of:
- QRGenerator.cs
- AztecGenerator.cs
- Logger.cs

This allows:
- ? Main app continues working independently
- ? Core library is clean and minimal
- ? No dependencies between projects
- ? Easy maintenance of both

---

## File Checklist

Before publishing, ensure these files exist:

- [x] `AztecQRGenerator.Core/AztecQRGenerator.Core.csproj`
- [x] `AztecQRGenerator.Core/QRGenerator.cs`
- [x] `AztecQRGenerator.Core/AztecGenerator.cs`
- [x] `AztecQRGenerator.Core/Logger.cs`
- [x] `AztecQRGenerator.Core/Properties/AssemblyInfo.cs`
- [x] `AztecQRGenerator.Core/NUGET_README.md`
- [x] `AztecQRGenerator.Core/NUGET_PUBLISHING_GUIDE.md`
- [x] `AztecQRGenerator.Core/README.md`
- [ ] `AztecQRGenerator.Core/icon.png` (create this!)

---

## Testing Checklist

Before publishing to NuGet:

- [ ] Build succeeds in Release mode
- [ ] Package builds without errors (`.nupkg` created)
- [ ] Test installation in a separate project
- [ ] Test basic functionality (generate QR, save to file)
- [ ] Verify all dependencies are correct
- [ ] Review package contents (extract `.nupkg` as ZIP)
- [ ] Check icon displays correctly
- [ ] Verify README displays on test installation

---

## Support

- **Issues**: https://github.com/johanhenningsson4-hash/AztecQRGenerator/issues
- **Repository**: https://github.com/johanhenningsson4-hash/AztecQRGenerator
- **Documentation**: See README files
- **Author**: Johan Henningsson

---

## Quick Commands Reference

```bash
# Build package
msbuild AztecQRGenerator.Core\AztecQRGenerator.Core.csproj /p:Configuration=Release /t:Rebuild

# Test locally
dotnet add package AztecQRGenerator.Core --version 1.2.0 --source "C:\Jobb\AztecQRGenerator\AztecQRGenerator.Core\bin\Release"

# Publish to NuGet
cd AztecQRGenerator.Core\bin\Release
nuget push AztecQRGenerator.Core.1.2.0.nupkg YOUR_API_KEY -Source https://api.nuget.org/v3/index.json

# Install from NuGet (after publishing)
Install-Package AztecQRGenerator.Core
```

---

**?? Your NuGet package project is ready! Follow the steps above to build and publish.** ??

---

*Last Updated: January 2025*  
*Version: 1.2.0*  
*Status: Ready for Publication*
