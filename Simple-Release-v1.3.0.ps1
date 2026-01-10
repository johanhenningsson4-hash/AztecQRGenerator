# Simple Release v1.3.0 - Manual Steps
# Use this if the automated script has issues

$ErrorActionPreference = "Stop"
$version = "1.3.0"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Simple Release v$version" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$repoPath = "C:\Jobb\AztecQRGenerator"
Set-Location $repoPath

Write-Host "This script will guide you through creating release v$version" -ForegroundColor Yellow
Write-Host ""

# Step 1: Update versions manually
Write-Host "[Step 1] Update Version Numbers" -ForegroundColor Cyan
Write-Host "Edit these files and change '1.2.3' to '$version':" -ForegroundColor White
Write-Host "  - AztecQRGenerator.Core\Properties\AssemblyInfo.cs" -ForegroundColor Gray
Write-Host "  - AztecQRGenerator.Core\AztecQRGenerator.Core.csproj" -ForegroundColor Gray
Write-Host "  - AztecQRGenerator.Core\AztecQRGenerator.Core.nuspec" -ForegroundColor Gray
Write-Host "  - Properties\AssemblyInfo.cs" -ForegroundColor Gray
Write-Host "  - AztecQRGenerator.Tests\Properties\AssemblyInfo.cs" -ForegroundColor Gray
Write-Host ""
$done = Read-Host "Done updating versions? (Y to continue)"
if ($done -ne "Y" -and $done -ne "y") {
    Write-Host "Exiting..." -ForegroundColor Yellow
    exit
}
Write-Host ""

# Step 2: Build in Visual Studio
Write-Host "[Step 2] Build Solution" -ForegroundColor Cyan
Write-Host "In Visual Studio:" -ForegroundColor White
Write-Host "  1. Select 'Release' configuration" -ForegroundColor Gray
Write-Host "  2. Build > Rebuild Solution (Ctrl+Shift+B)" -ForegroundColor Gray
Write-Host "  3. Verify build is successful" -ForegroundColor Gray
Write-Host ""
$done = Read-Host "Build successful? (Y to continue)"
if ($done -ne "Y" -and $done -ne "y") {
    Write-Host "Fix build errors and try again" -ForegroundColor Yellow
    exit
}
Write-Host ""

# Step 3: Run tests
Write-Host "[Step 3] Run Tests" -ForegroundColor Cyan
Write-Host "In Visual Studio:" -ForegroundColor White
Write-Host "  1. Open Test Explorer (Test > Test Explorer)" -ForegroundColor Gray
Write-Host "  2. Click 'Run All Tests'" -ForegroundColor Gray
Write-Host "  3. Verify all 58 tests pass" -ForegroundColor Gray
Write-Host ""
$done = Read-Host "All tests passing? (Y to continue, N to skip tests)"
if ($done -ne "Y" -and $done -ne "y" -and $done -ne "N" -and $done -ne "n") {
    Write-Host "Exiting..." -ForegroundColor Yellow
    exit
}
Write-Host ""

# Step 4: Create NuGet package (using Visual Studio or manually)
Write-Host "[Step 4] Create NuGet Package" -ForegroundColor Cyan
Write-Host "Option A - Using Visual Studio NuGet Package Manager Console:" -ForegroundColor White
Write-Host "  1. Tools > NuGet Package Manager > Package Manager Console" -ForegroundColor Gray
Write-Host "  2. Run: cd AztecQRGenerator.Core" -ForegroundColor Gray
Write-Host "  3. Run: nuget pack AztecQRGenerator.Core.nuspec -Properties Configuration=Release" -ForegroundColor Gray
Write-Host ""
Write-Host "Option B - Skip for now (GitHub Actions will create it)" -ForegroundColor White
Write-Host ""
$createPackage = Read-Host "Created package? (Y/N/Skip)"
Write-Host ""

# Step 5: Commit changes
Write-Host "[Step 5] Commit Changes" -ForegroundColor Cyan
Write-Host "Run these commands:" -ForegroundColor White
Write-Host ""
Write-Host "git add ." -ForegroundColor Yellow
Write-Host 'git commit -m "feat: Release v1.3.0 - Automated Testing & CI/CD"' -ForegroundColor Yellow
Write-Host ""
$done = Read-Host "Run commands above? (Y/Skip)"
if ($done -eq "Y" -or $done -eq "y") {
    git add .
    git commit -m "feat: Release v1.3.0 - Automated Testing & CI/CD"
    Write-Host "  ? Changes committed" -ForegroundColor Green
}
Write-Host ""

# Step 6: Create tag
Write-Host "[Step 6] Create Git Tag" -ForegroundColor Cyan
Write-Host "Run these commands:" -ForegroundColor White
Write-Host ""
Write-Host "git tag -a v$version -m ""Release v$version""" -ForegroundColor Yellow
Write-Host ""
$done = Read-Host "Create tag? (Y/Skip)"
if ($done -eq "Y" -or $done -eq "y") {
    git tag -a "v$version" -m "Release v$version - Automated Testing & CI/CD"
    Write-Host "  ? Tag created" -ForegroundColor Green
}
Write-Host ""

# Step 7: Push
Write-Host "[Step 7] Push to GitHub" -ForegroundColor Cyan
Write-Host "Run these commands:" -ForegroundColor White
Write-Host ""
Write-Host "git push origin main" -ForegroundColor Yellow
Write-Host "git push origin v$version" -ForegroundColor Yellow
Write-Host ""
$done = Read-Host "Push now? (Y/Skip)"
if ($done -eq "Y" -or $done -eq "y") {
    git push origin main
    git push origin "v$version"
    Write-Host "  ? Pushed to GitHub" -ForegroundColor Green
}
Write-Host ""

# Step 8: Create GitHub Release
Write-Host "[Step 8] Create GitHub Release" -ForegroundColor Cyan
Write-Host "Go to: https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v$version" -ForegroundColor White
Write-Host ""
Write-Host "Fill in:" -ForegroundColor White
Write-Host "  Title: v$version - Automated Testing & CI/CD" -ForegroundColor Gray
Write-Host "  Description: Copy from RELEASE_NOTES_v$version.md" -ForegroundColor Gray
Write-Host "  Attach: AztecQRGenerator.Core\AztecQRGenerator.Core.$version.nupkg (if created)" -ForegroundColor Gray
Write-Host "  Check: 'Set as the latest release'" -ForegroundColor Gray
Write-Host ""

# Copy release notes
if (Test-Path "RELEASE_NOTES_v$version.md") {
    try {
        Get-Content "RELEASE_NOTES_v$version.md" -Raw | Set-Clipboard
        Write-Host "  ? Release notes copied to clipboard!" -ForegroundColor Green
    } catch {
        Write-Host "  ?? Clipboard not available" -ForegroundColor Gray
    }
}

$openBrowser = Read-Host "Open GitHub release page? (Y/N)"
if ($openBrowser -eq "Y" -or $openBrowser -eq "y") {
    Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v$version"
    Write-Host "  ? Browser opened" -ForegroundColor Green
}
Write-Host ""

# Summary
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "? Release Steps Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "What happens after you publish the GitHub release:" -ForegroundColor Yellow
Write-Host "  ? GitHub Actions will:" -ForegroundColor Cyan
Write-Host "     - Build the Core library" -ForegroundColor White
Write-Host "     - Create NuGet package" -ForegroundColor White
Write-Host "     - Publish to NuGet.org (using your NUGET_API_KEY secret)" -ForegroundColor White
Write-Host "     - Run all CI tests" -ForegroundColor White
Write-Host ""
Write-Host "  ?? Package will be available on NuGet.org in ~10-15 minutes" -ForegroundColor Cyan
Write-Host ""
Write-Host "Monitor progress:" -ForegroundColor Yellow
Write-Host "  Actions: https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions" -ForegroundColor White
Write-Host "  NuGet: https://www.nuget.org/packages/AztecQRGenerator.Core" -ForegroundColor White
Write-Host ""

