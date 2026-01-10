# ?? Publish to NuGet - Quick Reference

## ? FASTEST Way (Recommended)

### One Command:
```powershell
.\Complete-NuGet-Publish.ps1
```

This handles EVERYTHING:
1. ? Checks version is 1.3.0
2. ? Verifies Git status
3. ? Confirms tag exists on GitHub
4. ? Opens GitHub release page
5. ? Monitors GitHub Actions
6. ? Verifies NuGet publication

---

## ?? Step-by-Step (If Needed)

### 1. Update Version & Push (if not done)
```powershell
.\Update-Version-And-Push.ps1
```

### 2. Create GitHub Release
```powershell
Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.3.0"
```

**Fill in:**
- Title: `v1.3.0 - Automated Testing & CI/CD`
- Description: Copy from `RELEASE_NOTES_v1.3.0.md`
- Check: "Set as the latest release"
- Click: "Publish release"

### 3. GitHub Actions Does the Rest!
- Builds package
- Publishes to NuGet.org
- Takes ~5 minutes

### 4. Verify
```powershell
Start-Process "https://www.nuget.org/packages/AztecQRGenerator.Core/1.3.0"
```

---

## ?? One-Time Setup: NuGet API Key

**Only needed once!**

### Get API Key:
1. Go to: https://www.nuget.org/account/apikeys
2. Create key named `AztecQRGenerator`
3. Give it "Push" permission
4. Copy the key

### Add to GitHub:
1. Go to: https://github.com/johanhenningsson4-hash/AztecQRGenerator/settings/secrets/actions
2. New secret: `NUGET_API_KEY`
3. Paste your API key
4. Save

**Done! Now GitHub Actions can publish automatically!**

---

## ? Verification Checklist

After publishing, verify:

- [ ] GitHub Actions workflow succeeded
  - Check: https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions

- [ ] Package visible on NuGet
  - Check: https://www.nuget.org/packages/AztecQRGenerator.Core/1.3.0

- [ ] Installation works
  ```powershell
  dotnet add package AztecQRGenerator.Core --version 1.3.0
  ```

---

## ?? What Gets Published

```
AztecQRGenerator.Core v1.3.0
??? Target: .NET Framework 4.7.2
??? Dependencies: ZXing.Net 0.16.9
??? Files:
?   ??? AztecQRGenerator.Core.dll
?   ??? AztecQRGenerator.Core.xml
?   ??? NUGET_README.md
?   ??? icon.png
??? Features:
    ??? 58 unit tests
    ??? CI/CD pipeline
    ??? Automated testing
    ??? Quality scanning
```

---

## ?? Timeline

| Time | Status |
|------|--------|
| Now | GitHub Actions triggered |
| 2-5 min | Package published to NuGet |
| 5-10 min | Package visible at direct URL |
| 15-30 min | Fully searchable on NuGet.org |

---

## ?? Quick Commands

```powershell
# Complete publishing (automated)
.\Complete-NuGet-Publish.ps1

# Or do it manually:

# 1. Update and push
.\Update-Version-And-Push.ps1

# 2. Open release page
Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.3.0"

# 3. Check Actions
Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions"

# 4. Check NuGet
Start-Process "https://www.nuget.org/packages/AztecQRGenerator.Core/1.3.0"
```

---

## ?? Troubleshooting

### "Tag doesn't exist"
```powershell
.\Update-Version-And-Push.ps1
```

### "API key not working"
Re-add it to GitHub Secrets (see above)

### "Package not appearing"
Wait 15-30 minutes for full indexing

### "Workflow not running"
Check: Settings ? Actions ? Enable workflows

---

## ?? Documentation

- **Complete Guide:** `PUBLISH_TO_NUGET.md`
- **This Reference:** `NUGET_QUICK_REF.md`
- **Troubleshooting:** `TROUBLESHOOTING_RELEASE.md`

---

## ?? Success!

After publishing, users install with:
```
Install-Package AztecQRGenerator.Core -Version 1.3.0
```

---

**Just run: `.\Complete-NuGet-Publish.ps1` and follow the prompts!** ??

