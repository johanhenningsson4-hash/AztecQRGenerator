# Quick Reference: NuGet Publishing Commands

## ?? One Manual Step First!

**IMPORTANT:** Close Visual Studio, edit `AztecQRGenerator.Core.csproj`, change version from 1.2.0 to 1.2.1, save, and reopen.

---

## ?? Publishing Workflow (Copy & Paste)

### 1?? Clean & Build Release
```powershell
cd C:\Jobb\AztecQRGenerator\AztecQRGenerator.Core
dotnet clean --configuration Release
dotnet build --configuration Release
```

### 2?? Verify Package Location
```powershell
# Package should be here:
dir "bin\Release\AztecQRGenerator.Core.1.2.1.nupkg"
```

### 3?? Test Locally (Optional)
```powershell
# Create test project
cd C:\Temp
dotnet new console -n TestPackage
cd TestPackage

# Install local package
dotnet add package AztecQRGenerator.Core --version 1.2.1 --source C:\Jobb\AztecQRGenerator\AztecQRGenerator.Core\bin\Release

# Verify
dotnet list package
```

### 4?? Publish to NuGet.org

**Get API Key:**
1. Go to: https://www.nuget.org/account/apikeys
2. Create key: Name=`AztecQRGenerator`, Glob pattern=`AztecQRGenerator.*`, Select `Push`
3. Copy the key

**Push Package:**
```powershell
cd C:\Jobb\AztecQRGenerator\AztecQRGenerator.Core

# Replace YOUR_API_KEY with actual key
dotnet nuget push "bin\Release\AztecQRGenerator.Core.1.2.1.nupkg" `
    --api-key YOUR_API_KEY `
    --source https://api.nuget.org/v3/index.json
```

### 5?? Tag & Push to GitHub
```powershell
cd C:\Jobb\AztecQRGenerator

git add .
git commit -m "Release v1.2.1 - Remove unused parameters"
git tag -a v1.2.1 -m "Release version 1.2.1"
git push origin NUGET_PUBLISHING
git push origin v1.2.1
```

### 6?? Create GitHub Release
1. Visit: https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new
2. Choose tag: `v1.2.1`
3. Title: `v1.2.1 - Code Cleanup`
4. Description: Copy from `CHANGELOG.md`
5. Attach: `AztecQRGenerator.Core.1.2.1.nupkg`
6. Click: **Publish release**

---

## ?? Verification Commands

### Check Build Output
```powershell
Get-ChildItem bin\Release\*.nupkg
```

### Inspect Package Contents
```powershell
# Extract package
Expand-Archive "bin\Release\AztecQRGenerator.Core.1.2.1.nupkg" -DestinationPath "PackageContents" -Force

# View contents
Get-ChildItem PackageContents -Recurse | Select-Object FullName
```

### Verify Package on NuGet.org (after publishing)
```powershell
# Search for package
dotnet nuget list AztecQRGenerator.Core --source https://api.nuget.org/v3/index.json

# Install from NuGet.org (wait 10-15 minutes after push)
dotnet add package AztecQRGenerator.Core --version 1.2.1
```

---

## ?? Troubleshooting

### Package Already Exists
**Error:** "409 Conflict - A package with ID 'AztecQRGenerator.Core' and version '1.2.1' already exists"

**Solution:** You cannot replace a published version. Increment to 1.2.2 or 2.0.0

### Invalid API Key
**Error:** "403 Forbidden"

**Solution:** 
1. Check API key is correct
2. Verify key has `Push` permission
3. Ensure key glob pattern matches `AztecQRGenerator.*`

### Package Not Found After Publishing
**Solution:** Wait 10-15 minutes for NuGet.org indexing

### Build Errors
**Solution:**
1. Clean solution: `dotnet clean`
2. Restore packages: `dotnet restore`
3. Build again: `dotnet build --configuration Release`

---

## ?? Pre-Flight Checklist

Before running commands above:

- [ ] Closed Visual Studio
- [ ] Edited `.csproj` version to 1.2.1
- [ ] Reopened Visual Studio
- [ ] Built successfully in Release mode
- [ ] Verified no errors or warnings
- [ ] Have NuGet.org API key ready

---

## ?? Breaking Change Version Alternative

If you want to follow semantic versioning strictly (recommended):

**Use version 2.0.0 instead of 1.2.1**

1. Change all `1.2.1` references to `2.0.0` in:
   - AztecQRGenerator.Core.csproj
   - AssemblyInfo.cs
   - CHANGELOG.md

2. Update all commands above replacing `1.2.1` with `2.0.0`

3. Publish as major version due to breaking changes

---

## ?? Important Links

- **NuGet Package Manager:** https://www.nuget.org/packages/AztecQRGenerator.Core/
- **GitHub Repository:** https://github.com/johanhenningsson4-hash/AztecQRGenerator
- **NuGet API Keys:** https://www.nuget.org/account/apikeys
- **GitHub Releases:** https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases

---

## ?? Help

For detailed explanations, see:
- `PUBLISHING_SUMMARY.md` - Overview and status
- `NUGET_PUBLISHING_CHECKLIST.md` - Detailed step-by-step guide
- `CHANGELOG.md` - Version history and migration guide

---

**Ready?** Start with step 1 above! ??
