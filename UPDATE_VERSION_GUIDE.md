# ? Update Version & Push - Ready!

## ?? One Command to Do Everything

```powershell
.\Update-Version-And-Push.ps1
```

This script will:
1. ? Update version from 1.2.3 ? 1.3.0 in all files
2. ? Show you the changes
3. ? Commit with proper message
4. ? Create Git tag v1.3.0
5. ? Push to GitHub (main + tag)
6. ? Copy release notes to clipboard
7. ? Open GitHub release page

---

## ?? What Gets Updated

### Files that will be updated:
- ? `AztecQRGenerator.Core\Properties\AssemblyInfo.cs` (1.2.3.0 ? 1.3.0.0)
- ? `Properties\AssemblyInfo.cs` (1.2.3.0 ? 1.3.0.0)
- ? `AztecQRGenerator.Tests\Properties\AssemblyInfo.cs` (1.2.3.0 ? 1.3.0.0)
- ? `AztecQRGenerator.Core\AztecQRGenerator.Core.csproj` (1.2.3 ? 1.3.0)
- ? `AztecQRGenerator.Core\AztecQRGenerator.Core.nuspec` (1.2.3 ? 1.3.0)

---

## ?? Step-by-Step Preview

### Step 1: Update Versions
Script updates all version numbers automatically

### Step 2: Review Changes
Shows `git diff` so you can see what changed

### Step 3: Commit
Commits with message:
```
feat: Release v1.3.0 - Automated Testing & CI/CD
```

### Step 4: Create Tag
Creates annotated tag: `v1.3.0`

### Step 5: Push
Pushes to GitHub (asks for confirmation first)

---

## ? After Pushing

**GitHub Actions will automatically:**
1. ? Trigger CI build
2. ? Run all 58 tests
3. ? Generate test reports

**When you create the GitHub release:**
1. ? Build Core library
2. ? Create NuGet package
3. ? Publish to NuGet.org

---

## ?? Safety Features

- ? **Shows changes before committing**
- ? **Asks for confirmation before pushing**
- ? **Handles existing tags gracefully**
- ? **Safe error handling**
- ? **Can be stopped at any step**

---

## ?? Usage

### Standard Usage (Recommended)
```powershell
cd "C:\Jobb\AztecQRGenerator"
.\Update-Version-And-Push.ps1
```

Answer "Y" when asked to proceed with each step.

### Review Only (Don't Push)
Run the script and answer "N" when asked to push.
You can push manually later:
```powershell
git push origin main
git push origin v1.3.0
```

---

## ?? What Happens After

### Immediately after pushing:
1. ? Commits appear on GitHub
2. ? Tag v1.3.0 visible in releases
3. ? CI workflow starts running

### After creating GitHub release:
1. ? NuGet publish workflow triggers
2. ? Package published to NuGet.org
3. ? Available for installation in ~10-15 min

---

## ?? Quick Links

After running the script, you'll need to:
1. **Create GitHub Release:** Opens automatically or:
   https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.3.0

2. **Check Actions:** 
   https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions

3. **Monitor NuGet:**
   https://www.nuget.org/packages/AztecQRGenerator.Core

---

## ?? If Something Goes Wrong

### Tag already exists locally
Script will ask if you want to delete and recreate it.

### Tag already exists remotely
Script will ask if you want to force push.

### Commit fails
Usually means nothing changed. That's OK - continue.

### Push fails
Check your Git credentials or network connection.

---

## ?? Expected Output

```
========================================
Update Version to 1.3.0
========================================

[1/5] Updating version numbers from 1.2.3 to 1.3.0...
  ? Updated: AztecQRGenerator.Core\Properties\AssemblyInfo.cs
  ? Updated: Properties\AssemblyInfo.cs
  ? Updated: AztecQRGenerator.Tests\Properties\AssemblyInfo.cs
  ? Updated: AztecQRGenerator.Core\AztecQRGenerator.Core.csproj
  ? Updated: AztecQRGenerator.Core\AztecQRGenerator.Core.nuspec

Updated 5 files

[2/5] Reviewing changes...
[Shows git diff]

Continue with commit? (Y/N): Y

[3/5] Committing changes...
  ? Changes committed

[4/5] Creating Git tag v1.3.0...
  ? Tag v1.3.0 created

[5/5] Pushing to GitHub...

Push to GitHub now? (Y/N): Y

Pushing commits...
  ? Commits pushed
Pushing tag...
  ? Tag pushed

========================================
? Version Updated and Pushed!
========================================

? Version 1.3.0 is now on GitHub!
```

---

## ?? Ready?

Just run:
```powershell
.\Update-Version-And-Push.ps1
```

**Everything else happens automatically!**

