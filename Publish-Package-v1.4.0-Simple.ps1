# NuGet Publishing Script for AztecQRGenerator.Core v1.4.0
# Simplified version to avoid PowerShell parsing issues

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "NuGet Package Publishing - v1.4.0" -ForegroundColor Cyan  
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Version Check
Write-Host "[1/7] Version Check" -ForegroundColor Yellow
$projectFile = "AztecQRGenerator.Core\AztecQRGenerator.Core.csproj"

if (Test-Path $projectFile) {
    $content = Get-Content $projectFile -Raw
    if ($content -match '1\.4\.0') {
        Write-Host "  ✓ Found version 1.4.0 in project file" -ForegroundColor Green
    } else {
        Write-Host "  ✗ Version 1.4.0 not found in project file!" -ForegroundColor Red
        Write-Host "    Update $projectFile and change Version to 1.4.0" -ForegroundColor Yellow
        $continue = Read-Host "    Continue anyway? (Y/N)"
        if ($continue -ne "Y") { exit }
    }
}

Write-Host ""

# Step 2: Build Release
Write-Host "[2/7] Building Release Package" -ForegroundColor Yellow
Write-Host "  Running release build..." -ForegroundColor Gray

$buildResult = & .\build_and_test.ps1 -Solution AztecQRGenerator.sln -Configuration Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "  ✗ Build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "  ✓ Build successful!" -ForegroundColor Green

Write-Host ""

# Step 3: Find Package
Write-Host "[3/7] Locating Package" -ForegroundColor Yellow

$packageFile = $null
$searchPaths = @(
    "AztecQRGenerator.Core\bin\Release\AztecQRGenerator.Core.1.4.0.nupkg"
    "packages\AztecQRGenerator.Core.1.4.0\AztecQRGenerator.Core.1.4.0.nupkg"
)

foreach ($path in $searchPaths) {
    if (Test-Path $path) {
        $packageFile = $path
        break
    }
}

if (-not $packageFile) {
    $found = Get-ChildItem -Recurse -Filter "AztecQRGenerator.Core.1.4.0.nupkg" -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($found) {
        $packageFile = $found.FullName
    }
}

if ($packageFile) {
    Write-Host "  ✓ Package found: $packageFile" -ForegroundColor Green
} else {
    Write-Host "  ✗ Package not found!" -ForegroundColor Red
    Write-Host "    Make sure version is updated to 1.4.0 in project file" -ForegroundColor Yellow
    exit
}

Write-Host ""

# Step 4: Package Validation
Write-Host "[4/7] Package Validation" -ForegroundColor Yellow

$tempDir = "temp_validation"
Remove-Item $tempDir -Recurse -Force -ErrorAction SilentlyContinue

Expand-Archive $packageFile -DestinationPath $tempDir -ErrorAction SilentlyContinue

$dllExists = Test-Path "$tempDir\lib\net472\AztecQRGenerator.Core.dll"
$readmeExists = Test-Path "$tempDir\NUGET_README.md" 
$iconExists = Test-Path "$tempDir\icon.png"

if ($dllExists) { Write-Host "  ✓ DLL found" -ForegroundColor Green }
if ($readmeExists) { Write-Host "  ✓ README found" -ForegroundColor Green }
if ($iconExists) { Write-Host "  ✓ Icon found" -ForegroundColor Green }

Remove-Item $tempDir -Recurse -Force -ErrorAction SilentlyContinue

Write-Host ""

# Step 5: Get API Key
Write-Host "[5/7] NuGet API Key" -ForegroundColor Yellow
Write-Host "  Get your API key from: https://www.nuget.org/account/apikeys" -ForegroundColor Gray
$apiKey = Read-Host "  Enter API Key (or press Enter to skip)"

Write-Host ""

# Step 6: Publish
Write-Host "[6/7] Publishing" -ForegroundColor Yellow

if ([string]::IsNullOrWhiteSpace($apiKey)) {
    Write-Host "  Manual publishing commands:" -ForegroundColor Gray
    Write-Host "  dotnet nuget push `"$packageFile`" --api-key YOUR_KEY --source https://api.nuget.org/v3/index.json" -ForegroundColor Cyan
    Write-Host "  OR upload at: https://www.nuget.org/packages/manage/upload" -ForegroundColor Cyan
} else {
    $confirm = Read-Host "  Publish to NuGet.org? Type YES to confirm"
    if ($confirm -eq "YES") {
        Write-Host "  Publishing..." -ForegroundColor Cyan
        & dotnet nuget push $packageFile --api-key $apiKey --source https://api.nuget.org/v3/index.json
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  ✓ Published successfully!" -ForegroundColor Green
        } else {
            Write-Host "  ✗ Publish failed!" -ForegroundColor Red
        }
    }
}

Write-Host ""

# Step 7: Git Operations
Write-Host "[7/7] Git Operations" -ForegroundColor Yellow

Write-Host "  Committing and tagging..." -ForegroundColor Gray
& git add .
& git commit -m "v1.4.0 release"

$existingTag = & git tag -l "v1.4.0"
if ($existingTag) {
    Write-Host "  Tag v1.4.0 already exists" -ForegroundColor Yellow
    $recreate = Read-Host "  Delete and recreate? (Y/N)"
    if ($recreate -eq "Y") {
        & git tag -d v1.4.0
        & git push origin --delete v1.4.0 2>$null
    }
}

& git tag -a v1.4.0 -m "Release v1.4.0"
& git push origin main
& git push origin v1.4.0

Write-Host "  ✓ Git operations complete!" -ForegroundColor Green

Write-Host ""

# Final Instructions
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "           ALMOST DONE!                 " -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Create GitHub Release:" -ForegroundColor White
Write-Host "   https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.4.0" -ForegroundColor Cyan
Write-Host ""
Write-Host "2. Release Description:" -ForegroundColor White
Write-Host "   AztecQRGenerator.Core v1.4.0 - Major CI/CD and Testing Improvements" -ForegroundColor Gray
Write-Host ""
Write-Host "3. Key Features:" -ForegroundColor White
Write-Host "   - Comprehensive GitHub Actions CI/CD pipeline" -ForegroundColor Gray
Write-Host "   - Enhanced test coverage with NUnit + conditional MSTest shims" -ForegroundColor Gray  
Write-Host "   - Automated build script (build_and_test.ps1)" -ForegroundColor Gray
Write-Host "   - Code coverage reports (HTML + Cobertura)" -ForegroundColor Gray
Write-Host "   - StyleCop cleanup and code quality improvements" -ForegroundColor Gray
Write-Host "   - Updated documentation with CI information" -ForegroundColor Gray
Write-Host ""
Write-Host "4. Installation:" -ForegroundColor White
Write-Host "   Install-Package AztecQRGenerator.Core -Version 1.4.0" -ForegroundColor Green
Write-Host "   dotnet add package AztecQRGenerator.Core --version 1.4.0" -ForegroundColor Green

$openGitHub = Read-Host "`nOpen GitHub releases page? (Y/N)"
if ($openGitHub -eq "Y") {
    Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.4.0"
}

Write-Host "`nDone! 🎉" -ForegroundColor Green
