# NuGet Publishing Script for AztecQRGenerator.Core v1.4.0
# Updated for comprehensive CI/CD and testing improvements

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "NuGet Package Publishing - v1.4.0" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Pre-flight checks
Write-Host "[1/8] Pre-flight Verification" -ForegroundColor Yellow
Write-Host "  Checking version consistency..." -ForegroundColor Gray

$projectFile = "AztecQRGenerator.Core\AztecQRGenerator.Core.csproj"
$assemblyInfo = "AztecQRGenerator.Core\Properties\AssemblyInfo.cs"
$mainAssemblyInfo = "Properties\AssemblyInfo.cs"

# Check if .csproj version is updated
if (Test-Path $projectFile) {
    $csprojContent = Get-Content $projectFile -Raw
    if ($csprojContent -match 'Version.*1\.4\.0.*Version') {
        Write-Host "  ✓ Project file version: 1.4.0" -ForegroundColor Green
    } else {
        Write-Host "  ✗ Project file version needs update!" -ForegroundColor Red
        Write-Host "    Please update VERSION in $projectFile to 1.4.0" -ForegroundColor Yellow
        Write-Host "    Close Visual Studio, edit the file, change VERSION to 1.4.0" -ForegroundColor Cyan
        $continue = Read-Host "    Continue anyway? (Y/N)"
        if ($continue -ne "Y" -and $continue -ne "y") {
            exit
        }
    }
}

# Check assembly versions
if (Test-Path $assemblyInfo) {
    $asmContent = Get-Content $assemblyInfo -Raw
    if ($asmContent -match 'AssemblyVersion\("1\.4\.0\.0"\)') {
        Write-Host "  ✓ Core AssemblyVersion: 1.4.0.0" -ForegroundColor Green
    } else {
        Write-Host "  ⚠ Core AssemblyVersion may need update" -ForegroundColor Yellow
    }
}

if (Test-Path $mainAssemblyInfo) {
    $mainAsmContent = Get-Content $mainAssemblyInfo -Raw
    if ($mainAsmContent -match 'AssemblyVersion\("1\.4\.0\.0"\)') {
        Write-Host "  ✓ Main AssemblyVersion: 1.4.0.0" -ForegroundColor Green
    } else {
        Write-Host "  ⚠ Main AssemblyVersion may need update" -ForegroundColor Yellow
    }
}

Write-Host ""

# Build Release Package
Write-Host "[2/8] Building Release Package" -ForegroundColor Yellow
Write-Host "  Running Release build with package generation..." -ForegroundColor Gray

try {
    & .\build_and_test.ps1 -Solution AztecQRGenerator.sln -Configuration Release
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ✓ Release build successful!" -ForegroundColor Green
    } else {
        Write-Host "  ✗ Build failed!" -ForegroundColor Red
        exit 1
    }
}
catch {
    Write-Host "  ✗ Build error: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Find the generated package
Write-Host "[3/8] Locating Generated Package" -ForegroundColor Yellow

$packageFile = $null
$searchPaths = @(
    "AztecQRGenerator.Core\bin\Release\AztecQRGenerator.Core.1.4.0.nupkg",
    "packages\AztecQRGenerator.Core.1.4.0\AztecQRGenerator.Core.1.4.0.nupkg"
)

# Try to find the package
foreach ($path in $searchPaths) {
    if (Test-Path $path) {
        $packageFile = $path
        break
    }
}

# If not found, search recursively
if (-not $packageFile) {
    $found = Get-ChildItem -Recurse -Filter "AztecQRGenerator.Core.1.4.0.nupkg" -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($found) {
        $packageFile = $found.FullName
    }
}

if ($packageFile) {
    $fileInfo = Get-Item $packageFile
    Write-Host "  ✓ Package found: $($fileInfo.Name)" -ForegroundColor Green
    Write-Host "    Location: $packageFile" -ForegroundColor Gray
    Write-Host "    Size: $([math]::Round($fileInfo.Length / 1KB, 1)) KB" -ForegroundColor Gray
    Write-Host "    Created: $($fileInfo.LastWriteTime)" -ForegroundColor Gray
} else {
    Write-Host "  ✗ Package not found!" -ForegroundColor Red
    Write-Host "    Expected: AztecQRGenerator.Core.1.4.0.nupkg" -ForegroundColor Yellow
    Write-Host "    Make sure the project file version is updated to 1.4.0" -ForegroundColor Yellow
    $manual = Read-Host "    Continue with manual package creation? (Y/N)"
    if ($manual -eq "Y" -or $manual -eq "y") {
        Write-Host "    Run: msbuild AztecQRGenerator.Core\AztecQRGenerator.Core.csproj /p:Configuration=Release" -ForegroundColor Cyan
    }
    exit
}

Write-Host ""

# Package Validation
Write-Host "[4/8] Package Validation" -ForegroundColor Yellow
Write-Host "  Validating package contents..." -ForegroundColor Gray

$tempDir = "temp_package_validation"
if (Test-Path $tempDir) { 
    Remove-Item $tempDir -Recurse -Force 
}

try {
    Expand-Archive $packageFile -DestinationPath $tempDir
    
    $expectedFiles = @(
        "lib\net472\AztecQRGenerator.Core.dll",
        "NUGET_README.md",
        "icon.png"
    )
    
    $validationPassed = $true
    foreach ($expectedFile in $expectedFiles) {
        $fullPath = Join-Path $tempDir $expectedFile
        if (Test-Path $fullPath) {
            Write-Host "    ✓ $expectedFile" -ForegroundColor Green
        } else {
            Write-Host "    ✗ Missing: $expectedFile" -ForegroundColor Red
            $validationPassed = $false
        }
    }
    
    if ($validationPassed) {
        Write-Host "  ✓ Package validation passed!" -ForegroundColor Green
    } else {
        Write-Host "  ✗ Package validation failed!" -ForegroundColor Red
        $continue = Read-Host "    Continue anyway? (Y/N)"
        if ($continue -ne "Y" -and $continue -ne "y") {
            exit
        }
    }
    
    Remove-Item $tempDir -Recurse -Force -ErrorAction SilentlyContinue
}
catch {
    Write-Host "  ⚠ Could not validate package contents: $_" -ForegroundColor Yellow
}

Write-Host ""

# Test Installation (Optional)
Write-Host "[5/8] Local Package Testing (Optional)" -ForegroundColor Yellow
Write-Host "  You can test the package locally before publishing:" -ForegroundColor Gray
Write-Host "  1. Create test project: dotnet new console -n TestAztecQR" -ForegroundColor Cyan
Write-Host "  2. Add local package: dotnet add package AztecQRGenerator.Core --version 1.4.0 --source $(pwd)" -ForegroundColor Cyan
Write-Host "  3. Test the functionality with sample code" -ForegroundColor Cyan
Write-Host ""

$testNow = Read-Host "  Skip local testing and proceed to publishing? (Y/N)"
if ($testNow -ne "Y" -and $testNow -ne "y") {
    Write-Host "  Test the package first, then run this script again." -ForegroundColor Yellow
    exit
}

Write-Host ""

# NuGet API Key
Write-Host "[6/8] NuGet.org Authentication" -ForegroundColor Yellow
Write-Host "  You need an API key from https://www.nuget.org/account/apikeys" -ForegroundColor Gray
Write-Host ""

$apiKey = Read-Host "  Enter your NuGet.org API Key (or press Enter to skip and get manual commands)"

Write-Host ""

if ([string]::IsNullOrWhiteSpace($apiKey)) {
    Write-Host "[7/8] Manual Publishing Commands" -ForegroundColor Yellow
    Write-Host "  Use one of these commands to publish:" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  Option 1 - dotnet CLI:" -ForegroundColor Cyan
    Write-Host "    dotnet nuget push `"$packageFile`" --api-key YOUR_KEY --source https://api.nuget.org/v3/index.json" -ForegroundColor White
    Write-Host ""
    Write-Host "  Option 2 - nuget.exe:" -ForegroundColor Cyan
    Write-Host "    nuget push `"$packageFile`" -ApiKey YOUR_KEY -Source https://api.nuget.org/v3/index.json" -ForegroundColor White
    Write-Host ""
    Write-Host "  Option 3 - NuGet.org Web Interface:" -ForegroundColor Cyan
    Write-Host "    1. Go to: https://www.nuget.org/packages/manage/upload" -ForegroundColor White
    Write-Host "    2. Upload: $packageFile" -ForegroundColor White
} else {
    # Publish Package
    Write-Host "[7/8] Publishing to NuGet.org" -ForegroundColor Yellow
    Write-Host "  This will publish AztecQRGenerator.Core v1.4.0 to the public repository." -ForegroundColor Gray
    Write-Host ""
    
    $confirm = Read-Host "  Are you sure you want to publish? Type 'YES' to confirm"
    
    if ($confirm -eq "YES") {
        Write-Host "  Publishing package..." -ForegroundColor Cyan
        
        try {
            & dotnet nuget push $packageFile --api-key $apiKey --source https://api.nuget.org/v3/index.json
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "  ✓ Package published successfully!" -ForegroundColor Green
                Write-Host "  ✓ Processing time: 10-15 minutes for NuGet.org indexing" -ForegroundColor Yellow
                Write-Host "  🌐 Package URL: https://www.nuget.org/packages/AztecQRGenerator.Core/1.4.0" -ForegroundColor Cyan
            } else {
                Write-Host "  ✗ Publishing failed!" -ForegroundColor Red
                Write-Host "    Check your API key and network connection" -ForegroundColor Yellow
                exit 1
            }
        }
        catch {
            Write-Host "  ✗ Publishing error: $_" -ForegroundColor Red
            exit 1
        }
    } else {
        Write-Host "  ✗ Publishing cancelled." -ForegroundColor Yellow
        exit
    }
}

Write-Host ""

# Git Operations
Write-Host "[8/8] Git Release Operations" -ForegroundColor Yellow

# Commit version changes
Write-Host "  Committing version updates..." -ForegroundColor Gray
& git add .
& git commit -m "v1.4.0: Release with comprehensive CI/CD and testing improvements"

# Tag the release
Write-Host "  Creating git tag v1.4.0..." -ForegroundColor Gray

$tagExists = & git tag -l "v1.4.0"
if ($tagExists) {
    Write-Host "  ⚠ Tag v1.4.0 already exists" -ForegroundColor Yellow
    $recreateTag = Read-Host "    Delete and recreate tag? (Y/N)"
    
    if ($recreateTag -eq "Y" -or $recreateTag -eq "y") {
        & git tag -d v1.4.0
        & git push origin --delete v1.4.0 2>$null
        Write-Host "    Old tag removed" -ForegroundColor Gray
    }
}

# Create new tag
& git tag -a v1.4.0 -m "Release v1.4.0 - Comprehensive CI/CD and testing improvements"
Write-Host "  ✓ Tag v1.4.0 created locally" -ForegroundColor Green

# Push to GitHub
Write-Host "  Pushing to GitHub..." -ForegroundColor Cyan
& git push origin main
& git push origin v1.4.0

Write-Host "  ✓ Pushed to GitHub" -ForegroundColor Green
Write-Host ""

# GitHub Release Instructions
Write-Host "[MANUAL] Create GitHub Release" -ForegroundColor Yellow
Write-Host "  Complete these steps manually:" -ForegroundColor Gray
Write-Host "  1. Go to: https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new" -ForegroundColor Cyan
Write-Host "  2. Select tag: v1.4.0" -ForegroundColor Cyan
Write-Host "  3. Release title: v1.4.0 - CI/CD and Testing Improvements" -ForegroundColor Cyan
Write-Host ""
Write-Host "  4. Description (copy this text):" -ForegroundColor Cyan
Write-Host ""
Write-Host "## AztecQRGenerator.Core v1.4.0" -ForegroundColor White
Write-Host ""
Write-Host "### Major Improvements" -ForegroundColor White
Write-Host "- CI/CD Pipeline: Comprehensive GitHub Actions workflow with MSBuild + vstest" -ForegroundColor White
Write-Host "- Enhanced Testing: Proper NUnit assertions and conditional MSTest shims" -ForegroundColor White
Write-Host "- Build Automation: Added build_and_test.ps1 script for consistent builds" -ForegroundColor White
Write-Host "- Code Coverage: Integrated ReportGenerator for HTML and Cobertura reports" -ForegroundColor White
Write-Host "- Code Quality: Fixed numerous StyleCop warnings and improved consistency" -ForegroundColor White
Write-Host "- Documentation: Updated README with CI information and workflows" -ForegroundColor White
Write-Host ""
Write-Host "### Installation" -ForegroundColor White
Write-Host "Install-Package AztecQRGenerator.Core -Version 1.4.0" -ForegroundColor White
Write-Host "dotnet add package AztecQRGenerator.Core --version 1.4.0" -ForegroundColor White
Write-Host ""
Write-Host "  5. Attach file: $packageFile" -ForegroundColor Cyan
Write-Host "  6. Click 'Publish release'" -ForegroundColor Cyan
Write-Host ""

$openGitHub = Read-Host "  Open GitHub releases page? (Y/N)"
if ($openGitHub -eq "Y" -or $openGitHub -eq "y") {
    Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.4.0"
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "        PUBLISHING COMPLETE!        " -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Summary:" -ForegroundColor Yellow
Write-Host "  ✓ Package: AztecQRGenerator.Core.1.4.0.nupkg" -ForegroundColor Green
if (-not [string]::IsNullOrWhiteSpace($apiKey)) {
    Write-Host "  ✓ Published: NuGet.org" -ForegroundColor Green
}
Write-Host "  ✓ Git tagged: v1.4.0" -ForegroundColor Green
Write-Host "  ⏳ TODO: Complete GitHub release" -ForegroundColor Yellow
Write-Host ""

Write-Host "Important Links:" -ForegroundColor Yellow
Write-Host "  NuGet: https://www.nuget.org/packages/AztecQRGenerator.Core/" -ForegroundColor Cyan
Write-Host "  GitHub: https://github.com/johanhenningsson4-hash/AztecQRGenerator" -ForegroundColor Cyan
Write-Host "  Releases: https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases" -ForegroundColor Cyan
Write-Host "  CI/CD: https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions" -ForegroundColor Cyan
Write-Host ""

Write-Host "For users - Installation commands:" -ForegroundColor Yellow
Write-Host "  Install-Package AztecQRGenerator.Core -Version 1.4.0" -ForegroundColor Green
Write-Host "  dotnet add package AztecQRGenerator.Core --version 1.4.0" -ForegroundColor Green
Write-Host ""

Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "  1. Monitor NuGet.org for package availability (10-15 mins)" -ForegroundColor White
Write-Host "  2. Complete GitHub release creation" -ForegroundColor White
Write-Host "  3. Update documentation if needed" -ForegroundColor White
Write-Host "  4. Announce the release on relevant channels" -ForegroundColor White
Write-Host ""

Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
