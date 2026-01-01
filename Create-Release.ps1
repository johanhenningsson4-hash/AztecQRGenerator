# Create Release Package for v1.3.0
# This script packages the release build into a distributable ZIP file

param(
    [string]$Version = "1.3.0",
    [string]$BuildPath = "C:\Jobb\AztecQRGenerator\bin\Release",
    [string]$OutputPath = "C:\Jobb\AztecQRGenerator\Releases"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "AztecQRGenerator Release Packager" -ForegroundColor Cyan
Write-Host "Version: $Version" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Create output directory
if (-not (Test-Path $OutputPath)) {
    Write-Host "Creating releases directory..." -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
}

$packageName = "AztecQRGenerator_v$Version"
$packagePath = Join-Path $OutputPath $packageName
$zipPath = "$packagePath.zip"

# Clean up old package if exists
if (Test-Path $packagePath) {
    Write-Host "Removing old package directory..." -ForegroundColor Yellow
    Remove-Item $packagePath -Recurse -Force
}

if (Test-Path $zipPath) {
    Write-Host "Removing old ZIP file..." -ForegroundColor Yellow
    Remove-Item $zipPath -Force
}

# Create package directory
Write-Host "Creating package directory..." -ForegroundColor Yellow
New-Item -ItemType Directory -Path $packagePath -Force | Out-Null

# Copy executable and dependencies
Write-Host "Copying application files..." -ForegroundColor Yellow
$filesToCopy = @(
    "AztecQRGenerator.exe",
    "AztecQRGenerator.exe.config",
    "AztecQRGenerator.Core.dll",
    "zxing.dll",
    "zxing.presentation.dll"
)

foreach ($file in $filesToCopy) {
    $sourcePath = Join-Path $BuildPath $file
    if (Test-Path $sourcePath) {
        Copy-Item $sourcePath $packagePath
        Write-Host "  ? Copied: $file" -ForegroundColor Green
    } else {
        Write-Host "  ? Missing: $file" -ForegroundColor Yellow
    }
}

# Copy documentation
Write-Host "Copying documentation..." -ForegroundColor Yellow
$docsToInclude = @(
    "README.md",
    "LICENSE",
    "CHANGELOG.md",
    "RELEASE_NOTES_v$Version.md",
    "USAGE_EXAMPLES.md",
    "IMAGE_FORMAT_GUIDE.md",
    "PERMISSION_FIX_SUMMARY.md"
)

$docsPath = Join-Path $packagePath "Docs"
New-Item -ItemType Directory -Path $docsPath -Force | Out-Null

foreach ($doc in $docsToInclude) {
    $sourcePath = Join-Path "C:\Jobb\AztecQRGenerator" $doc
    if (Test-Path $sourcePath) {
        Copy-Item $sourcePath $docsPath
        Write-Host "  ? Copied: $doc" -ForegroundColor Green
    } else {
        Write-Host "  ? Missing: $doc" -ForegroundColor Yellow
    }
}

# Copy test script
Write-Host "Copying test script..." -ForegroundColor Yellow
$testScript = "Test-PermissionFix.ps1"
$testScriptSource = Join-Path "C:\Jobb\AztecQRGenerator" $testScript
if (Test-Path $testScriptSource) {
    Copy-Item $testScriptSource $packagePath
    Write-Host "  ? Copied: $testScript" -ForegroundColor Green
}

# Create README for the package
Write-Host "Creating package README..." -ForegroundColor Yellow
$packageReadme = @"
# AztecQRGenerator v$Version

## Quick Start

### GUI Mode
Double-click ``AztecQRGenerator.exe``

### CLI Mode
``````powershell
# Generate QR code
.\AztecQRGenerator.exe QR "SGVsbG8gV29ybGQh" output.png 300 2

# Generate Aztec code
.\AztecQRGenerator.exe AZTEC "SGVsbG8gV29ybGQh" output.png 300 2
``````

## File Locations

- **Logs**: ``%LocalAppData%\AztecQRGenerator\Logs\``
- **Output**: ``%UserProfile%\Documents\AztecQRGenerator\Output\``

## Testing

Run ``Test-PermissionFix.ps1`` to verify installation.

## Documentation

See the ``Docs`` folder for:
- Complete README
- Changelog
- Release Notes
- Usage Examples
- Image Format Guide

## Requirements

- Windows 7 or later
- .NET Framework 4.7.2 or higher
- No administrator privileges required!

## Support

- GitHub: https://github.com/johanhenningsson4-hash/AztecQRGenerator
- Issues: https://github.com/johanhenningsson4-hash/AztecQRGenerator/issues

## License

MIT License - See LICENSE file in Docs folder

Copyright © 2025 Johan Olof Henningsson
"@

$packageReadme | Out-File (Join-Path $packagePath "README.txt") -Encoding UTF8
Write-Host "  ? Created package README" -ForegroundColor Green

# Create version info file
$versionInfo = @"
AztecQRGenerator
Version: $Version
Build Date: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
.NET Framework: 4.7.2
Platform: AnyCPU

Changes in this version:
- Permission-safe file locations (no admin required)
- Smart file saving with automatic fallback
- Enhanced CLI mode with better error messages
- Logger now uses AppData and Documents folders

For detailed release notes, see Docs\RELEASE_NOTES_v$Version.md
"@

$versionInfo | Out-File (Join-Path $packagePath "VERSION.txt") -Encoding UTF8
Write-Host "  ? Created version info" -ForegroundColor Green

# Create ZIP archive
Write-Host "" 
Write-Host "Creating ZIP archive..." -ForegroundColor Yellow
Compress-Archive -Path "$packagePath\*" -DestinationPath $zipPath -Force
Write-Host "  ? Created: $zipPath" -ForegroundColor Green

# Get file info
$zipInfo = Get-Item $zipPath
$zipSizeMB = [math]::Round($zipInfo.Length / 1MB, 2)

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Release Package Created Successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Package Details:" -ForegroundColor Yellow
Write-Host "  Version:    $Version" -ForegroundColor White
Write-Host "  Location:   $zipPath" -ForegroundColor White
Write-Host "  Size:       $zipSizeMB MB" -ForegroundColor White
Write-Host "  Files:      $(Get-ChildItem $packagePath -Recurse -File | Measure-Object | Select-Object -ExpandProperty Count)" -ForegroundColor White
Write-Host ""

# Generate checksums
Write-Host "Generating checksums..." -ForegroundColor Yellow
$md5 = Get-FileHash $zipPath -Algorithm MD5
$sha256 = Get-FileHash $zipPath -Algorithm SHA256

$checksumFile = "$zipPath.checksums.txt"
$checksums = @"
AztecQRGenerator v$Version
Checksums for: $(Split-Path $zipPath -Leaf)

MD5:    $($md5.Hash)
SHA256: $($sha256.Hash)

Generated: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
"@

$checksums | Out-File $checksumFile -Encoding UTF8
Write-Host "  ? Checksums saved to: $checksumFile" -ForegroundColor Green

Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "  1. Test the package by extracting and running" -ForegroundColor White
Write-Host "  2. Create Git tag: git tag -a v$Version -m 'Release v$Version'" -ForegroundColor White
Write-Host "  3. Push tag: git push origin v$Version" -ForegroundColor White
Write-Host "  4. Create GitHub release and upload ZIP file" -ForegroundColor White
Write-Host "  5. Update NuGet package to v$Version" -ForegroundColor White
Write-Host ""

# Open release folder
$openFolder = Read-Host "Open release folder? (Y/N)"
if ($openFolder -eq "Y" -or $openFolder -eq "y") {
    explorer $OutputPath
}

Write-Host ""
Write-Host "Release package ready! ??" -ForegroundColor Green
