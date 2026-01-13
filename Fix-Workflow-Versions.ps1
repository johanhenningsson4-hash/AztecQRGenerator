# Fix GitHub Actions Workflows - Update to Latest Versions
# Fixes deprecated actions/upload-artifact@v4 and other outdated actions

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Fix GitHub Actions Workflows" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Set-Location "C:\Jobb\AztecQRGenerator"

Write-Host "Updates applied:" -ForegroundColor Yellow
Write-Host "  ? actions/upload-artifact: v3 ? v4" -ForegroundColor Green
Write-Host "  ? microsoft/setup-msbuild: v1.3 ? v2" -ForegroundColor Green
Write-Host "  ? NuGet/setup-nuget: v1 ? v2" -ForegroundColor Green
Write-Host "  ? github/codeql-action/upload-sarif: v2 ? v3" -ForegroundColor Green
Write-Host ""

Write-Host "Files updated:" -ForegroundColor Yellow
Write-Host "  • .github/workflows/ci.yml" -ForegroundColor Gray
Write-Host "  • .github/workflows/nuget-publish.yml" -ForegroundColor Gray
Write-Host "  • .github/workflows/code-quality.yml" -ForegroundColor Gray
Write-Host ""

# Check Git status
Write-Host "Checking Git status..." -ForegroundColor Yellow
$status = git status --short

if ($status) {
    Write-Host ""
    Write-Host "Changed files:" -ForegroundColor Cyan
    $status | ForEach-Object { Write-Host "  $_" -ForegroundColor Gray }
} else {
    Write-Host "  ?? No changes detected (workflows may already be updated)" -ForegroundColor Yellow
}
Write-Host ""

$continue = Read-Host "Commit and push workflow fixes? (Y/N)"

if ($continue -eq "Y" -or $continue -eq "y") {
    Write-Host ""
    Write-Host "Committing changes..." -ForegroundColor Yellow
    
    # Stage workflow files
    git add .github/workflows/
    
    # Commit with properly escaped message
    $commitMsg = "fix: Update GitHub Actions workflows to latest versions"
    git commit -m $commitMsg
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ? Changes committed" -ForegroundColor Green
    } else {
        Write-Host "  ?? Nothing to commit or commit failed" -ForegroundColor Yellow
        exit 0
    }
    
    Write-Host ""
    Write-Host "Pushing to GitHub..." -ForegroundColor Yellow
    
    git push origin main
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ? Successfully pushed to GitHub" -ForegroundColor Green
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host "? Workflows Updated!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "? All workflows now use latest action versions" -ForegroundColor Green
        Write-Host ""
        Write-Host "? CI workflow will trigger automatically" -ForegroundColor Cyan
        Write-Host "  • Build solution" -ForegroundColor White
        Write-Host "  • Run 58 tests" -ForegroundColor White
        Write-Host "  • Upload artifacts (using v4)" -ForegroundColor White
        Write-Host ""
        Write-Host "Monitor at:" -ForegroundColor Yellow
        Write-Host "  https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions" -ForegroundColor Cyan
        Write-Host ""
        
        $openActions = Read-Host "Open GitHub Actions? (Y/N)"
        if ($openActions -eq "Y" -or $openActions -eq "y") {
            Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions"
        }
        
    } else {
        Write-Host "  ? Push failed" -ForegroundColor Red
        Write-Host ""
        Write-Host "You can push manually later with:" -ForegroundColor Yellow
        Write-Host "  git push origin main" -ForegroundColor Cyan
    }
    
} else {
    Write-Host ""
    Write-Host "Aborted. No changes committed." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "?? Done!" -ForegroundColor Green
Write-Host ""
