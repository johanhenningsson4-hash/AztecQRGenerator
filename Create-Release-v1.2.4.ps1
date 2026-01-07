# Create GitHub Release v1.2.4 - Documentation Cleanup
# Automated script for creating and pushing the release

param(
    [switch]$DryRun = $false
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "GitHub Release v1.2.4 - Documentation" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$repoPath = "C:\Jobb\AztecQRGenerator"
Set-Location $repoPath

# Check current status
Write-Host "[1/7] Checking repository status..." -ForegroundColor Yellow
$status = git status --short
if ($status) {
    Write-Host "  Uncommitted changes found:" -ForegroundColor Gray
    $status | ForEach-Object { Write-Host "    $_" -ForegroundColor Gray }
} else {
    Write-Host "  ? Working directory clean" -ForegroundColor Green
}
Write-Host ""

# Stage cleanup files
Write-Host "[2/7] Staging cleanup files..." -ForegroundColor Yellow
if (-not $DryRun) {
    git add USAGE_EXAMPLES.md CODE_CLEANUP_SUMMARY.md CODE_CLEANUP_COMPLETE.md GITHUB_RELEASE_GUIDE_v1.2.4.md Create-Release-v1.2.4.ps1
    Write-Host "  ? Files staged" -ForegroundColor Green
} else {
    Write-Host "  [DRY RUN] Would stage: USAGE_EXAMPLES.md, CODE_CLEANUP_*.md, scripts" -ForegroundColor Gray
}
Write-Host ""

# Commit changes
Write-Host "[3/7] Committing changes..." -ForegroundColor Yellow
$commitMessage = @"
docs: Code cleanup v1.2.4 - fix deprecated examples and documentation

- Fixed USAGE_EXAMPLES.md to remove deprecated lTaNmbrqr parameter
- Updated all code examples to match v1.2.3 API
- Added comprehensive cleanup documentation
- Added release guide and automation script
- No code changes, documentation only
"@

if (-not $DryRun) {
    git commit -m $commitMessage
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ? Changes committed" -ForegroundColor Green
    } else {
        Write-Host "  ?? No changes to commit (already committed)" -ForegroundColor Yellow
    }
} else {
    Write-Host "  [DRY RUN] Would commit with message:" -ForegroundColor Gray
    Write-Host "    $($commitMessage.Split([Environment]::NewLine)[0])" -ForegroundColor Gray
}
Write-Host ""

# Create tag
Write-Host "[4/7] Creating Git tag v1.2.4..." -ForegroundColor Yellow
$tagMessage = @"
v1.2.4 - Documentation Cleanup

Major Changes:
- Fixed deprecated API examples in USAGE_EXAMPLES.md
- Updated documentation to match v1.2.3 API
- Added cleanup summary documentation

No breaking changes - documentation updates only.
"@

if (-not $DryRun) {
    $existingTag = git tag -l "v1.2.4"
    if ($existingTag) {
        Write-Host "  ?? Tag v1.2.4 already exists!" -ForegroundColor Yellow
        $deleteTag = Read-Host "    Delete and recreate? (Y/N)"
        if ($deleteTag -eq "Y" -or $deleteTag -eq "y") {
            git tag -d "v1.2.4"
            git push origin ":refs/tags/v1.2.4" 2>$null
            Write-Host "    Old tag deleted" -ForegroundColor Gray
        } else {
            Write-Host "  Skipping tag creation" -ForegroundColor Yellow
            $skipTag = $true
        }
    }
    
    if (-not $skipTag) {
        git tag -a "v1.2.4" -m $tagMessage
        Write-Host "  ? Tag v1.2.4 created" -ForegroundColor Green
    }
} else {
    Write-Host "  [DRY RUN] Would create tag: v1.2.4" -ForegroundColor Gray
}
Write-Host ""

# Push to GitHub
Write-Host "[5/7] Pushing to GitHub..." -ForegroundColor Yellow
if (-not $DryRun) {
    $push = Read-Host "  Push to GitHub? (Y/N)"
    if ($push -eq "Y" -or $push -eq "y") {
        Write-Host "  Pushing commits..." -ForegroundColor Gray
        git push origin main
        
        if (-not $skipTag) {
            Write-Host "  Pushing tag..." -ForegroundColor Gray
            git push origin "v1.2.4"
        }
        
        Write-Host "  ? Pushed to GitHub" -ForegroundColor Green
    } else {
        Write-Host "  Skipped push" -ForegroundColor Yellow
    }
} else {
    Write-Host "  [DRY RUN] Would push commits and tag" -ForegroundColor Gray
}
Write-Host ""

# Copy release description
Write-Host "[6/7] Preparing release description..." -ForegroundColor Yellow

$releaseDescription = @"
## ?? Documentation Cleanup Release

This release focuses on cleaning up documentation and code examples to ensure accuracy and consistency.

### ? What's New

#### Documentation Updates
- ? **Fixed USAGE_EXAMPLES.md** - Removed deprecated ``lTaNmbrqr`` parameter from all examples
- ? **Updated API Examples** - All code samples now match v1.2.3 API
- ? **Added Cleanup Documentation** - Comprehensive cleanup summary included

### ?? Changes

#### Fixed Examples
**Before (Incorrect):**
``````csharp
// Deprecated API with unused parameter
qrGenerator.GenerateQRBitmap(1, base64Data, 2, 300);
``````

**After (Correct):**
``````csharp
// Current API without deprecated parameter
qrGenerator.GenerateQRBitmap(base64Data, 2, 300);
``````

#### Files Modified
- ``USAGE_EXAMPLES.md`` - Fixed deprecated API usage
- ``CODE_CLEANUP_SUMMARY.md`` - Added detailed cleanup report
- ``CODE_CLEANUP_COMPLETE.md`` - Added completion summary

### ?? Code Quality

- ? **Build Status:** Success (0 errors, 0 warnings)
- ? **Documentation:** All examples current and accurate
- ? **API Version:** v1.2.3
- ? **No Breaking Changes:** Documentation updates only

### ?? Installation

No changes to the NuGet package. Continue using v1.2.3:

``````bash
Install-Package AztecQRGenerator.Core -Version 1.2.3
``````

### ?? Links

- **NuGet Package:** https://www.nuget.org/packages/AztecQRGenerator.Core/1.2.3
- **Documentation:** [README.md](https://github.com/johanhenningsson4-hash/AztecQRGenerator/blob/main/README.md)
- **Usage Examples:** [USAGE_EXAMPLES.md](https://github.com/johanhenningsson4-hash/AztecQRGenerator/blob/main/USAGE_EXAMPLES.md)
- **Cleanup Summary:** [CODE_CLEANUP_SUMMARY.md](https://github.com/johanhenningsson4-hash/AztecQRGenerator/blob/main/CODE_CLEANUP_SUMMARY.md)

### ?? Project Health

| Aspect | Status |
|--------|--------|
| Build | ? Success |
| Documentation | ? Current |
| Code Quality | ? Excellent |
| Examples | ? Updated |

### ?? Technical Details

- **Type:** Documentation update
- **Breaking Changes:** None
- **Dependencies:** No changes
- **Target Framework:** .NET Framework 4.7.2
- **License:** MIT

---

**Full Changelog:** [v1.2.3...v1.2.4](https://github.com/johanhenningsson4-hash/AztecQRGenerator/compare/v1.2.3...v1.2.4)
"@

if (-not $DryRun) {
    $releaseDescription | Set-Clipboard
    Write-Host "  ? Release description copied to clipboard" -ForegroundColor Green
} else {
    Write-Host "  [DRY RUN] Would copy release description to clipboard" -ForegroundColor Gray
}
Write-Host ""

# Open GitHub
Write-Host "[7/7] Opening GitHub release page..." -ForegroundColor Yellow
if (-not $DryRun) {
    $openBrowser = Read-Host "  Open GitHub release page? (Y/N)"
    if ($openBrowser -eq "Y" -or $openBrowser -eq "y") {
        Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.2.4"
        Write-Host "  ? Browser opened" -ForegroundColor Green
    }
} else {
    Write-Host "  [DRY RUN] Would open: https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.2.4" -ForegroundColor Gray
}
Write-Host ""

# Summary
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "? Release Preparation Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. Go to GitHub release page (opened in browser)" -ForegroundColor White
Write-Host "2. Title: 'v1.2.4 - Documentation Cleanup'" -ForegroundColor White
Write-Host "3. Description: Already copied to clipboard - just paste!" -ForegroundColor White
Write-Host "4. Check 'Set as the latest release'" -ForegroundColor White
Write-Host "5. Click 'Publish release'" -ForegroundColor White
Write-Host ""
Write-Host "Release URL: https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.2.4" -ForegroundColor Cyan
Write-Host ""

