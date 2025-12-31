# NuGet Publishing Checklist for AztecQRGenerator.Core v1.2.1

## Pre-Publishing Checklist

### ? Code Quality
- [x] All code compiles without errors
- [x] All code compiles without warnings
- [x] Removed unused parameters (`lTaNmbrqr` from both generators)
- [x] XML documentation comments present on all public methods
- [x] Consistent coding style throughout

### ? Version Information
- [x] AssemblyInfo.cs updated to version 1.2.1
- [ ] Project file (.csproj) updated to version 1.2.1 *(Manual edit required after closing solution)*
- [x] Release notes added for version 1.2.1

### ? Required Files
- [x] `NUGET_README.md` - NuGet package description
- [x] `icon.png` - Package icon (128x128 or larger)
- [x] `LICENSE` file - MIT License *(Verify presence in repository root)*
- [x] Project file configured with NuGet metadata

### ? Documentation
- [x] README with comprehensive examples
- [x] Usage examples documented
- [x] API reference included
- [x] Troubleshooting guide included

### ?? Manual Steps Required

#### Step 1: Update Project File Version
**File:** `AztecQRGenerator.Core.csproj`

1. Close Visual Studio solution
2. Open `AztecQRGenerator.Core.csproj` in a text editor
3. Update the following line:
   ```xml
   <Version>1.2.1</Version>
   ```
4. Update release notes:
   ```xml
   <PackageReleaseNotes>
Version 1.2.1:
- Removed unused lTaNmbrqr parameter from GenerateAztecBitmap() and GenerateQRBitmap() methods
- Code cleanup and consistency improvements

Version 1.2.0:
- Added support for multiple image formats (PNG, JPEG, BMP)
- Added GenerateQRCodeToFile() and GenerateAztecCodeToFile() with format selection
- Added GenerateQRCodeAsBitmap() and GenerateAztecCodeAsBitmap() for in-memory generation
- Comprehensive logging support
- Full backward compatibility maintained
   </PackageReleaseNotes>
   ```
5. Save and reopen solution

#### Step 2: Build Release Package
```powershell
# Clean previous builds
dotnet clean --configuration Release

# Build in Release mode (generates NuGet package)
dotnet build --configuration Release

# Or use MSBuild
msbuild /p:Configuration=Release
```

The NuGet package will be created in:
```
AztecQRGenerator.Core\bin\Release\AztecQRGenerator.Core.1.2.1.nupkg
```

#### Step 3: Test the Package Locally

##### Option A: Test in Local NuGet Feed
```powershell
# Create a local NuGet feed folder
mkdir C:\LocalNuGet

# Copy the package
copy "bin\Release\AztecQRGenerator.Core.1.2.1.nupkg" "C:\LocalNuGet\"

# Add the local source in Visual Studio
# Tools -> Options -> NuGet Package Manager -> Package Sources
# Add: Name=Local, Source=C:\LocalNuGet
```

##### Option B: Test by Installing Package
```powershell
# Create a test project
mkdir TestProject
cd TestProject
dotnet new console

# Install the local package
dotnet add package AztecQRGenerator.Core --version 1.2.1 --source C:\LocalNuGet

# Test the functionality
# (Create test code using the library)
```

#### Step 4: Verify Package Contents
```powershell
# Install NuGet Package Explorer (if not installed)
# https://github.com/NuGetPackageExplorer/NuGetPackageExplorer

# Or extract and inspect manually
Expand-Archive "bin\Release\AztecQRGenerator.Core.1.2.1.nupkg" -DestinationPath "PackageContents"
```

**Verify the package contains:**
- [x] AztecQRGenerator.Core.dll
- [x] NUGET_README.md
- [x] icon.png
- [x] Metadata (version, authors, description)
- [x] Dependencies (ZXing.Net)

#### Step 5: Publish to NuGet.org

##### Prerequisites
1. Create NuGet.org account: https://www.nuget.org/users/account/LogOn
2. Generate API Key: https://www.nuget.org/account/apikeys
   - Key name: `AztecQRGenerator Publishing`
   - Glob pattern: `AztecQRGenerator.*`
   - Select: `Push` and `Push new packages and package versions`

##### Publishing Commands

**Option A: Using dotnet CLI**
```powershell
# Set your API key (one-time setup)
$env:NUGET_API_KEY = "your-api-key-here"

# Push to NuGet.org
dotnet nuget push "bin\Release\AztecQRGenerator.Core.1.2.1.nupkg" `
    --api-key $env:NUGET_API_KEY `
    --source https://api.nuget.org/v3/index.json
```

**Option B: Using nuget.exe**
```powershell
# Download nuget.exe if needed
# https://www.nuget.org/downloads

# Push to NuGet.org
nuget push "bin\Release\AztecQRGenerator.Core.1.2.1.nupkg" `
    -ApiKey your-api-key-here `
    -Source https://api.nuget.org/v3/index.json
```

**Option C: Using NuGet.org Web Interface**
1. Go to https://www.nuget.org/packages/manage/upload
2. Click "Upload" and select your `.nupkg` file
3. Review the package details
4. Click "Submit"

#### Step 6: Verify Publication
- Package appears at: https://www.nuget.org/packages/AztecQRGenerator.Core/
- Check that version 1.2.1 is listed
- Verify README displays correctly
- Verify icon displays correctly
- Check that dependencies are correct

#### Step 7: Post-Publication

##### Update GitHub Repository
```powershell
# Tag the release
git tag -a v1.2.1 -m "Release version 1.2.1 - Parameter cleanup"
git push origin v1.2.1

# Update README with installation instructions
# Update CHANGELOG.md with release notes
```

##### Create GitHub Release
1. Go to: https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases
2. Click "Draft a new release"
3. Choose tag: `v1.2.1`
4. Release title: `v1.2.1 - Code Cleanup`
5. Description:
   ```markdown
   ## Changes in v1.2.1
   
   ### Bug Fixes & Improvements
   - Removed unused `lTaNmbrqr` parameter from `GenerateAztecBitmap()` and `GenerateQRBitmap()` methods
   - Updated AssemblyInfo.cs to match package metadata
   - Code cleanup and consistency improvements
   
   ### Installation
   ```powershell
   Install-Package AztecQRGenerator.Core -Version 1.2.1
   ```
   
   ### Breaking Changes
   ?? If you were using `GenerateAztecBitmap()` or `GenerateQRBitmap()` with 4 parameters, update to 3 parameters:
   
   **Before:**
   ```csharp
   generator.GenerateAztecBitmap(1, base64String, 2, 300);
   ```
   
   **After:**
   ```csharp
   generator.GenerateAztecBitmap(base64String, 2, 300);
   ```
   ```
6. Attach the `.nupkg` file
7. Click "Publish release"

##### Test Installation
```powershell
# Create a fresh test project
dotnet new console -n InstallTest
cd InstallTest

# Install from NuGet.org (wait 10-15 minutes after publishing)
dotnet add package AztecQRGenerator.Core --version 1.2.1

# Verify installation
dotnet list package
```

## Testing Checklist

### Unit Tests (Optional but Recommended)
- [ ] Test `GenerateQRCodeAsBitmap()` with valid data
- [ ] Test `GenerateAztecCodeAsBitmap()` with valid data
- [ ] Test with invalid Base64 strings (should throw)
- [ ] Test with invalid parameters (should throw)
- [ ] Test file saving functionality
- [ ] Test memory management (no leaks)

### Integration Tests
- [ ] Generate QR code and verify scanability
- [ ] Generate Aztec code and verify scanability
- [ ] Test with Windows Forms application
- [ ] Test with Console application
- [ ] Test with ASP.NET application

## Common Issues & Solutions

### Issue 1: Package Not Found After Publishing
**Solution:** Wait 10-15 minutes for NuGet.org indexing to complete

### Issue 2: Package Validation Errors
**Solution:** Ensure all required fields in .csproj are filled:
- Authors
- Description
- License
- ProjectUrl
- RepositoryUrl

### Issue 3: Icon Not Displaying
**Solution:** 
- Verify icon.png exists in project root
- Verify icon.png is included in .csproj with `Pack="true"`
- Use PNG format, minimum 128x128 pixels

### Issue 4: README Not Displaying
**Solution:**
- Verify NUGET_README.md exists
- Verify it's referenced in .csproj with correct PackagePath

### Issue 5: Dependencies Missing
**Solution:**
- Verify ZXing.Net is listed as PackageReference in .csproj
- Check that dependency version is specified

## Post-Release Monitoring

### Week 1
- [ ] Check download statistics
- [ ] Monitor GitHub issues for bug reports
- [ ] Respond to any NuGet.org package reviews

### Ongoing
- [ ] Monitor for compatibility issues
- [ ] Track feature requests
- [ ] Plan next version improvements

## Resources

- **NuGet.org Package Page:** https://www.nuget.org/packages/AztecQRGenerator.Core/
- **GitHub Repository:** https://github.com/johanhenningsson4-hash/AztecQRGenerator
- **NuGet Documentation:** https://docs.microsoft.com/en-us/nuget/
- **Package Explorer:** https://github.com/NuGetPackageExplorer/NuGetPackageExplorer

## Notes

### Breaking Changes in v1.2.1
The removal of the `lTaNmbrqr` parameter is a **breaking change** for users who called:
- `GenerateAztecBitmap(int, string, int, int)`
- `GenerateQRBitmap(int, string, int, int)`

**Migration path:** Remove the first integer parameter from method calls.

### Semantic Versioning
According to SemVer:
- Major version (X.0.0): Breaking changes
- Minor version (0.X.0): New features (backward compatible)
- Patch version (0.0.X): Bug fixes (backward compatible)

**Consider:** This is a breaking change, so version 2.0.0 might be more appropriate than 1.2.1.

**Recommendation:** 
- Use 1.2.1 if you expect minimal usage of these specific methods
- Use 2.0.0 if the methods are widely used

---

## Ready to Publish?

Once you've completed all manual steps above and verified the checklist, you're ready to publish!

### Quick Command Summary
```powershell
# 1. Update .csproj version (manual edit)
# 2. Build release package
dotnet clean --configuration Release
dotnet build --configuration Release

# 3. Test locally
dotnet add package AztecQRGenerator.Core --version 1.2.1 --source ./bin/Release

# 4. Publish to NuGet.org
dotnet nuget push "bin\Release\AztecQRGenerator.Core.1.2.1.nupkg" `
    --api-key YOUR_API_KEY `
    --source https://api.nuget.org/v3/index.json

# 5. Tag and push to GitHub
git tag -a v1.2.1 -m "Release version 1.2.1"
git push origin v1.2.1
```

Good luck with your NuGet package release! ??
