# Package Verification and Publishing Script
# AztecQRGenerator.Core v1.2.1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "NuGet Package Verification & Publishing" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Set working directory
$packageDir = "C:\Jobb\AztecQRGenerator\AztecQRGenerator.Core"
Set-Location $packageDir

# 1. Verify Package Exists
Write-Host "[1/6] Verifying package file..." -ForegroundColor Yellow
$packageFile = "AztecQRGenerator.Core.1.2.1.nupkg"

if (Test-Path $packageFile) {
    $fileInfo = Get-Item $packageFile
    Write-Host "  ? Package found: $packageFile" -ForegroundColor Green
    Write-Host "    Size: $($fileInfo.Length) bytes" -ForegroundColor Gray
    Write-Host "    Created: $($fileInfo.LastWriteTime)" -ForegroundColor Gray
} else {
    Write-Host "  ? Package not found!" -ForegroundColor Red
    Write-Host "    Run: .\nuget.exe pack AztecQRGenerator.Core.csproj -Properties Configuration=Release" -ForegroundColor Yellow
    exit
}

Write-Host ""

# 2. Test Package Locally (Optional)
Write-Host "[2/6] Local Package Testing (Optional)" -ForegroundColor Yellow
Write-Host "  You can test the package locally before publishing:" -ForegroundColor Gray
Write-Host "    cd C:\Temp" -ForegroundColor Cyan
Write-Host "    dotnet new console -n TestPackage" -ForegroundColor Cyan
Write-Host "    cd TestPackage" -ForegroundColor Cyan
Write-Host "    dotnet add package AztecQRGenerator.Core --version 1.2.1 --source $packageDir" -ForegroundColor Cyan
Write-Host ""
$testLocal = Read-Host "  Do you want to skip to publishing? (Y/N)"

if ($testLocal -eq "N" -or $testLocal -eq "n") {
    Write-Host "  Skipped - Test the package first, then run this script again." -ForegroundColor Yellow
    exit
}

Write-Host ""

# 3. Get NuGet API Key
Write-Host "[3/6] NuGet.org API Key" -ForegroundColor Yellow
Write-Host "  You need an API key from https://www.nuget.org/account/apikeys" -ForegroundColor Gray
Write-Host ""
$apiKey = Read-Host "  Enter your NuGet.org API Key (or press Enter to skip)"

if ([string]::IsNullOrWhiteSpace($apiKey)) {
    Write-Host "  ? No API key provided. Skipping publish." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "To publish manually, run:" -ForegroundColor Gray
    Write-Host "  dotnet nuget push `"$packageFile`" --api-key YOUR_KEY --source https://api.nuget.org/v3/index.json" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Or:" -ForegroundColor Gray
    Write-Host "  .\nuget.exe push `"$packageFile`" -ApiKey YOUR_KEY -Source https://api.nuget.org/v3/index.json" -ForegroundColor Cyan
    
    # Skip to git steps
    $skipGit = Read-Host "`nContinue to Git tagging? (Y/N)"
    if ($skipGit -eq "N" -or $skipGit -eq "n") {
        exit
    }
} else {
    Write-Host ""
    
    # 4. Push to NuGet.org
    Write-Host "[4/6] Publishing to NuGet.org..." -ForegroundColor Yellow
    Write-Host "  This will publish the package to the public NuGet repository." -ForegroundColor Gray
    Write-Host ""
    $confirm = Read-Host "  Are you sure you want to publish? (YES/no)"
    
    if ($confirm -eq "YES") {
        Write-Host "  Publishing..." -ForegroundColor Cyan
        
        try {
            & dotnet nuget push $packageFile --api-key $apiKey --source https://api.nuget.org/v3/index.json
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "  ? Package published successfully!" -ForegroundColor Green
                Write-Host "  ? Wait 10-15 minutes for NuGet.org indexing" -ForegroundColor Yellow
                Write-Host "  ?? Package URL: https://www.nuget.org/packages/AztecQRGenerator.Core/1.2.1" -ForegroundColor Gray
            } else {
                Write-Host "  ? Publishing failed!" -ForegroundColor Red
                exit
            }
        } catch {
            Write-Host "  ? Error: $_" -ForegroundColor Red
            exit
        }
    } else {
        Write-Host "  ? Publishing cancelled." -ForegroundColor Yellow
        exit
    }
}

Write-Host ""

# 5. Git Tagging
Write-Host "[5/6] Git Tagging" -ForegroundColor Yellow
Write-Host "  Creating git tag v1.2.1..." -ForegroundColor Gray

$gitDir = "C:\Jobb\AztecQRGenerator"
Set-Location $gitDir

try {
    # Check if tag already exists
    $tagExists = git tag -l "v1.2.1"
    
    if ($tagExists) {
        Write-Host "  ? Tag v1.2.1 already exists" -ForegroundColor Yellow
        $deleteTag = Read-Host "    Delete and recreate? (Y/N)"
        
        if ($deleteTag -eq "Y" -or $deleteTag -eq "y") {
            git tag -d v1.2.1
            git push origin :refs/tags/v1.2.1
            Write-Host "    Old tag deleted" -ForegroundColor Gray
        }
    }
    
    # Create tag
    git add .
    git commit -m "Release v1.2.1 - Remove unused parameters"
    git tag -a v1.2.1 -m "Release version 1.2.1 - Code cleanup and parameter removal"
    
    Write-Host "  ? Tag v1.2.1 created locally" -ForegroundColor Green
    
    # Push
    Write-Host "  Pushing to GitHub..." -ForegroundColor Cyan
    git push origin NUGET_PUBLISHING
    git push origin v1.2.1
    
    Write-Host "  ? Pushed to GitHub" -ForegroundColor Green
    
} catch {
    Write-Host "  ? Git operations: $_" -ForegroundColor Yellow
}

Write-Host ""

# 6. Create GitHub Release
Write-Host "[6/6] Create GitHub Release" -ForegroundColor Yellow
Write-Host "  Manual steps required:" -ForegroundColor Gray
Write-Host "    1. Go to: https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new" -ForegroundColor Cyan
Write-Host "    2. Select tag: v1.2.1" -ForegroundColor Cyan
Write-Host "    3. Title: v1.2.1 - Code Cleanup" -ForegroundColor Cyan
Write-Host "    4. Description: Copy from CHANGELOG.md" -ForegroundColor Cyan
Write-Host "    5. Attach file: AztecQRGenerator.Core.1.2.1.nupkg" -ForegroundColor Cyan
Write-Host "    6. Click 'Publish release'" -ForegroundColor Cyan
Write-Host ""

$openBrowser = Read-Host "  Open GitHub releases page in browser? (Y/N)"
if ($openBrowser -eq "Y" -or $openBrowser -eq "y") {
    Start-Process "https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v1.2.1"
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "          PUBLISHING COMPLETE!          " -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Summary:" -ForegroundColor Yellow
Write-Host "  ? Package: AztecQRGenerator.Core.1.2.1.nupkg" -ForegroundColor Green
Write-Host "  ? Published to: NuGet.org (if API key provided)" -ForegroundColor Green
Write-Host "  ? Git tagged: v1.2.1" -ForegroundColor Green
Write-Host "  ? Complete GitHub release manually" -ForegroundColor Yellow
Write-Host ""
Write-Host "Links:" -ForegroundColor Yellow
Write-Host "  NuGet: https://www.nuget.org/packages/AztecQRGenerator.Core/1.2.1" -ForegroundColor Cyan
Write-Host "  GitHub: https://github.com/johanhenningsson4-hash/AztecQRGenerator" -ForegroundColor Cyan
Write-Host "  Releases: https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases" -ForegroundColor Cyan
Write-Host ""
Write-Host "Installation command for users:" -ForegroundColor Yellow
Write-Host "  Install-Package AztecQRGenerator.Core -Version 1.2.1" -ForegroundColor Cyan
Write-Host "  dotnet add package AztecQRGenerator.Core --version 1.2.1" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
