# ?? Publish v1.3.0 to NuGet - Complete Guide

## ? Recommended: Automated via GitHub Actions

This is the **easiest and safest** way. GitHub Actions will handle everything automatically.

### Prerequisites Check
1. ? Version already updated to 1.3.0? ? Run `.\Update-Version-And-Push.ps1` if not
2. ? Code pushed to GitHub? ? Check https://github.com/johanhenningsson4-hash/AztecQRGenerator
3. ? NuGet API Key added to GitHub? ? Check Settings ? Secrets ? Actions ? NUGET_API_KEY

### Step 1: Create GitHub Release

```powershell
# Open the release page
Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.3.0"
```

**Fill in:**
- **Tag:** v1.3.0 (should be auto-selected)
- **Title:** `v1.3.0 - Automated Testing & CI/CD`
- **Description:** Copy from `RELEASE_NOTES_v1.3.0.md`
- **Check:** ? "Set as the latest release"
- **Click:** "Publish release"

### Step 2: Wait for GitHub Actions

Go to: https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions

You'll see:
1. ? "NuGet Package Publish" workflow starts automatically
2. ? Builds Core library
3. ? Creates NuGet package
4. ? Publishes to NuGet.org

**Time:** ~2-5 minutes

### Step 3: Verify on NuGet

After 10-15 minutes:
https://www.nuget.org/packages/AztecQRGenerator.Core/1.3.0

**That's it!** GitHub Actions did all the work! ??

---

## ?? Setup NuGet API Key (If Not Done)

If you haven't added the NuGet API key to GitHub:

### Get Your API Key
1. Go to: https://www.nuget.org/account/apikeys
2. Click "Create"
3. **Key Name:** `AztecQRGenerator`
4. **Glob Pattern:** `AztecQRGenerator.*`
5. **Select Scopes:** Check "Push"
6. Click "Create"
7. **Copy the key** (you won't see it again!)

### Add to GitHub
1. Go to: https://github.com/johanhenningsson4-hash/AztecQRGenerator/settings/secrets/actions
2. Click "New repository secret"
3. **Name:** `NUGET_API_KEY`
4. **Value:** Paste your API key
5. Click "Add secret"

Now GitHub Actions can publish automatically!

---

## ?? Alternative: Manual NuGet Publishing

If you prefer to publish manually or GitHub Actions isn't working:

### Step 1: Download/Locate NuGet.exe

```powershell
# Download if not present
$nugetUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
Invoke-WebRequest -Uri $nugetUrl -OutFile "nuget.exe"
```

### Step 2: Build Package

```powershell
cd "C:\Jobb\AztecQRGenerator\AztecQRGenerator.Core"

# Restore packages
..\nuget.exe restore

# Build in Release mode
msbuild AztecQRGenerator.Core.csproj /p:Configuration=Release

# Create package
..\nuget.exe pack AztecQRGenerator.Core.nuspec -Properties Configuration=Release
```

This creates: `AztecQRGenerator.Core.1.3.0.nupkg`

### Step 3: Publish to NuGet

```powershell
# Get your API key from https://www.nuget.org/account/apikeys
$apiKey = Read-Host "Enter your NuGet API key" -AsSecureString
$apiKeyPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($apiKey))

# Publish
..\nuget.exe push AztecQRGenerator.Core.1.3.0.nupkg -ApiKey $apiKeyPlain -Source https://api.nuget.org/v3/index.json

# Or with plain text key (less secure)
# ..\nuget.exe push AztecQRGenerator.Core.1.3.0.nupkg -ApiKey YOUR_API_KEY -Source https://api.nuget.org/v3/index.json
```

### Step 4: Wait for Indexing

- **Immediate:** Package uploaded
- **~5 minutes:** Package visible at direct URL
- **~15 minutes:** Searchable on NuGet.org
- **~30 minutes:** Fully indexed

---

## ?? What Gets Published

### Package Information
- **Package ID:** AztecQRGenerator.Core
- **Version:** 1.3.0
- **Target Framework:** .NET Framework 4.7.2
- **Dependencies:** ZXing.Net 0.16.9

### Files Included
- ? AztecQRGenerator.Core.dll
- ? AztecQRGenerator.Core.xml (documentation)
- ? NUGET_README.md
- ? icon.png
- ? License file (MIT)

---

## ? Verification Steps

### 1. Check Package Exists
```powershell
# Search for your package
dotnet nuget search AztecQRGenerator.Core
```

### 2. Test Installation
```powershell
# Create test project
cd C:\Temp
dotnet new console -n TestInstall
cd TestInstall

# Install your package
dotnet add package AztecQRGenerator.Core --version 1.3.0

# Verify
dotnet list package
```

### 3. Check Package Page
https://www.nuget.org/packages/AztecQRGenerator.Core/1.3.0

Should show:
- ? Version 1.3.0
- ? Download count
- ? Dependencies
- ? README content
- ? Release notes

---

## ?? Complete Checklist

### Before Publishing
- [ ] Version updated to 1.3.0 in all files
- [ ] All tests passing (58/58)
- [ ] Code committed to Git
- [ ] Tag v1.3.0 created
- [ ] Pushed to GitHub
- [ ] GitHub release created

### Publishing (Automated)
- [ ] NuGet API key added to GitHub secrets
- [ ] GitHub release published
- [ ] GitHub Actions workflow running
- [ ] Workflow completed successfully
- [ ] Package visible on NuGet.org

### Publishing (Manual)
- [ ] NuGet.exe available
- [ ] Package built successfully
- [ ] Package pushed to NuGet.org
- [ ] Package indexed and searchable

### After Publishing
- [ ] Package searchable on NuGet.org
- [ ] Installation works: `Install-Package AztecQRGenerator.Core -Version 1.3.0`
- [ ] README displays correctly
- [ ] Dependencies listed correctly

---

## ?? Troubleshooting

### "Package already exists"
The package version already exists on NuGet. You need to:
- Increment version (e.g., 1.3.1)
- Update all version files
- Rebuild and republish

### "Invalid API Key"
- Regenerate key at https://www.nuget.org/account/apikeys
- Update GitHub secret
- Try publishing again

### "GitHub Actions workflow not triggering"
- Check workflows are enabled: Settings ? Actions ? General
- Verify tag was pushed: `git push origin v1.3.0`
- Check workflow file exists: `.github/workflows/nuget-publish.yml`

### "Package not appearing"
- Wait 15-30 minutes for full indexing
- Check directly: https://www.nuget.org/packages/AztecQRGenerator.Core/1.3.0
- Check workflow logs for errors

---

## ?? Publishing Methods Comparison

| Method | Time | Difficulty | Recommended For |
|--------|------|------------|-----------------|
| **GitHub Actions** | 5 min | Easy | ? Everyone |
| **Manual via Script** | 10 min | Medium | Advanced users |
| **Visual Studio** | 15 min | Easy | Visual Studio users |
| **Command Line** | 10 min | Hard | Experts |

---

## ?? Success!

After publishing, users can install with:

```powershell
Install-Package AztecQRGenerator.Core -Version 1.3.0
```

Or:

```bash
dotnet add package AztecQRGenerator.Core --version 1.3.0
```

---

## ?? Quick Commands

### Copy release notes to clipboard
```powershell
Get-Content RELEASE_NOTES_v1.3.0.md -Raw | Set-Clipboard
```

### Open GitHub release page
```powershell
Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.3.0"
```

### Open NuGet package page
```powershell
Start-Process "https://www.nuget.org/packages/AztecQRGenerator.Core"
```

### Check GitHub Actions
```powershell
Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions"
```

---

## ?? Recommended Flow

**For v1.3.0 publication:**

1. ? Run: `.\Update-Version-And-Push.ps1` (if not done)
2. ? Create GitHub Release with v1.3.0 tag
3. ? Wait 5 minutes for GitHub Actions
4. ? Verify on NuGet.org
5. ? Test installation
6. ? Celebrate! ??

**Total time:** ~20 minutes (mostly waiting)

---

**That's it! GitHub Actions makes publishing effortless!** ??

