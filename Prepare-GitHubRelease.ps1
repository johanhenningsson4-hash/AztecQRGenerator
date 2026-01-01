# Prepare GitHub Release v1.3.0
# This script commits changes, creates tag, and provides GitHub release instructions

param(
    [string]$Version = "1.3.0",
    [switch]$DryRun = $false
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "GitHub Release Preparation v$Version" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$repoPath = "C:\Jobb\AztecQRGenerator"
Set-Location $repoPath

# Check if we're on main branch
$currentBranch = git branch --show-current
Write-Host "Current branch: $currentBranch" -ForegroundColor Yellow

if ($currentBranch -ne "main") {
    Write-Host "? Warning: You're not on main branch!" -ForegroundColor Yellow
    $continue = Read-Host "Continue anyway? (Y/N)"
    if ($continue -ne "Y" -and $continue -ne "y") {
        Write-Host "Aborted." -ForegroundColor Red
        exit
    }
}

Write-Host ""

# Check git status
Write-Host "[1/6] Checking Git status..." -ForegroundColor Yellow
$status = git status --porcelain
if ($status) {
    Write-Host "  Uncommitted changes found:" -ForegroundColor Gray
    $status | ForEach-Object { Write-Host "    $_" -ForegroundColor Gray }
} else {
    Write-Host "  ? Working directory clean" -ForegroundColor Green
}

Write-Host ""

# Stage all changes
Write-Host "[2/6] Staging changes..." -ForegroundColor Yellow
if (-not $DryRun) {
    git add .
    Write-Host "  ? Changes staged" -ForegroundColor Green
} else {
    Write-Host "  [DRY RUN] Would stage all changes" -ForegroundColor Gray
}

Write-Host ""

# Commit changes
Write-Host "[3/6] Committing changes..." -ForegroundColor Yellow
$commitMessage = "Release v$Version - Permission fixes and enhanced CLI mode"

if (-not $DryRun) {
    git commit -m $commitMessage
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ? Changes committed" -ForegroundColor Green
    } else {
        Write-Host "  ? No changes to commit (or commit failed)" -ForegroundColor Yellow
    }
} else {
    Write-Host "  [DRY RUN] Would commit with message:" -ForegroundColor Gray
    Write-Host "    $commitMessage" -ForegroundColor Gray
}

Write-Host ""

# Create tag
Write-Host "[4/6] Creating Git tag..." -ForegroundColor Yellow
$tagMessage = @"
Release v$Version

Major Changes:
- Permission-safe file locations (no admin required)
- Logger uses AppData with fallback hierarchy  
- Smart file saving with automatic fallback
- Enhanced CLI mode honors output file parameter
- Better error messages and logging

See RELEASE_NOTES_v$Version.md for full details.
"@

if (-not $DryRun) {
    # Check if tag already exists
    $existingTag = git tag -l "v$Version"
    if ($existingTag) {
        Write-Host "  ? Tag v$Version already exists!" -ForegroundColor Yellow
        $deleteTag = Read-Host "    Delete and recreate? (Y/N)"
        if ($deleteTag -eq "Y" -or $deleteTag -eq "y") {
            git tag -d "v$Version"
            git push origin ":refs/tags/v$Version" 2>$null
            Write-Host "    Old tag deleted" -ForegroundColor Gray
        } else {
            Write-Host "  Skipping tag creation" -ForegroundColor Yellow
            $skipTag = $true
        }
    }
    
    if (-not $skipTag) {
        git tag -a "v$Version" -m $tagMessage
        Write-Host "  ? Tag v$Version created" -ForegroundColor Green
    }
} else {
    Write-Host "  [DRY RUN] Would create tag: v$Version" -ForegroundColor Gray
    Write-Host "  Message:" -ForegroundColor Gray
    $tagMessage -split "`n" | ForEach-Object { Write-Host "    $_" -ForegroundColor Gray }
}

Write-Host ""

# Push to GitHub
Write-Host "[5/6] Pushing to GitHub..." -ForegroundColor Yellow
if (-not $DryRun) {
    $push = Read-Host "  Push to GitHub? (Y/N)"
    if ($push -eq "Y" -or $push -eq "y") {
        Write-Host "  Pushing commits..." -ForegroundColor Gray
        git push origin $currentBranch
        
        Write-Host "  Pushing tag..." -ForegroundColor Gray
        git push origin "v$Version"
        
        Write-Host "  ? Pushed to GitHub" -ForegroundColor Green
    } else {
        Write-Host "  Skipped push (you can push manually later)" -ForegroundColor Yellow
    }
} else {
    Write-Host "  [DRY RUN] Would push commits and tag to origin/$currentBranch" -ForegroundColor Gray
}

Write-Host ""

# GitHub Release Instructions
Write-Host "[6/6] GitHub Release Instructions" -ForegroundColor Yellow
Write-Host ""

$releaseUrl = "https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v$Version"
$zipPath = "C:\Jobb\AztecQRGenerator\Releases\AztecQRGenerator_v$Version.zip"

Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "Create GitHub Release" -ForegroundColor Cyan
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Go to GitHub Releases:" -ForegroundColor White
Write-Host "   $releaseUrl" -ForegroundColor Cyan
Write-Host ""
Write-Host "2. Fill in the form:" -ForegroundColor White
Write-Host "   - Tag: v$Version (should be auto-selected)" -ForegroundColor Gray
Write-Host "   - Title: AztecQRGenerator v$Version - Permission Fixes" -ForegroundColor Gray
Write-Host ""
Write-Host "3. Release Description (copy below):" -ForegroundColor White
Write-Host "????????????????????????????????????????" -ForegroundColor DarkGray

$releaseDescription = @"
## ?? What's New in v$Version

### ?? No Administrator Privileges Required!
The biggest improvement - AztecQRGenerator now runs with standard user permissions. No more "Access Denied" errors!

### ?? Smart File Locations
- **Logs**: ``%LocalAppData%\AztecQRGenerator\Logs\``
- **Output**: ``%UserProfile%\Documents\AztecQRGenerator\Output\``

### ? Key Features
- ? No admin rights needed
- ? Safe default locations following Windows best practices
- ? Automatic fallback if permission denied
- ? Enhanced CLI mode honors output file parameter
- ? Better error messages

## ?? Installation

1. Download ``AztecQRGenerator_v$Version.zip`` below
2. Extract to any folder (no installation required)
3. Run ``AztecQRGenerator.exe``
4. No administrator privileges needed!

## ?? Quick Start

### GUI Mode
``````
AztecQRGenerator.exe
``````

### CLI Mode
``````bash
# Generate QR code to specific location
AztecQRGenerator.exe QR "SGVsbG8gV29ybGQh" output.png 300 2

# Generate Aztec code
AztecQRGenerator.exe AZTEC "SGVsbG8gV29ybGQh" output.png 300 2
``````

## ?? Documentation

- [Release Notes](https://github.com/johanhenningsson4-hash/AztecQRGenerator/blob/main/RELEASE_NOTES_v$Version.md)
- [Changelog](https://github.com/johanhenningsson4-hash/AztecQRGenerator/blob/main/CHANGELOG.md)
- [Permission Fix Summary](https://github.com/johanhenningsson4-hash/AztecQRGenerator/blob/main/PERMISSION_FIX_SUMMARY.md)

## ?? Bug Fixes

- Fixed application requiring administrator privileges
- Fixed "Access Denied" errors when creating log files
- Fixed file save failures in protected directories
- Fixed unclear error messages about file locations

## ? Full Changelog

See [CHANGELOG.md](https://github.com/johanhenningsson4-hash/AztecQRGenerator/blob/main/CHANGELOG.md) for complete version history.

## ?? Checksums

See attached ``AztecQRGenerator_v$Version.zip.checksums.txt``

---

**No more permission headaches! ??**
"@

Write-Host $releaseDescription -ForegroundColor Gray
Write-Host "????????????????????????????????????????" -ForegroundColor DarkGray
Write-Host ""
Write-Host "4. Attach files:" -ForegroundColor White
Write-Host "   - $zipPath" -ForegroundColor Gray
if (Test-Path "$zipPath.checksums.txt") {
    Write-Host "   - $zipPath.checksums.txt" -ForegroundColor Gray
}
Write-Host ""
Write-Host "5. Check 'Set as the latest release'" -ForegroundColor White
Write-Host "6. Click 'Publish release'" -ForegroundColor White
Write-Host ""

# Copy description to clipboard
if (-not $DryRun) {
    $copyToClipboard = Read-Host "Copy release description to clipboard? (Y/N)"
    if ($copyToClipboard -eq "Y" -or $copyToClipboard -eq "y") {
        $releaseDescription | Set-Clipboard
        Write-Host "  ? Description copied to clipboard!" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan

# Open browser
if (-not $DryRun) {
    $openBrowser = Read-Host "Open GitHub release page in browser? (Y/N)"
    if ($openBrowser -eq "Y" -or $openBrowser -eq "y") {
        Start-Process $releaseUrl
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Preparation Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Summary:" -ForegroundColor Yellow
Write-Host "  ? Version:  $Version" -ForegroundColor White
Write-Host "  ? Branch:   $currentBranch" -ForegroundColor White
Write-Host "  ? Tag:      v$Version" -ForegroundColor White
Write-Host "  ? Package:  $zipPath" -ForegroundColor White
Write-Host ""
Write-Host "Next: Create the GitHub release using the instructions above!" -ForegroundColor Yellow
Write-Host ""
