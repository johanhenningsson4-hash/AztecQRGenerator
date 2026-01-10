# Troubleshooting Release v1.3.0

## Issue: "nuget command not found"

### Solution 1: Use Visual Studio (Easiest)
1. Open `AztecQRGenerator.sln` in Visual Studio
2. Build solution (Ctrl+Shift+B)
3. Run tests from Test Explorer
4. Use Package Manager Console for NuGet commands:
   ```
   Tools ? NuGet Package Manager ? Package Manager Console
   ```

### Solution 2: Download NuGet.exe
```powershell
# Download NuGet.exe
$nugetUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
Invoke-WebRequest -Uri $nugetUrl -OutFile "C:\Jobb\AztecQRGenerator\nuget.exe"

# Verify it works
.\nuget.exe
```

### Solution 3: Use the Simple Script
```powershell
.\Simple-Release-v1.3.0.ps1
```

This script guides you through manual steps without needing NuGet in PATH.

---

## Issue: Test runner not found

### Solution: Install via Visual Studio
1. Open Visual Studio
2. Tools ? NuGet Package Manager ? Manage NuGet Packages for Solution
3. Search for "NUnit.ConsoleRunner"
4. Install version 3.16.3

### Or skip tests:
```powershell
.\Create-Release-v1.3.0.ps1 -SkipTests
```

---

## Issue: Build fails

### Check:
1. ? Visual Studio 2019 or later installed?
2. ? .NET Framework 4.7.2 SDK installed?
3. ? All NuGet packages restored?

### Solution:
```powershell
# Restore packages
nuget restore AztecQRGenerator.sln

# Or in Visual Studio:
# Right-click solution ? Restore NuGet Packages
```

---

## Issue: Version not updating

### Manual Update:
Open these files and change `1.2.3` to `1.3.0`:

1. **AztecQRGenerator.Core\Properties\AssemblyInfo.cs**
   ```csharp
   [assembly: AssemblyVersion("1.3.0.0")]
   [assembly: AssemblyFileVersion("1.3.0.0")]
   ```

2. **AztecQRGenerator.Core\AztecQRGenerator.Core.csproj**
   ```xml
   <Version>1.3.0</Version>
   ```

3. **AztecQRGenerator.Core\AztecQRGenerator.Core.nuspec**
   ```xml
   <version>1.3.0</version>
   ```

4. **Properties\AssemblyInfo.cs**
   ```csharp
   [assembly: AssemblyVersion("1.3.0.0")]
   [assembly: AssemblyFileVersion("1.3.0.0")]
   ```

5. **AztecQRGenerator.Tests\Properties\AssemblyInfo.cs**
   ```csharp
   [assembly: AssemblyVersion("1.3.0.0")]
   [assembly: AssemblyFileVersion("1.3.0.0")]
   ```

---

## Issue: GitHub Actions not running

### Check:
1. ? Workflows enabled? Go to: Settings ? Actions ? General
2. ? NUGET_API_KEY secret added? Go to: Settings ? Secrets ? Actions

### Add NuGet Secret:
1. Go to https://www.nuget.org/account/apikeys
2. Create new key
3. Copy the key
4. Go to GitHub: Settings ? Secrets and variables ? Actions
5. New secret: Name = `NUGET_API_KEY`, Value = (paste key)

---

## Issue: Can't push to GitHub

### Check permissions:
```powershell
# Test Git connection
git remote -v

# If needed, configure:
git config user.name "Your Name"
git config user.email "your@email.com"
```

### Authentication:
If using HTTPS, you may need a Personal Access Token:
1. Go to GitHub Settings ? Developer settings ? Personal access tokens
2. Generate new token with `repo` permissions
3. Use token as password when pushing

---

## Alternative: Manual Release Without Scripts

### Step-by-Step:

1. **Update Versions** (see above for files to edit)

2. **Build in Visual Studio**
   - Select "Release" configuration
   - Build ? Rebuild Solution

3. **Run Tests**
   - Test ? Run All Tests
   - Verify 58 tests pass

4. **Create Package** (in Package Manager Console)
   ```
   cd AztecQRGenerator.Core
   nuget pack AztecQRGenerator.Core.nuspec -Properties Configuration=Release
   ```

5. **Commit**
   ```powershell
   git add .
   git commit -m "feat: Release v1.3.0"
   ```

6. **Tag**
   ```powershell
   git tag -a v1.3.0 -m "Release v1.3.0"
   ```

7. **Push**
   ```powershell
   git push origin main
   git push origin v1.3.0
   ```

8. **Create GitHub Release**
   - Go to: https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new
   - Select tag: v1.3.0
   - Title: v1.3.0 - Automated Testing & CI/CD
   - Paste description from RELEASE_NOTES_v1.3.0.md
   - Attach .nupkg file (if created)
   - Publish

---

## Verification

### After release, verify:

1. **GitHub Release**
   https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases

2. **GitHub Actions**
   https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions
   - Should see workflow running

3. **NuGet Package** (after 10-15 min)
   https://www.nuget.org/packages/AztecQRGenerator.Core/1.3.0

---

## Get Help

### Useful Commands:

```powershell
# Check Git status
git status

# Check current branch
git branch

# View recent commits
git log --oneline -5

# Check remote
git remote -v

# Test build
msbuild AztecQRGenerator.sln /p:Configuration=Release
```

### Logs:
- Build errors: Check Output window in Visual Studio
- Test results: Check Test Explorer
- GitHub Actions: Check Actions tab on GitHub

---

## Quick Reset (if something goes wrong)

```powershell
# Undo uncommitted changes
git checkout .

# Remove untracked files
git clean -fd

# Delete local tag
git tag -d v1.3.0

# Delete remote tag (if already pushed)
git push origin :refs/tags/v1.3.0
```

---

## Contact

If you encounter issues not covered here:
1. Check GitHub Actions logs
2. Review error messages carefully
3. Ensure all prerequisites are installed
4. Try the Simple-Release script instead

**Remember:** GitHub Actions will handle NuGet publishing automatically once you create the GitHub release with the NUGET_API_KEY secret configured!

