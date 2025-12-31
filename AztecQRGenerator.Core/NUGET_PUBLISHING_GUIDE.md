# Building and Publishing AztecQRGenerator.Core to NuGet

This guide explains how to build the NuGet package and publish it to nuget.org.

## Prerequisites

1. **.NET Framework 4.7.2 SDK** installed
2. **MSBuild** (comes with Visual Studio or Build Tools)
3. **NuGet.exe** CLI tool ([Download](https://www.nuget.org/downloads))
4. **NuGet.org account** (create at https://www.nuget.org/)
5. **API Key** from NuGet.org (https://www.nuget.org/account/apikeys)

## Project Structure

```
AztecQRGenerator/
??? AztecQRGenerator.csproj          # Main WinForms application
??? QRGenerator.cs
??? AztecGenerator.cs
??? Logger.cs
??? Program.cs
??? AztecQR.cs
??? AztecQRGenerator.Core/           # NuGet package project (separate)
    ??? AztecQRGenerator.Core.csproj
    ??? QRGenerator.cs               # Copy of generator
    ??? AztecGenerator.cs            # Copy of generator
    ??? Logger.cs                    # Copy of logger
    ??? NUGET_README.md              # Package documentation
    ??? icon.png                     # Package icon (optional)
    ??? Properties/
        ??? AssemblyInfo.cs
```

## Step 1: Prepare the Package Icon (Optional but Recommended)

Create a 128x128 PNG icon for your package:

1. Create or generate a QR code pattern image (128x128 pixels)
2. Save as `AztecQRGenerator.Core/icon.png`
3. The icon will automatically be included in the package

**Quick PowerShell Icon Generator:**
```powershell
Add-Type -AssemblyName System.Drawing
$bmp = New-Object System.Drawing.Bitmap 128, 128
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.Clear([System.Drawing.Color]::White)
# Draw QR-like pattern
$g.FillRectangle([System.Drawing.Brushes]::Black, 10, 10, 30, 30)
$g.FillRectangle([System.Drawing.Brushes]::Black, 88, 10, 30, 30)
$g.FillRectangle([System.Drawing.Brushes]::Black, 10, 88, 30, 30)
$bmp.Save("AztecQRGenerator.Core\icon.png", [System.Drawing.Imaging.ImageFormat]::Png)
$g.Dispose()
$bmp.Dispose()
```

## Step 2: Build the Core Project

### Option A: Using Visual Studio

1. Open **Visual Studio**
2. Open **AztecQRGenerator.Core.csproj** (not the solution file!)
3. Select **Release** configuration
4. Build ? **Rebuild Project**
5. Package will be created at: `AztecQRGenerator.Core\bin\Release\AztecQRGenerator.Core.1.2.0.nupkg`

### Option B: Using MSBuild Command Line

```bash
# Navigate to project directory
cd C:\Jobb\AztecQRGenerator

# Restore NuGet packages
nuget restore AztecQRGenerator.Core\AztecQRGenerator.Core.csproj

# Build in Release mode (generates NuGet package automatically)
msbuild AztecQRGenerator.Core\AztecQRGenerator.Core.csproj /p:Configuration=Release /t:Rebuild
```

The `.nupkg` file will be created at:
```
AztecQRGenerator.Core\bin\Release\AztecQRGenerator.Core.1.2.0.nupkg
```

### Option C: Using dotnet CLI (if .NET Core SDK is installed)

```bash
cd AztecQRGenerator.Core
dotnet pack -c Release
```

## Step 3: Test the Package Locally

Before publishing, test the package locally:

### Create a Test Project

```bash
# Create test project
mkdir TestAztecQR
cd TestAztecQR
dotnet new console -f net472

# Add local NuGet source
nuget sources Add -Name "LocalTest" -Source "C:\Jobb\AztecQRGenerator\AztecQRGenerator.Core\bin\Release"

# Install package
nuget install AztecQRGenerator.Core -Version 1.2.0 -Source LocalTest

# Or using dotnet
dotnet add package AztecQRGenerator.Core --version 1.2.0 --source "C:\Jobb\AztecQRGenerator\AztecQRGenerator.Core\bin\Release"
```

### Test Code

```csharp
using System;
using System.Drawing;
using System.Drawing.Imaging;
using AztecQR;

class Program
{
    static void Main()
    {
        var generator = new QRGenerator();
        
        // Test 1: Generate as Bitmap
        Bitmap qr = generator.GenerateQRCodeAsBitmap("SGVsbG8gV29ybGQh", 2, 300);
        Console.WriteLine($"Generated QR: {qr.Width}x{qr.Height}");
        qr.Dispose();
        
        // Test 2: Save to file
        bool success = generator.GenerateQRCodeToFile(
            "SGVsbG8gV29ybGQh", 2, 300, "test.png", ImageFormat.Png
        );
        Console.WriteLine($"Saved to file: {success}");
        
        Console.WriteLine("All tests passed!");
    }
}
```

## Step 4: Publish to NuGet.org

### Get Your API Key

1. Go to https://www.nuget.org/account/apikeys
2. Click "Create" to generate a new API key
3. Set:
   - **Key Name**: "AztecQRGenerator Publishing"
   - **Package Owner**: Your username
   - **Glob Pattern**: `AztecQRGenerator.*` (or `*` for all packages)
   - **Scopes**: Push, Push new packages and package versions
4. Click "Create"
5. **Copy the key immediately** (you won't see it again!)

### Publish Using NuGet CLI

```bash
cd C:\Jobb\AztecQRGenerator\AztecQRGenerator.Core\bin\Release

# Push to NuGet.org
nuget push AztecQRGenerator.Core.1.2.0.nupkg YOUR_API_KEY_HERE -Source https://api.nuget.org/v3/index.json
```

**Example:**
```bash
nuget push AztecQRGenerator.Core.1.2.0.nupkg oy2abc...xyz -Source https://api.nuget.org/v3/index.json
```

### Publish Using dotnet CLI

```bash
dotnet nuget push AztecQRGenerator.Core.1.2.0.nupkg --api-key YOUR_API_KEY_HERE --source https://api.nuget.org/v3/index.json
```

## Step 5: Verify Publication

1. Go to https://www.nuget.org/packages/AztecQRGenerator.Core
2. Package should appear within **5-15 minutes**
3. It may take up to **2 hours** to be searchable

## Installing Your Published Package

Once published, anyone can install it:

### Package Manager Console
```powershell
Install-Package AztecQRGenerator.Core
```

### .NET CLI
```bash
dotnet add package AztecQRGenerator.Core
```

### Visual Studio
1. Right-click project ? Manage NuGet Packages
2. Browse tab ? Search "AztecQRGenerator.Core"
3. Install

## Updating the Package (Future Versions)

1. Update version in `AztecQRGenerator.Core.csproj`:
   ```xml
   <Version>1.2.1</Version>
   ```

2. Update `AssemblyVersion` in `Properties/AssemblyInfo.cs`:
   ```csharp
   [assembly: AssemblyVersion("1.2.1.0")]
   [assembly: AssemblyFileVersion("1.2.1.0")]
   ```

3. Update `PackageReleaseNotes` in `.csproj`

4. Rebuild and republish:
   ```bash
   msbuild AztecQRGenerator.Core\AztecQRGenerator.Core.csproj /p:Configuration=Release /t:Rebuild
   nuget push AztecQRGenerator.Core\bin\Release\AztecQRGenerator.Core.1.2.1.nupkg YOUR_API_KEY -Source https://api.nuget.org/v3/index.json
   ```

## Troubleshooting

### Build Errors

**Problem**: CS0101 duplicate definition errors  
**Solution**: Make sure you're building `AztecQRGenerator.Core.csproj` directly, not the main solution file

**Problem**: Missing ZXing.Net reference  
**Solution**: Run `nuget restore AztecQRGenerator.Core\AztecQRGenerator.Core.csproj`

### Publish Errors

**Problem**: "Package already exists"  
**Solution**: You cannot republish the same version. Increment the version number.

**Problem**: "Invalid API key"  
**Solution**: Regenerate your API key on nuget.org and try again

**Problem**: "Package size too large"  
**Solution**: Ensure you're building in Release mode (not Debug) and not including unnecessary files

### Package Not Appearing

- Wait 15-30 minutes after publishing
- Clear your NuGet package cache:
  ```bash
  dotnet nuget locals all --clear
  ```
- Check package status at https://www.nuget.org/account/Packages

## Best Practices

1. ? **Always test locally** before publishing
2. ? **Use semantic versioning** (Major.Minor.Patch)
3. ? **Update release notes** for each version
4. ? **Keep icon.png** < 1 MB (ideally < 100 KB)
5. ? **Review package contents** before publishing:
   ```bash
   nuget pack AztecQRGenerator.Core.csproj -Properties Configuration=Release
   # Extract and inspect the .nupkg (it's a ZIP file)
   ```
6. ? **Test on a clean machine** or VM to ensure all dependencies are correct
7. ?? **Never commit your API key** to version control

## Package Information

- **Package ID**: AztecQRGenerator.Core
- **Current Version**: 1.2.0
- **License**: MIT
- **Target Framework**: .NET Framework 4.7.2
- **Dependencies**: ZXing.Net 0.16.9
- **Package Size**: ~100-200 KB
- **GitHub**: https://github.com/johanhenningsson4-hash/AztecQRGenerator
- **NuGet**: https://www.nuget.org/packages/AztecQRGenerator.Core (after publication)

## Support

- **Issues**: https://github.com/johanhenningsson4-hash/AztecQRGenerator/issues
- **Documentation**: See NUGET_README.md in package
- **Author**: Johan Henningsson

---

**Ready to publish? Follow the steps above to share your library with the .NET community!** ??
