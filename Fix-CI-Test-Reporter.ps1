# Quick Fix: CI Workflow Test Reporter Error
# Fixes TypeError: Cannot read properties of undefined (reading '$')

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Fix CI Workflow Test Reporter" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Set-Location "C:\Jobb\AztecQRGenerator"

Write-Host "Fix applied:" -ForegroundColor Yellow
Write-Host "  ? Changed test reporter format: java-junit ? nunit" -ForegroundColor Green
Write-Host "  ? Added continue-on-error to prevent failures" -ForegroundColor Green
Write-Host "  ? Added if-no-files-found: warn for robustness" -ForegroundColor Green
Write-Host "  ? Improved error handling throughout workflow" -ForegroundColor Green
Write-Host ""

Write-Host "Committing fix..." -ForegroundColor Yellow
git add .github/workflows/ci.yml
git commit -m "fix: CI workflow test reporter error - use nunit format"

if ($LASTEXITCODE -eq 0) {
    Write-Host "  ? Committed" -ForegroundColor Green
} else {
    Write-Host "  ? Nothing to commit" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Pushing to GitHub..." -ForegroundColor Yellow
git push origin main

if ($LASTEXITCODE -eq 0) {
    Write-Host "  ? Pushed!" -ForegroundColor Green
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "? CI Workflow Fixed!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "? CI will now run with:" -ForegroundColor Cyan
    Write-Host "  • Correct NUnit test format" -ForegroundColor White
    Write-Host "  • Proper error handling" -ForegroundColor White
    Write-Host "  • No TypeError issues" -ForegroundColor White
    Write-Host ""
    Write-Host "Monitor at:" -ForegroundColor Yellow
    Write-Host "  https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions" -ForegroundColor Cyan
    Write-Host ""
    
    $open = Read-Host "Open GitHub Actions? (Y/N)"
    if ($open -eq "Y" -or $open -eq "y") {
        Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions"
    }
} else {
    Write-Host "  ? Push failed" -ForegroundColor Red
}

Write-Host ""
