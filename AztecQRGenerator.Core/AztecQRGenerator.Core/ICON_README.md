# Icon Creation for AztecQRGenerator

## Current Icon

The project now includes a **128x128 pixel PNG icon** (`icon.png`) that represents the AztecQRGenerator library. The icon features a stylized QR code pattern with blue accent position markers.

### Icon Details
- **Size**: 128x128 pixels
- **Format**: PNG
- **File Size**: ~2.4 KB
- **Style**: QR code pattern with royal blue accent markers
- **Location**: `AztecQRGenerator.Core/icon.png`

## How It Was Created

The icon was generated using the `Create-Icon.ps1` PowerShell script, which programmatically creates a QR code-style icon using the System.Drawing library.

### Regenerate the Icon

If you need to regenerate or modify the icon, you have three options:

#### Option 1: PowerShell Script (Recommended)
```powershell
cd AztecQRGenerator.Core
.\Create-Icon.ps1
```

#### Option 2: Batch File
```batch
cd AztecQRGenerator.Core
create_icon.bat
```

#### Option 3: Custom Code
Use the `CreateIcon.cs` class in your own code:
```csharp
AztecQR.IconCreator.CreateProjectIcon("icon.png");
```

## Icon in NuGet Package

The icon is automatically included in the NuGet package through the project configuration:

```xml
<PackageIcon>icon.png</PackageIcon>
<None Include="icon.png" Pack="true" PackagePath="\" />
```

When you publish to NuGet.org, this icon will appear:
- On the package page
- In Visual Studio NuGet Package Manager
- In package search results

## Customization

To customize the icon appearance, edit `Create-Icon.ps1`:

- **Size**: Change the `$size` variable (line 7)
- **Colors**: Modify the color values:
  - Background: Lines 19-23
  - Data pattern: Line 30 (dark color)
  - Position markers: Line 31 (blue accent)
  - Border: Line 101
- **Pattern Density**: Adjust the `$probability` value (line 62)
- **Cell Size**: Change `$cellSize` variable (line 9)

## Files

- `icon.png` - The actual icon file (included in project)
- `Create-Icon.ps1` - PowerShell script to generate the icon
- `create_icon.bat` - Batch wrapper for PowerShell script
- `CreateIcon.cs` - C# class for icon generation (for programmatic use)

## Build Verification

The icon is automatically verified during build:
- The project only includes the icon if it exists
- Condition: `Exists('icon.png')`
- If missing, the package builds without an icon (no error)

? **Status**: Icon successfully added and verified!
