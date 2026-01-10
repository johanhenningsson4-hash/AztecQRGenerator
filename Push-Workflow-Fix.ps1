# Simple Workflow Fix - Just commit and push
# No complex commit messages, just get it done

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Push Workflow Fixes to GitHub" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Set-Location "C:\Jobb\AztecQRGenerator"

# Stage all changes
Write-Host "Staging changes..." -ForegroundColor Yellow
git add .

# Simple commit
Write-Host "Committing..." -ForegroundColor Yellow
git commit -m "fix: Update workflows to latest action versions"

if ($LASTEXITCODE -eq 0) {
    Write-Host "  ? Committed" -ForegroundColor Green
} else {
    Write-Host "  ?? Nothing to commit" -ForegroundColor Yellow
    $status = git status --short
    if (-not $status) {
        Write-Host ""
        Write-Host "All changes already committed!" -ForegroundColor Green
        Write-Host ""
    }
}

# Push
Write-Host ""
Write-Host "Pushing to GitHub..." -ForegroundColor Yellow
git push origin main

if ($LASTEXITCODE -eq 0) {
    Write-Host "  ? Pushed successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "? Done!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "View actions at:" -ForegroundColor Yellow
    Write-Host "https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions" -ForegroundColor Cyan
    Write-Host ""
    
    $open = Read-Host "Open GitHub Actions? (Y/N)"
    if ($open -eq "Y" -or $open -eq "y") {
        Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions"
    }
} else {
    Write-Host "  ? Push failed" -ForegroundColor Red
    Write-Host ""
    Write-Host "Check your credentials and try again." -ForegroundColor Yellow
}

Write-Host ""
