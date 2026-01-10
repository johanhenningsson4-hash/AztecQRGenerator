# ?? Release v1.3.0 - Quick Guide

## Fastest Way - Run the Automated Script

```powershell
cd "C:\Jobb\AztecQRGenerator"
.\Create-Release-v1.3.0.ps1
```

The script will:
1. ? Update version to 1.3.0 in all files
2. ? Build solution in Release mode
3. ? Run all 58 tests
4. ? Create NuGet package
5. ? Commit changes
6. ? Create and push Git tag
7. ? Copy release notes to clipboard
8. ? Open GitHub release page

Then just **paste release notes and publish!**

---

## What's in This Release

### ?? Major Features
- ? **58 Unit Tests** - Comprehensive test coverage
- ? **GitHub Actions CI/CD** - Automated testing on every push
- ? **Auto NuGet Publishing** - Publishes on GitHub releases
- ? **Code Quality Scans** - Weekly security analysis

### ?? Changes
- Added NUnit test project
- Added 3 GitHub Actions workflows
- Fixed deprecated examples
- Enhanced documentation
- **No breaking changes!**

---

## Manual Steps (if needed)

### 1. Update Versions
Edit these files and change `1.2.3` to `1.3.0`:
- `AztecQRGenerator.Core\Properties\AssemblyInfo.cs`
- `AztecQRGenerator.Core\AztecQRGenerator.Core.csproj`
- `AztecQRGenerator.Core\AztecQRGenerator.Core.nuspec`
- `Properties\AssemblyInfo.cs`
- `AztecQRGenerator.Tests\Properties\AssemblyInfo.cs`

### 2. Build & Test
```powershell
# Build
msbuild AztecQRGenerator.sln /p:Configuration=Release

# Test
.\packages\NUnit.ConsoleRunner.3.16.3\tools\nunit3-console.exe `
  AztecQRGenerator.Tests\bin\Release\AztecQRGenerator.Tests.dll
```

### 3. Create NuGet Package
```powershell
cd AztecQRGenerator.Core
nuget pack AztecQRGenerator.Core.nuspec -Properties Configuration=Release
cd ..
```

### 4. Commit & Tag
```powershell
git add .
git commit -m "feat: Release v1.3.0 - Automated Testing & CI/CD"
git tag -a v1.3.0 -m "Release v1.3.0"
git push origin main
git push origin v1.3.0
```

### 5. Create GitHub Release
Go to: https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.3.0

**Title:** `v1.3.0 - Automated Testing & CI/CD`

**Description:** Copy from `RELEASE_NOTES_v1.3.0.md`

**Attach:** `AztecQRGenerator.Core\AztecQRGenerator.Core.1.3.0.nupkg`

**Check:** "Set as the latest release"

**Click:** "Publish release"

---

## After Publishing

### Automatic Actions
GitHub Actions will:
1. ? Trigger NuGet publish workflow
2. ? Build Core library
3. ? Create package
4. ? Publish to NuGet.org

### Verification (after 10-15 min)
- Check NuGet: https://www.nuget.org/packages/AztecQRGenerator.Core/1.3.0
- Check Actions: https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions

---

## Test the Release

```powershell
# Create test project
dotnet new console -n TestRelease
cd TestRelease

# Install package
dotnet add package AztecQRGenerator.Core --version 1.3.0

# Verify
dotnet list package
```

---

## Key Changes from v1.2.3

| Feature | v1.2.3 | v1.3.0 |
|---------|--------|--------|
| Unit Tests | 0 | 58 ? |
| CI/CD | Manual | Automated ? |
| Test Coverage | None | Excellent ? |
| NuGet Publish | Manual | Automated ? |
| Code Quality | Manual | Automated ? |

---

## ?? Success Criteria

- [x] Version bumped to 1.3.0
- [x] All 58 tests pass
- [x] Build successful
- [x] NuGet package created
- [ ] GitHub release published
- [ ] NuGet.org updated (auto via GitHub Actions)

---

## ?? Important Links

- **GitHub Release:** https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.3.0
- **Current Releases:** https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases
- **Actions:** https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions
- **NuGet Package:** https://www.nuget.org/packages/AztecQRGenerator.Core

---

**? Run `.\Create-Release-v1.3.0.ps1` to automate everything!**

