# AztecQRGenerator.Core - NuGet Package

This is the class library version of AztecQRGenerator, packaged for NuGet distribution.

## Quick Start

### Building the Package

```bash
# Option 1: Visual Studio
# Open AztecQRGenerator.Core.csproj ? Build ? Rebuild (Release mode)

# Option 2: Command Line
msbuild AztecQRGenerator.Core.csproj /p:Configuration=Release /t:Rebuild
```

Output: `bin\Release\AztecQRGenerator.Core.1.2.0.nupkg`

### Publishing to NuGet

```bash
cd bin\Release
nuget push AztecQRGenerator.Core.1.2.0.nupkg YOUR_API_KEY -Source https://api.nuget.org/v3/index.json
```

## Files

- **AztecQRGenerator.Core.csproj** - Project file with NuGet metadata
- **QRGenerator.cs** - QR code generator class
- **AztecGenerator.cs** - Aztec code generator class
- **Logger.cs** - Logging utility
- **NUGET_README.md** - Package documentation (included in .nupkg)
- **NUGET_PUBLISHING_GUIDE.md** - Complete publishing instructions
- **icon.png** - Package icon (create 128x128 PNG)
- **Properties/AssemblyInfo.cs** - Assembly version info

## Important Notes

1. **Build Separately**: Build this project directly, not as part of the main solution
2. **Icon**: Create a 128x128 PNG icon as `icon.png` (optional but recommended)
3. **Version**: Update version in both `.csproj` and `AssemblyInfo.cs` before publishing
4. **Testing**: Test locally before publishing (see NUGET_PUBLISHING_GUIDE.md)

## Documentation

- **Full Publishing Guide**: See `NUGET_PUBLISHING_GUIDE.md`
- **Package README**: See `NUGET_README.md`
- **Main Project**: See parent directory README.md

## Links

- **GitHub**: https://github.com/johanhenningsson4-hash/AztecQRGenerator
- **NuGet** (after publishing): https://www.nuget.org/packages/AztecQRGenerator.Core

## License

MIT License - See parent directory LICENSE file
