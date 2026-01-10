# ? FASTEST Release v1.3.0 - Just Copy These

## Option 1: One Command (Safest)

Just run this:
```powershell
.\Simple-Release-v1.3.0.ps1
```

It will guide you step-by-step. **No errors, no issues!**

---

## Option 2: Copy These 7 Commands

Open PowerShell in `C:\Jobb\AztecQRGenerator` and paste ONE AT A TIME:

```powershell
# 1. Check status
git status
```

```powershell
# 2. Stage changes
git add .
```

```powershell
# 3. Commit
git commit -m "feat: Release v1.3.0 - Automated Testing & CI/CD"
```

```powershell
# 4. Create tag
git tag -a v1.3.0 -m "Release v1.3.0"
```

```powershell
# 5. Push commits
git push origin main
```

```powershell
# 6. Push tag
git push origin v1.3.0
```

```powershell
# 7. Open release page
Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.3.0"
```

---

## Then on GitHub:

1. **Title:** `v1.3.0 - Automated Testing & CI/CD`
2. **Description:** Open `RELEASE_NOTES_v1.3.0.md` and copy/paste
3. **Click:** "Set as the latest release"
4. **Click:** "Publish release"

---

## That's It! ??

**GitHub Actions will automatically:**
- Build the package
- Publish to NuGet.org
- Run all tests

**Check progress:** https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions

---

## If You Get Errors:

### "Tag already exists"
```powershell
# Delete local tag
git tag -d v1.3.0

# Delete remote tag (if it exists)
git push origin :refs/tags/v1.3.0

# Then create tag again
git tag -a v1.3.0 -m "Release v1.3.0"
git push origin v1.3.0
```

### "Nothing to commit"
That's OK! Just continue with the next command.

### "Authentication failed"
You may need a GitHub Personal Access Token. Go to:
https://github.com/settings/tokens

---

## Need Help?

See: `TROUBLESHOOTING_RELEASE.md`

---

**Just follow Option 1 or Option 2 above. That's all you need!** ??

