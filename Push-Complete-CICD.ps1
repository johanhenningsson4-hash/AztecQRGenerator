# Complete CI/CD Setup - Push Everything
# Commits and pushes:
# - Updated workflows (action version fixes)
# - Updated README with CI/CD badges
# - All documentation
# Ready for v1.3.0 release!

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Push CI/CD Setup to GitHub" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Set-Location "C:\Jobb\AztecQRGenerator"

# Check what's changed
Write-Host "Checking changes..." -ForegroundColor Yellow
$status = git status --short

if ($status) {
    Write-Host ""
    Write-Host "Files to commit:" -ForegroundColor Cyan
    $status | ForEach-Object { Write-Host "  $_" -ForegroundColor Gray }
    Write-Host ""
} else {
    Write-Host "  ? No changes to commit" -ForegroundColor Green
    Write-Host ""
    Write-Host "Everything is already committed!" -ForegroundColor Yellow
    Write-Host ""
    $pushOnly = Read-Host "Push existing commits? (Y/N)"
    if ($pushOnly -ne "Y" -and $pushOnly -ne "y") {
        exit 0
    }
}

if ($status) {
    $continue = Read-Host "Commit these changes? (Y/N)"
    if ($continue -ne "Y" -and $continue -ne "y") {
        Write-Host "Aborted." -ForegroundColor Yellow
        exit 0
    }
    
    Write-Host ""
    Write-Host "Staging all files..." -ForegroundColor Yellow
    git add .
    
    Write-Host "Committing..." -ForegroundColor Yellow
    git commit -m "feat: Complete CI/CD setup with automated testing"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ? Changes committed" -ForegroundColor Green
    } else {
        Write-Host "  ?? Commit failed" -ForegroundColor Yellow
        exit 1
    }
}

Write-Host ""
Write-Host "Pushing to GitHub..." -ForegroundColor Yellow
git push origin main

if ($LASTEXITCODE -eq 0) {
    Write-Host "  ? Pushed to GitHub!" -ForegroundColor Green
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "? CI/CD Setup Pushed!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "What was pushed:" -ForegroundColor Yellow
    Write-Host "  ? Updated workflows (latest action versions)" -ForegroundColor White
    Write-Host "  ? Updated README with CI/CD badges" -ForegroundColor White
    Write-Host "  ? 58 unit tests" -ForegroundColor White
    Write-Host "  ? Complete CI/CD documentation" -ForegroundColor White
    Write-Host ""
    Write-Host "? GitHub Actions will now:" -ForegroundColor Cyan
    Write-Host "  1. Build solution (Release)" -ForegroundColor White
    Write-Host "  2. Run all 58 tests" -ForegroundColor White
    Write-Host "  3. Upload artifacts with v4" -ForegroundColor White
    Write-Host "  4. Generate test reports" -ForegroundColor White
    Write-Host ""
    Write-Host "Monitor at:" -ForegroundColor Yellow
    Write-Host "  https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Repository:" -ForegroundColor Yellow
    Write-Host "  https://github.com/johanhenningsson4-hash/AztecQRGenerator" -ForegroundColor Cyan
    Write-Host ""
    
    $openActions = Read-Host "Open GitHub Actions? (Y/N)"
    if ($openActions -eq "Y" -or $openActions -eq "y") {
        Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions"
        Write-Host "  ? Opened" -ForegroundColor Green
    }
    
    Write-Host ""
    Write-Host "?? Ready for v1.3.0 release!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "  1. Wait for CI to complete (~5 min)" -ForegroundColor White
    Write-Host "  2. Run: .\Complete-NuGet-Publish.ps1" -ForegroundColor White
    Write-Host "  3. Publish v1.3.0 to NuGet!" -ForegroundColor White
    Write-Host ""
    
} else {
    Write-Host "  ? Push failed" -ForegroundColor Red
    Write-Host ""
    Write-Host "Common issues:" -ForegroundColor Yellow
    Write-Host "  • Check Git credentials" -ForegroundColor White
    Write-Host "  • Verify network connection" -ForegroundColor White
    Write-Host "  • Ensure you have push access" -ForegroundColor White
    Write-Host ""
    Write-Host "Try again with:" -ForegroundColor Yellow
    Write-Host "  git push origin main" -ForegroundColor Cyan
    Write-Host ""
}
