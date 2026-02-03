# NuGet Publishing Script for AztecQRGenerator.Core v1.4.0
# Minimal version - no complex PowerShell syntax

Write-Host "NuGet Package Publishing - v1.4.0" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Version Check
Write-Host "[1] Checking version..." -ForegroundColor Yellow
$projectFile = "AztecQRGenerator.Core\AztecQRGenerator.Core.csproj"
$content = Get-Content $projectFile -Raw -ErrorAction SilentlyContinue
$hasVersion = $content -match '1\.4\.0'

Write-Host "Version 1.4.0 found: $hasVersion" -ForegroundColor Green
Write-Host ""

# Step 2: Build
Write-Host "[2] Building Release..." -ForegroundColor Yellow
& .\build_and_test.ps1 -Solution AztecQRGenerator.sln -Configuration Release
Write-Host "Build exit code: $LASTEXITCODE" -ForegroundColor Green
Write-Host ""

# Step 3: Find Package
Write-Host "[3] Looking for package..." -ForegroundColor Yellow
$pkg1 = "AztecQRGenerator.Core\bin\Release\AztecQRGenerator.Core.1.4.0.nupkg"
$pkg2 = "packages\AztecQRGenerator.Core.1.4.0\AztecQRGenerator.Core.1.4.0.nupkg"

$packageFile = $null
if (Test-Path $pkg1) { $packageFile = $pkg1 }
if (Test-Path $pkg2) { $packageFile = $pkg2 }

Write-Host "Package file: $packageFile" -ForegroundColor Green
Write-Host ""

# Step 4: Validation
Write-Host "[4] Validating package..." -ForegroundColor Yellow
if ($packageFile) {
    $tempDir = "temp_validation"
    Remove-Item $tempDir -Recurse -Force -ErrorAction SilentlyContinue
    Expand-Archive $packageFile -DestinationPath $tempDir -ErrorAction SilentlyContinue
    
    $dll = Test-Path "$tempDir\lib\net472\AztecQRGenerator.Core.dll"
    $readme = Test-Path "$tempDir\NUGET_README.md"
    $icon = Test-Path "$tempDir\icon.png"
    
    Write-Host "DLL exists: $dll" -ForegroundColor Green
    Write-Host "README exists: $readme" -ForegroundColor Green  
    Write-Host "Icon exists: $icon" -ForegroundColor Green
    
    Remove-Item $tempDir -Recurse -Force -ErrorAction SilentlyContinue
}
Write-Host ""

# Step 5: API Key
Write-Host "[5] NuGet API Key" -ForegroundColor Yellow
Write-Host "Get API key from: https://www.nuget.org/account/apikeys" -ForegroundColor Gray
$apiKey = Read-Host "Enter API Key (or Enter to skip)"
Write-Host ""

# Step 6: Publishing
Write-Host "[6] Publishing Options" -ForegroundColor Yellow

if ($apiKey) {
    Write-Host "Manual publish command:" -ForegroundColor Gray
    Write-Host "dotnet nuget push `"$packageFile`" --api-key $apiKey --source https://api.nuget.org/v3/index.json" -ForegroundColor Cyan
    
    $doPublish = Read-Host "Run publish command now? (Y/N)"
    if ($doPublish -eq "Y") {
        & dotnet nuget push $packageFile --api-key $apiKey --source https://api.nuget.org/v3/index.json
        Write-Host "Publish exit code: $LASTEXITCODE" -ForegroundColor Green
    }
} else {
    Write-Host "Manual commands (replace YOUR_KEY):" -ForegroundColor Gray
    Write-Host "dotnet nuget push `"$packageFile`" --api-key YOUR_KEY --source https://api.nuget.org/v3/index.json" -ForegroundColor Cyan
    Write-Host "OR upload at: https://www.nuget.org/packages/manage/upload" -ForegroundColor Cyan
}
Write-Host ""

# Step 7: Git
Write-Host "[7] Git Operations" -ForegroundColor Yellow
& git add .
& git commit -m "v1.4.0 release"
Write-Host "Commit exit code: $LASTEXITCODE" -ForegroundColor Green

& git tag -a v1.4.0 -m "Release v1.4.0"
Write-Host "Tag exit code: $LASTEXITCODE" -ForegroundColor Green

& git push origin main
& git push origin v1.4.0
Write-Host "Push exit code: $LASTEXITCODE" -ForegroundColor Green

Write-Host ""

# Final Steps
Write-Host "FINAL STEPS:" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Create GitHub Release at:" -ForegroundColor White
Write-Host "   https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.4.0" -ForegroundColor Cyan
Write-Host ""
Write-Host "2. Title: v1.4.0 - CI/CD and Testing Improvements" -ForegroundColor White
Write-Host ""
Write-Host "3. Description:" -ForegroundColor White
Write-Host "   Major improvements to CI/CD pipeline, testing, and code quality" -ForegroundColor Gray
Write-Host ""
Write-Host "4. Installation for users:" -ForegroundColor White
Write-Host "   Install-Package AztecQRGenerator.Core -Version 1.4.0" -ForegroundColor Green
Write-Host ""

$openBrowser = Read-Host "Open GitHub releases page? (Y/N)"
if ($openBrowser -eq "Y") {
    Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.4.0"
}

Write-Host ""
Write-Host "Done!" -ForegroundColor Green
