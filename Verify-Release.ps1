# Verify Release v1.3.0 Ready

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Release v1.3.0 Status Check" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$allGood = $true

# Check 1: Git tag
Write-Host "[1/5] Checking Git tag..." -ForegroundColor Yellow
$tag = git tag -l "v1.3.0"
if ($tag) {
    Write-Host "  ? Tag v1.3.0 exists" -ForegroundColor Green
} else {
    Write-Host "  ? Tag v1.3.0 not found" -ForegroundColor Red
    $allGood = $false
}

# Check 2: Tag pushed
Write-Host "[2/5] Checking if tag is on GitHub..." -ForegroundColor Yellow
$remote = git ls-remote --tags origin "v1.3.0"
if ($remote) {
    Write-Host "  ? Tag pushed to GitHub" -ForegroundColor Green
} else {
    Write-Host "  ? Tag not on GitHub" -ForegroundColor Red
    $allGood = $false
}

# Check 3: Release package
Write-Host "[3/5] Checking release package..." -ForegroundColor Yellow
$zipPath = "C:\Jobb\AztecQRGenerator\Releases\AztecQRGenerator_v1.3.0.zip"
if (Test-Path $zipPath) {
    $zipSize = (Get-Item $zipPath).Length
    Write-Host "  ? Package exists ($([math]::Round($zipSize/1KB, 2)) KB)" -ForegroundColor Green
} else {
    Write-Host "  ? Package not found" -ForegroundColor Red
    $allGood = $false
}

# Check 4: Checksums
Write-Host "[4/5] Checking checksums file..." -ForegroundColor Yellow
$checksumPath = "$zipPath.checksums.txt"
if (Test-Path $checksumPath) {
    Write-Host "  ? Checksums file exists" -ForegroundColor Green
} else {
    Write-Host "  ? Checksums file not found" -ForegroundColor Red
    $allGood = $false
}

# Check 5: Release description
Write-Host "[5/5] Checking release description..." -ForegroundColor Yellow
$descPath = "C:\Jobb\AztecQRGenerator\GITHUB_RELEASE_DESCRIPTION.txt"
if (Test-Path $descPath) {
    Write-Host "  ? Release description ready" -ForegroundColor Green
} else {
    Write-Host "  ? Description file not found" -ForegroundColor Red
    $allGood = $false
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan

if ($allGood) {
    Write-Host "? EVERYTHING READY!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Next step: Create GitHub Release" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "1. Go to: https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.3.0" -ForegroundColor White
    Write-Host "2. Title: AztecQRGenerator v1.3.0 - Permission Fixes" -ForegroundColor White
    Write-Host "3. Description: Press Ctrl+V (already in clipboard)" -ForegroundColor White
    Write-Host "4. Upload files from: Releases\" -ForegroundColor White
    Write-Host "5. Click 'Publish release'" -ForegroundColor White
    Write-Host ""
    
    $open = Read-Host "Open GitHub and copy description to clipboard? (Y/N)"
    if ($open -eq "Y" -or $open -eq "y") {
        Get-Content $descPath | Set-Clipboard
        Write-Host "? Description copied to clipboard!" -ForegroundColor Green
        Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.3.0"
        explorer "C:\Jobb\AztecQRGenerator\Releases"
    }
} else {
    Write-Host "? ISSUES FOUND" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Please fix the issues above before releasing." -ForegroundColor Yellow
}

Write-Host ""
