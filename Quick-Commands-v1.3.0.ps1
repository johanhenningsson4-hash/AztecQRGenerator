# Quick Release Commands for v1.3.0
# Copy and paste these commands one by one

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Quick Release v1.3.0" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Copy and run these commands ONE AT A TIME:" -ForegroundColor Yellow
Write-Host ""

Write-Host "# 1. Navigate to repository" -ForegroundColor Green
Write-Host 'cd "C:\Jobb\AztecQRGenerator"' -ForegroundColor White
Write-Host ""

Write-Host "# 2. Check status" -ForegroundColor Green
Write-Host "git status" -ForegroundColor White
Write-Host ""

Write-Host "# 3. Stage all changes" -ForegroundColor Green
Write-Host "git add ." -ForegroundColor White
Write-Host ""

Write-Host "# 4. Commit" -ForegroundColor Green
Write-Host 'git commit -m "feat: Release v1.3.0 - Automated Testing & CI/CD"' -ForegroundColor White
Write-Host ""

Write-Host "# 5. Create tag" -ForegroundColor Green
Write-Host 'git tag -a v1.3.0 -m "Release v1.3.0"' -ForegroundColor White
Write-Host ""

Write-Host "# 6. Push commits" -ForegroundColor Green
Write-Host "git push origin main" -ForegroundColor White
Write-Host ""

Write-Host "# 7. Push tag" -ForegroundColor Green
Write-Host "git push origin v1.3.0" -ForegroundColor White
Write-Host ""

Write-Host "# 8. Open GitHub release page" -ForegroundColor Green
Write-Host 'Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.3.0"' -ForegroundColor White
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "After running all commands above:" -ForegroundColor Yellow
Write-Host "1. GitHub will open" -ForegroundColor White
Write-Host "2. Title: v1.3.0 - Automated Testing & CI/CD" -ForegroundColor White
Write-Host "3. Paste description from RELEASE_NOTES_v1.3.0.md" -ForegroundColor White
Write-Host "4. Click 'Publish release'" -ForegroundColor White
Write-Host "5. GitHub Actions will auto-publish to NuGet!" -ForegroundColor White
Write-Host ""

# Copy description to clipboard
Write-Host "Copying release notes to clipboard..." -ForegroundColor Yellow
if (Test-Path "RELEASE_NOTES_v1.3.0.md") {
    try {
        Get-Content "RELEASE_NOTES_v1.3.0.md" -Raw | Set-Clipboard
        Write-Host "? Release notes in clipboard - ready to paste!" -ForegroundColor Green
    } catch {
        Write-Host "?? Could not copy to clipboard" -ForegroundColor Yellow
    }
} else {
    Write-Host "?? RELEASE_NOTES_v1.3.0.md not found" -ForegroundColor Yellow
}
Write-Host ""
