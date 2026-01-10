# Update README and Push to Git
# Complete automation for updating README with CI/CD info and pushing

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Update README & Push to Git" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Set-Location "C:\Jobb\AztecQRGenerator"

# Step 1: Check Git status
Write-Host "[1/4] Checking Git status..." -ForegroundColor Yellow
$status = git status --short

if ($status) {
    Write-Host "  Files changed:" -ForegroundColor Gray
    $status | ForEach-Object { Write-Host "    $_" -ForegroundColor Gray }
} else {
    Write-Host "  ? No changes detected" -ForegroundColor Green
    Write-Host ""
    Write-Host "README may already be up to date." -ForegroundColor Yellow
    $continue = Read-Host "Continue anyway? (Y/N)"
    if ($continue -ne "Y" -and $continue -ne "y") {
        exit 0
    }
}
Write-Host ""

# Step 2: Review changes
Write-Host "[2/4] Reviewing README changes..." -ForegroundColor Yellow

if (Test-Path "README.md") {
    Write-Host "  ? README.md found" -ForegroundColor Green
    
    # Show diff if available
    try {
        $diff = git diff README.md
        if ($diff) {
            Write-Host ""
            Write-Host "  Changes to README.md:" -ForegroundColor Cyan
            Write-Host "  =====================" -ForegroundColor Cyan
            $diff | Select-Object -First 20 | ForEach-Object { Write-Host "  $_" -ForegroundColor Gray }
            if ($diff.Count -gt 20) {
                Write-Host "  ... (showing first 20 lines)" -ForegroundColor Gray
            }
        } else {
            Write-Host "  ?? No changes to README.md" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "  Could not show diff" -ForegroundColor Gray
    }
} else {
    Write-Host "  ?? README.md not found" -ForegroundColor Yellow
}
Write-Host ""

$continue = Read-Host "Continue with commit? (Y/N)"
if ($continue -ne "Y" -and $continue -ne "y") {
    Write-Host "Aborted." -ForegroundColor Yellow
    exit 0
}
Write-Host ""

# Step 3: Stage and commit
Write-Host "[3/4] Committing changes..." -ForegroundColor Yellow

# Stage all changes
git add .

# Commit message
$commitMessage = @"
docs: Update README with CI/CD and testing information

Updates:
- Added CI/CD status badges
- Added Quality Assurance section with 58 unit tests info
- Updated Recent Updates with v1.3.0 information
- Added For Developers section with CI/CD pipeline details
- Added links to testing and CI/CD documentation
- Updated installation instructions with latest version

Features Highlighted:
- Automated testing on every commit
- CI/CD pipeline with GitHub Actions
- Automated NuGet publishing
- Weekly security scans
- Test reporting

No code changes - documentation only
"@

git commit -m $commitMessage

if ($LASTEXITCODE -eq 0) {
    Write-Host "  ? Changes committed" -ForegroundColor Green
} else {
    Write-Host "  ?? Nothing to commit or commit failed" -ForegroundColor Yellow
}
Write-Host ""

# Step 4: Push to GitHub
Write-Host "[4/4] Pushing to GitHub..." -ForegroundColor Yellow
Write-Host ""

$push = Read-Host "Push to GitHub now? (Y/N)"

if ($push -eq "Y" -or $push -eq "y") {
    Write-Host ""
    Write-Host "Pushing to main branch..." -ForegroundColor Gray
    
    git push origin main
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ? Successfully pushed to GitHub" -ForegroundColor Green
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host "? README Updated & Pushed!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "? README now includes:" -ForegroundColor Green
        Write-Host "  ? CI/CD status badges" -ForegroundColor White
        Write-Host "  ? 58 unit tests information" -ForegroundColor White
        Write-Host "  ? Quality assurance section" -ForegroundColor White
        Write-Host "  ? CI/CD pipeline details" -ForegroundColor White
        Write-Host "  ? v1.3.0 release information" -ForegroundColor White
        Write-Host "  ? Developer documentation" -ForegroundColor White
        Write-Host ""
        Write-Host "?? View on GitHub:" -ForegroundColor Cyan
        Write-Host "  https://github.com/johanhenningsson4-hash/AztecQRGenerator" -ForegroundColor White
        Write-Host ""
        
        # Trigger CI
        Write-Host "? GitHub Actions CI workflow will now:" -ForegroundColor Cyan
        Write-Host "  1. Build the solution" -ForegroundColor White
        Write-Host "  2. Run all 58 tests" -ForegroundColor White
        Write-Host "  3. Generate test reports" -ForegroundColor White
        Write-Host ""
        Write-Host "  Monitor at: https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions" -ForegroundColor Cyan
        Write-Host ""
        
        $openGitHub = Read-Host "Open GitHub repository? (Y/N)"
        if ($openGitHub -eq "Y" -or $openGitHub -eq "y") {
            Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator"
        }
        
        $openActions = Read-Host "Open GitHub Actions? (Y/N)"
        if ($openActions -eq "Y" -or $openActions -eq "y") {
            Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions"
        }
        
    } else {
        Write-Host "  ? Push failed" -ForegroundColor Red
        Write-Host ""
        Write-Host "Check your credentials and network connection." -ForegroundColor Yellow
        Write-Host "You can push manually later with:" -ForegroundColor Yellow
        Write-Host "  git push origin main" -ForegroundColor Cyan
    }
} else {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "? Changes Committed (Not Pushed)" -ForegroundColor Yellow
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Changes are committed locally." -ForegroundColor White
    Write-Host "To push later, run:" -ForegroundColor Yellow
    Write-Host "  git push origin main" -ForegroundColor Cyan
    Write-Host ""
}

Write-Host ""
Write-Host "?? Done!" -ForegroundColor Green
Write-Host ""
