# Release Notes - AztecQRGenerator v1.3.0

**Release Date:** January 31, 2025  
**Build Status:** ? Successful  
**Breaking Changes:** None  

---

## ?? What's New in v1.3.0

### ?? No Administrator Privileges Required!
The biggest improvement in this release - **AztecQRGenerator now runs perfectly with standard user permissions**. No more "Access Denied" errors!

### ?? Smart File Locations
Files are now saved to user-friendly, always-accessible locations:
- **Logs**: `%LocalAppData%\AztecQRGenerator\Logs\`
- **Output**: `%UserProfile%\Documents\AztecQRGenerator\Output\`

### ?? Automatic Fallback System
If the application can't write to a specific location, it automatically falls back to safe alternatives:
1. Try user-specified location
2. Fall back to Documents folder
3. Inform user where file was actually saved

---

## ? Key Features

### Permission-Safe Operation
? **No admin rights needed** - Works with standard user account  
? **Safe default locations** - Uses Windows best practices  
? **Graceful error handling** - Never crashes due to permissions  
? **Clear feedback** - Logs show exactly where files were saved  

### Enhanced CLI Mode
? **Honors output file parameter** - Saves to exact path you specify  
? **Format auto-detection** - Determines format from file extension  
? **Fallback protection** - Redirects to Documents if path is protected  
? **Better error messages** - Clear indication of what happened  

---

## ?? Download & Installation

### For End Users
1. **Download**: Get the latest release from [GitHub Releases](https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/tag/v1.3.0)
2. **Extract**: Unzip to any folder (no installation required)
3. **Run**: Double-click `AztecQRGenerator.exe`
4. **No Admin**: Works immediately without administrator privileges!

### For Developers
```bash
# NuGet Package (coming soon)
Install-Package AztecQRGenerator.Core -Version 1.3.0

# Or clone from GitHub
git clone https://github.com/johanhenningsson4-hash/AztecQRGenerator.git
cd AztecQRGenerator
git checkout v1.3.0
```

---

## ?? Quick Start

### GUI Mode
```bash
# Just double-click the executable
AztecQRGenerator.exe
```

### CLI Mode
```bash
# Generate QR code to specific location
AztecQRGenerator.exe QR "SGVsbG8gV29ybGQh" "C:\MyQRCodes\output.png" 300 2

# If C:\MyQRCodes doesn't exist or is protected:
# ? Automatically saved to Documents\AztecQRGenerator\Output\output.png
# ? You'll see a message indicating the actual location
```

### API Usage
```csharp
using AztecQR;
using System.Drawing;

// Generate in memory
var gen = new QRGenerator();
Bitmap qr = gen.GenerateQRCodeAsBitmap("SGVsbG8gV29ybGQh", 2, 300);

// Save to file (automatically uses safe location if needed)
gen.GenerateQRCodeToFile("SGVsbG8gV29ybGQh", 2, 300, "qr.png", ImageFormat.Png);

// Check where logs are stored
string logPath = Logger.Instance.GetLogFilePath();
Console.WriteLine($"Logs: {logPath}");
```

---

## ?? Where Are My Files?

### Quick Access
Press `Win + R` and paste:

**View Logs:**
```
%LocalAppData%\AztecQRGenerator\Logs
```

**View Output:**
```
%UserProfile%\Documents\AztecQRGenerator\Output
```

### Typical Locations
For user "John":
- **Logs**: `C:\Users\John\AppData\Local\AztecQRGenerator\Logs\`
- **Output**: `C:\Users\John\Documents\AztecQRGenerator\Output\`

---

## ?? What Changed Under the Hood

### Logger Improvements
- **Old**: Tried to create `Logs\` in application directory (often failed)
- **New**: Uses AppData with smart fallback hierarchy
- **Benefit**: Always works, no permission errors

### File Saving Improvements
- **Old**: Would fail with "Access Denied" on protected paths
- **New**: Automatically redirects to Documents folder
- **Benefit**: Files always save successfully

### Error Messages
- **Old**: "Access Denied" with no helpful information
- **New**: "Saved to Documents folder instead. Check log for details."
- **Benefit**: Users know exactly what happened

---

## ?? Upgrade Guide

### From v1.2.x
No code changes required! The application will:
- Create new log files in AppData (old logs remain in place)
- Use Documents folder for new files (old files remain in place)
- Work exactly as before, just better

### From v1.1.x or earlier
Same as above - fully backward compatible!

---

## ? Testing & Verification

### Automated Test
Run the included test script:
```powershell
.\Test-PermissionFix.ps1
```

This verifies:
- ? Application runs without admin
- ? Logs are created in correct location
- ? Files save successfully
- ? Protected paths are handled gracefully

### Manual Test
1. Run without admin: `AztecQRGenerator.exe`
2. Generate a code (GUI or CLI)
3. Check `%LocalAppData%\AztecQRGenerator\Logs` for log file
4. Check `Documents\AztecQRGenerator\Output` for output file

---

## ?? Bug Fixes

- **Fixed**: Application requiring administrator privileges
- **Fixed**: "Access Denied" errors when creating log files
- **Fixed**: File save failures in protected directories
- **Fixed**: Unclear error messages about file locations

---

## ?? Statistics

| Metric | Value |
|--------|-------|
| Lines of Code | ~2,500 |
| Files Modified | 3 |
| New Features | 4 |
| Bug Fixes | 4 |
| Breaking Changes | 0 |
| Dependencies | 1 (ZXing.Net) |
| Target Framework | .NET Framework 4.7.2 |

---

## ?? Acknowledgments

Thanks to everyone who reported permission issues and provided feedback!

---

## ?? Documentation

- **[README.md](README.md)** - Project overview and getting started
- **[CHANGELOG.md](CHANGELOG.md)** - Complete version history
- **[PERMISSION_FIX_SUMMARY.md](PERMISSION_FIX_SUMMARY.md)** - Detailed technical documentation
- **[USAGE_EXAMPLES.md](USAGE_EXAMPLES.md)** - Code examples and best practices
- **[IMAGE_FORMAT_GUIDE.md](IMAGE_FORMAT_GUIDE.md)** - Format selection guide

---

## ?? What's Next?

### Coming in v1.4.0
- Color customization (custom foreground/background colors)
- Batch processing from CSV/JSON files
- Custom margins support
- XML documentation on all public methods

### Long-term Roadmap
- .NET Core / .NET 6+ version
- Additional barcode formats (Data Matrix, PDF417)
- Web API version
- Cross-platform support

---

## ?? Feedback & Support

Having issues or suggestions? We'd love to hear from you!

- **Report a Bug**: [GitHub Issues](https://github.com/johanhenningsson4-hash/AztecQRGenerator/issues)
- **Request a Feature**: [GitHub Issues](https://github.com/johanhenningsson4-hash/AztecQRGenerator/issues)
- **View Source**: [GitHub Repository](https://github.com/johanhenningsson4-hash/AztecQRGenerator)

---

## ?? License

MIT License - See [LICENSE](LICENSE) file for details.

Copyright © 2025 Johan Olof Henningsson

---

**Enjoy the new release! ??**

*No more permission headaches!*
