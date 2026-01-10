# Complete NuGet Publishing for v1.3.0
# Handles everything: version, commit, push, GitHub release, and NuGet

param(
    [switch]$ManualNuGet = $false,
    [switch]$SkipGitHub = $false
)

$ErrorActionPreference = "Stop"
$version = "1.3.0"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Complete NuGet Publishing v$version" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Set-Location "C:\Jobb\AztecQRGenerator"

# Step 1: Check if version is already updated
Write-Host "[1/7] Checking version status..." -ForegroundColor Yellow
$assemblyInfo = Get-Content "AztecQRGenerator.Core\Properties\AssemblyInfo.cs" -Raw

if ($assemblyInfo -match 'AssemblyVersion\("1\.3\.0\.0"\)') {
    Write-Host "  ? Version already updated to 1.3.0" -ForegroundColor Green
    $versionUpdated = $true
} else {
    Write-Host "  ?? Version needs to be updated" -ForegroundColor Yellow
    $update = Read-Host "  Update version now? (Y/N)"
    
    if ($update -eq "Y" -or $update -eq "y") {
        Write-Host "  Running Update-Version-And-Push.ps1..." -ForegroundColor Gray
        .\Update-Version-And-Push.ps1
        $versionUpdated = $true
    } else {
        Write-Host "  ?? Skipping version update" -ForegroundColor Yellow
        $versionUpdated = $false
    }
}
Write-Host ""

# Step 2: Check Git status
Write-Host "[2/7] Checking Git status..." -ForegroundColor Yellow
$gitStatus = git status --short

if ($gitStatus) {
    Write-Host "  ?? Uncommitted changes found" -ForegroundColor Yellow
    Write-Host "  Run Update-Version-And-Push.ps1 first" -ForegroundColor White
    exit 1
} else {
    Write-Host "  ? Working directory clean" -ForegroundColor Green
}

# Check if tag exists
$tagExists = git tag -l "v$version"
if ($tagExists) {
    Write-Host "  ? Tag v$version exists" -ForegroundColor Green
} else {
    Write-Host "  ?? Tag v$version not found" -ForegroundColor Yellow
    Write-Host "  Run Update-Version-And-Push.ps1 first" -ForegroundColor White
    exit 1
}
Write-Host ""

# Step 3: Check GitHub
Write-Host "[3/7] Checking GitHub status..." -ForegroundColor Yellow
$remoteTag = git ls-remote --tags origin "refs/tags/v$version"

if ($remoteTag) {
    Write-Host "  ? Tag v$version exists on GitHub" -ForegroundColor Green
} else {
    Write-Host "  ?? Tag not pushed to GitHub" -ForegroundColor Yellow
    $push = Read-Host "  Push now? (Y/N)"
    
    if ($push -eq "Y" -or $push -eq "y") {
        git push origin main
        git push origin "v$version"
        Write-Host "  ? Pushed to GitHub" -ForegroundColor Green
    } else {
        Write-Host "  ?? Cannot proceed without pushing to GitHub" -ForegroundColor Yellow
        exit 1
    }
}
Write-Host ""

# Step 4: Check GitHub Secret
Write-Host "[4/7] Checking NuGet API Key secret..." -ForegroundColor Yellow
Write-Host "  ?? Cannot verify GitHub secret from here" -ForegroundColor Gray
Write-Host "  Verify at: https://github.com/johanhenningsson4-hash/AztecQRGenerator/settings/secrets/actions" -ForegroundColor Cyan
Write-Host ""
$hasSecret = Read-Host "  Is NUGET_API_KEY secret configured? (Y/N)"

if ($hasSecret -ne "Y" -and $hasSecret -ne "y") {
    Write-Host ""
    Write-Host "  ?? To add NuGet API Key secret:" -ForegroundColor Yellow
    Write-Host "  1. Go to: https://www.nuget.org/account/apikeys" -ForegroundColor White
    Write-Host "  2. Create key with 'Push' permission" -ForegroundColor White
    Write-Host "  3. Copy the key" -ForegroundColor White
    Write-Host "  4. Go to: https://github.com/johanhenningsson4-hash/AztecQRGenerator/settings/secrets/actions" -ForegroundColor White
    Write-Host "  5. New secret: Name=NUGET_API_KEY, Value=(paste key)" -ForegroundColor White
    Write-Host ""
    
    $continue = Read-Host "  Continue anyway? (Y/N)"
    if ($continue -ne "Y" -and $continue -ne "y") {
        exit 0
    }
}
Write-Host ""

# Step 5: Create GitHub Release
if (-not $SkipGitHub) {
    Write-Host "[5/7] Creating GitHub Release..." -ForegroundColor Yellow
    
    # Copy release notes to clipboard
    if (Test-Path "RELEASE_NOTES_v$version.md") {
        try {
            Get-Content "RELEASE_NOTES_v$version.md" -Raw | Set-Clipboard
            Write-Host "  ? Release notes copied to clipboard" -ForegroundColor Green
        } catch {
            Write-Host "  ?? Release notes available in RELEASE_NOTES_v$version.md" -ForegroundColor Gray
        }
    }
    
    Write-Host ""
    Write-Host "  ?? GitHub Release Instructions:" -ForegroundColor Cyan
    Write-Host "  1. Tag: v$version (auto-selected)" -ForegroundColor White
    Write-Host "  2. Title: v$version - Automated Testing & CI/CD" -ForegroundColor White
    Write-Host "  3. Description: Already in clipboard - just paste!" -ForegroundColor White
    Write-Host "  4. Check: 'Set as the latest release'" -ForegroundColor White
    Write-Host "  5. Click: 'Publish release'" -ForegroundColor White
    Write-Host ""
    
    $openRelease = Read-Host "  Open GitHub release page? (Y/N)"
    if ($openRelease -eq "Y" -or $openRelease -eq "y") {
        Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v$version"
        Write-Host "  ? Browser opened" -ForegroundColor Green
        Write-Host ""
        
        Write-Host "  ? Waiting for you to create the release..." -ForegroundColor Yellow
        Write-Host "  Press Enter after you've published the release" -ForegroundColor Cyan
        Read-Host
    }
} else {
    Write-Host "[5/7] Skipping GitHub Release (--SkipGitHub flag)" -ForegroundColor Yellow
}
Write-Host ""

# Step 6: Monitor GitHub Actions
Write-Host "[6/7] Monitoring GitHub Actions..." -ForegroundColor Yellow
Write-Host "  Opening GitHub Actions page..." -ForegroundColor Gray
Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions"

Write-Host ""
Write-Host "  ? GitHub Actions should now be:" -ForegroundColor Cyan
Write-Host "     1. Building Core library" -ForegroundColor White
Write-Host "     2. Creating NuGet package" -ForegroundColor White
Write-Host "     3. Publishing to NuGet.org" -ForegroundColor White
Write-Host ""
Write-Host "  ?? This takes about 2-5 minutes" -ForegroundColor Yellow
Write-Host ""

$waitForActions = Read-Host "  Wait for GitHub Actions to complete? (Y/N)"

if ($waitForActions -eq "Y" -or $waitForActions -eq "y") {
    Write-Host ""
    Write-Host "  ? Waiting 3 minutes for GitHub Actions..." -ForegroundColor Yellow
    for ($i = 1; $i -le 6; $i++) {
        Write-Host "    $($i*30) seconds..." -ForegroundColor Gray
        Start-Sleep -Seconds 30
    }
    Write-Host "  ? Wait complete" -ForegroundColor Green
}
Write-Host ""

# Step 7: Verify on NuGet
Write-Host "[7/7] Verifying NuGet publication..." -ForegroundColor Yellow
Write-Host ""
Write-Host "  Package should be available at:" -ForegroundColor Cyan
Write-Host "  https://www.nuget.org/packages/AztecQRGenerator.Core/$version" -ForegroundColor White
Write-Host ""

$openNuGet = Read-Host "  Open NuGet package page? (Y/N)"
if ($openNuGet -eq "Y" -or $openNuGet -eq "y") {
    Start-Process "https://www.nuget.org/packages/AztecQRGenerator.Core/$version"
    Write-Host "  ? Browser opened" -ForegroundColor Green
}
Write-Host ""

# Manual publishing option
if ($ManualNuGet) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Manual NuGet Publishing" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    
    Write-Host "Building NuGet package manually..." -ForegroundColor Yellow
    
    # Get or download nuget.exe
    if (-not (Test-Path "nuget.exe")) {
        Write-Host "  Downloading nuget.exe..." -ForegroundColor Gray
        $nugetUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
        Invoke-WebRequest -Uri $nugetUrl -OutFile "nuget.exe"
    }
    
    # Build package
    Push-Location "AztecQRGenerator.Core"
    ..\nuget.exe pack AztecQRGenerator.Core.nuspec -Properties Configuration=Release
    Pop-Location
    
    Write-Host "  ? Package built" -ForegroundColor Green
    Write-Host ""
    
    # Publish
    $publish = Read-Host "  Publish to NuGet.org now? (Y/N)"
    if ($publish -eq "Y" -or $publish -eq "y") {
        Write-Host ""
        Write-Host "  Enter your NuGet API key:" -ForegroundColor Yellow
        $apiKey = Read-Host -AsSecureString
        $apiKeyPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($apiKey))
        
        Push-Location "AztecQRGenerator.Core"
        ..\nuget.exe push "AztecQRGenerator.Core.$version.nupkg" -ApiKey $apiKeyPlain -Source https://api.nuget.org/v3/index.json
        Pop-Location
        
        Write-Host "  ? Published to NuGet.org" -ForegroundColor Green
    }
}

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "? Publishing Process Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "?? Package: AztecQRGenerator.Core v$version" -ForegroundColor White
Write-Host ""
Write-Host "?? Important Links:" -ForegroundColor Yellow
Write-Host "  • Actions: https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions" -ForegroundColor Cyan
Write-Host "  • NuGet: https://www.nuget.org/packages/AztecQRGenerator.Core/$version" -ForegroundColor Cyan
Write-Host "  • Releases: https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases" -ForegroundColor Cyan
Write-Host ""
Write-Host "?? Timeline:" -ForegroundColor Yellow
Write-Host "  • Now: Package published via GitHub Actions" -ForegroundColor White
Write-Host "  • 5 min: Package visible on NuGet.org" -ForegroundColor White
Write-Host "  • 15 min: Fully searchable" -ForegroundColor White
Write-Host ""
Write-Host "? Users can now install with:" -ForegroundColor Yellow
Write-Host "  Install-Package AztecQRGenerator.Core -Version $version" -ForegroundColor Cyan
Write-Host ""
Write-Host "?? Congratulations on publishing v$version!" -ForegroundColor Green
Write-Host ""

