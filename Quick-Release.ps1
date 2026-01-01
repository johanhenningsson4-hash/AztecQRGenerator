# Quick Release v1.3.0
# Simple script to create tag and push

$ErrorActionPreference = "Stop"

Write-Host "Creating and pushing release v1.3.0..." -ForegroundColor Cyan
Write-Host ""

try {
    # Navigate to repo
    Set-Location "C:\Jobb\AztecQRGenerator"
    
    # Check if tag exists
    $existingTag = git tag -l "v1.3.0"
    if ($existingTag) {
        Write-Host "Tag v1.3.0 already exists. Delete it? (Y/N)" -ForegroundColor Yellow
        $response = Read-Host
        if ($response -eq "Y") {
            git tag -d v1.3.0
            git push origin :refs/tags/v1.3.0 2>$null
            Write-Host "Old tag deleted" -ForegroundColor Gray
        } else {
            Write-Host "Keeping existing tag" -ForegroundColor Yellow
            exit
        }
    }
    
    # Create tag
    Write-Host "Creating tag v1.3.0..." -ForegroundColor Yellow
    git tag -a v1.3.0 -m "Release v1.3.0"
    Write-Host "? Tag created" -ForegroundColor Green
    
    # Push tag
    Write-Host "Pushing tag to GitHub..." -ForegroundColor Yellow
    git push origin v1.3.0
    Write-Host "? Tag pushed" -ForegroundColor Green
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "SUCCESS!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "1. Go to: https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.3.0" -ForegroundColor White
    Write-Host "2. Title: AztecQRGenerator v1.3.0 - Permission Fixes" -ForegroundColor White
    Write-Host "3. Upload: Releases\AztecQRGenerator_v1.3.0.zip" -ForegroundColor White
    Write-Host "4. Copy description from: RELEASE_NOTES_v1.3.0.md" -ForegroundColor White
    Write-Host "5. Click 'Publish release'" -ForegroundColor White
    Write-Host ""
    
    # Open browser
    $open = Read-Host "Open GitHub release page? (Y/N)"
    if ($open -eq "Y") {
        Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.3.0"
    }
    
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
    exit 1
}
