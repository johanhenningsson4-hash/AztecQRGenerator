# Update Version to 1.3.0 and Push to Git
# Simple, safe, step-by-step

$ErrorActionPreference = "Stop"
$oldVersion = "1.2.3"
$newVersion = "1.3.0"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Update Version to $newVersion" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Set-Location "C:\Jobb\AztecQRGenerator"

# Step 1: Update version numbers
Write-Host "[1/5] Updating version numbers from $oldVersion to $newVersion..." -ForegroundColor Yellow

$filesToUpdate = @{
    "AztecQRGenerator.Core\Properties\AssemblyInfo.cs" = @(
        @{ Pattern = '\[assembly: AssemblyVersion\("1\.2\.3\.0"\)\]'; Replace = '[assembly: AssemblyVersion("1.3.0.0")]' }
        @{ Pattern = '\[assembly: AssemblyFileVersion\("1\.2\.3\.0"\)\]'; Replace = '[assembly: AssemblyFileVersion("1.3.0.0")]' }
    )
    "Properties\AssemblyInfo.cs" = @(
        @{ Pattern = '\[assembly: AssemblyVersion\("1\.2\.3\.0"\)\]'; Replace = '[assembly: AssemblyVersion("1.3.0.0")]' }
        @{ Pattern = '\[assembly: AssemblyFileVersion\("1\.2\.3\.0"\)\]'; Replace = '[assembly: AssemblyFileVersion("1.3.0.0")]' }
    )
    "AztecQRGenerator.Tests\Properties\AssemblyInfo.cs" = @(
        @{ Pattern = '\[assembly: AssemblyVersion\("1\.2\.3\.0"\)\]'; Replace = '[assembly: AssemblyVersion("1.3.0.0")]' }
        @{ Pattern = '\[assembly: AssemblyFileVersion\("1\.2\.3\.0"\)\]'; Replace = '[assembly: AssemblyFileVersion("1.3.0.0")]' }
    )
    "AztecQRGenerator.Core\AztecQRGenerator.Core.csproj" = @(
        @{ Pattern = '<Version>1\.2\.3</Version>'; Replace = '<Version>1.3.0</Version>' }
    )
    "AztecQRGenerator.Core\AztecQRGenerator.Core.nuspec" = @(
        @{ Pattern = '<version>1\.2\.3</version>'; Replace = '<version>1.3.0</version>' }
    )
}

$updatedFiles = @()

foreach ($file in $filesToUpdate.Keys) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw
        $modified = $false
        
        foreach ($replacement in $filesToUpdate[$file]) {
            if ($content -match $replacement.Pattern) {
                $content = $content -replace $replacement.Pattern, $replacement.Replace
                $modified = $true
            }
        }
        
        if ($modified) {
            Set-Content $file $content -NoNewline
            $updatedFiles += $file
            Write-Host "  ? Updated: $file" -ForegroundColor Green
        } else {
            Write-Host "  ?? No changes needed: $file" -ForegroundColor Gray
        }
    } else {
        Write-Host "  ?? File not found: $file" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "Updated $($updatedFiles.Count) files" -ForegroundColor Cyan
Write-Host ""

# Step 2: Show changes
Write-Host "[2/5] Reviewing changes..." -ForegroundColor Yellow
git diff --stat
Write-Host ""

$continue = Read-Host "Continue with commit? (Y/N)"
if ($continue -ne "Y" -and $continue -ne "y") {
    Write-Host "Aborted. Changes remain uncommitted." -ForegroundColor Yellow
    exit 0
}
Write-Host ""

# Step 3: Stage and commit
Write-Host "[3/5] Committing changes..." -ForegroundColor Yellow

git add .

$commitMessage = @"
feat: Release v$newVersion - Automated Testing & CI/CD

Major Updates:
- Added 58 comprehensive unit tests (NUnit 3.14)
- Implemented GitHub Actions CI/CD pipeline
- Automated build, test, and NuGet publishing
- Added code quality and security scanning
- Enhanced documentation for testing
- Fixed deprecated examples in documentation

Test Coverage:
- QRGenerator: 25 tests
- AztecGenerator: 25 tests
- Logger: 8 tests

Breaking Changes: None
"@

git commit -m $commitMessage

if ($LASTEXITCODE -eq 0) {
    Write-Host "  ? Changes committed" -ForegroundColor Green
} else {
    Write-Host "  ?? Nothing to commit or commit failed" -ForegroundColor Yellow
}
Write-Host ""

# Step 4: Create tag
Write-Host "[4/5] Creating Git tag v$newVersion..." -ForegroundColor Yellow

# Check if tag exists locally
$localTag = git tag -l "v$newVersion"
if ($localTag) {
    Write-Host "  ?? Tag v$newVersion already exists locally" -ForegroundColor Yellow
    $deleteLocal = Read-Host "    Delete local tag? (Y/N)"
    if ($deleteLocal -eq "Y" -or $deleteLocal -eq "y") {
        git tag -d "v$newVersion"
        Write-Host "    Local tag deleted" -ForegroundColor Gray
    }
}

# Create tag
$tagMessage = "Release v$newVersion - Automated Testing & CI/CD"
git tag -a "v$newVersion" -m $tagMessage

if ($LASTEXITCODE -eq 0) {
    Write-Host "  ? Tag v$newVersion created" -ForegroundColor Green
} else {
    Write-Host "  ? Tag creation failed" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Step 5: Push to GitHub
Write-Host "[5/5] Pushing to GitHub..." -ForegroundColor Yellow
Write-Host ""

Write-Host "This will push:" -ForegroundColor Cyan
Write-Host "  • Commits to branch: main" -ForegroundColor White
Write-Host "  • Tag: v$newVersion" -ForegroundColor White
Write-Host ""

$push = Read-Host "Push to GitHub now? (Y/N)"

if ($push -eq "Y" -or $push -eq "y") {
    Write-Host ""
    Write-Host "Pushing commits..." -ForegroundColor Gray
    git push origin main
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ? Commits pushed" -ForegroundColor Green
    } else {
        Write-Host "  ? Push failed" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "Pushing tag..." -ForegroundColor Gray
    
    # Check if tag exists remotely
    $remoteTag = git ls-remote --tags origin "refs/tags/v$newVersion"
    if ($remoteTag) {
        Write-Host "  ?? Tag v$newVersion already exists on remote" -ForegroundColor Yellow
        $forceTag = Read-Host "    Force push tag? (Y/N)"
        if ($forceTag -eq "Y" -or $forceTag -eq "y") {
            git push origin "v$newVersion" --force
        } else {
            Write-Host "  Skipped tag push" -ForegroundColor Yellow
        }
    } else {
        git push origin "v$newVersion"
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  ? Tag pushed" -ForegroundColor Green
        } else {
            Write-Host "  ? Tag push failed" -ForegroundColor Red
        }
    }
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "? Version Updated and Pushed!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "? Version $newVersion is now on GitHub!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next Steps:" -ForegroundColor Yellow
    Write-Host "1. Create GitHub Release:" -ForegroundColor White
    Write-Host "   https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v$newVersion" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "2. Title: v$newVersion - Automated Testing & CI/CD" -ForegroundColor White
    Write-Host ""
    Write-Host "3. Description: Copy from RELEASE_NOTES_v$newVersion.md" -ForegroundColor White
    Write-Host ""
    Write-Host "4. Publish ? GitHub Actions will auto-publish to NuGet!" -ForegroundColor White
    Write-Host ""
    
    # Copy release notes
    if (Test-Path "RELEASE_NOTES_v$newVersion.md") {
        try {
            Get-Content "RELEASE_NOTES_v$newVersion.md" -Raw | Set-Clipboard
            Write-Host "? Release notes copied to clipboard!" -ForegroundColor Green
        } catch {
            Write-Host "?? Release notes at: RELEASE_NOTES_v$newVersion.md" -ForegroundColor Gray
        }
    }
    
    Write-Host ""
    $openRelease = Read-Host "Open GitHub release page? (Y/N)"
    if ($openRelease -eq "Y" -or $openRelease -eq "y") {
        Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v$newVersion"
    }
    
} else {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "? Version Updated (Not Pushed)" -ForegroundColor Yellow
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Changes are committed and tagged locally." -ForegroundColor White
    Write-Host "To push later, run:" -ForegroundColor Yellow
    Write-Host "  git push origin main" -ForegroundColor Cyan
    Write-Host "  git push origin v$newVersion" -ForegroundColor Cyan
    Write-Host ""
}

Write-Host ""
Write-Host "?? Done!" -ForegroundColor Green
Write-Host ""
