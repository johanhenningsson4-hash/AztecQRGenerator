# CI/CD Setup Verification Script
# Checks all CI/CD components are properly configured

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "CI/CD Setup Verification" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Set-Location "C:\Jobb\AztecQRGenerator"

$allGood = $true

# Check 1: GitHub Actions workflows
Write-Host "[1/8] Checking GitHub Actions workflows..." -ForegroundColor Yellow

$workflows = @(
    ".github\workflows\ci.yml",
    ".github\workflows\nuget-publish.yml",
    ".github\workflows\code-quality.yml"
)

foreach ($workflow in $workflows) {
    if (Test-Path $workflow) {
        Write-Host "  ? Found: $workflow" -ForegroundColor Green
    } else {
        Write-Host "  ? Missing: $workflow" -ForegroundColor Red
        $allGood = $false
    }
}
Write-Host ""

# Check 2: Solution file
Write-Host "[2/8] Checking solution file..." -ForegroundColor Yellow
if (Test-Path "AztecQRGenerator.sln") {
    Write-Host "  ? AztecQRGenerator.sln found" -ForegroundColor Green
} else {
    Write-Host "  ? AztecQRGenerator.sln not found" -ForegroundColor Red
    $allGood = $false
}
Write-Host ""

# Check 3: Test project
Write-Host "[3/8] Checking test project..." -ForegroundColor Yellow
if (Test-Path "AztecQRGenerator.Tests\AztecQRGenerator.Tests.csproj") {
    Write-Host "  ? Test project found" -ForegroundColor Green
    
    # Check test files
    $testFiles = @(
        "AztecQRGenerator.Tests\QRGeneratorTests.cs",
        "AztecQRGenerator.Tests\AztecGeneratorTests.cs",
        "AztecQRGenerator.Tests\LoggerTests.cs"
    )
    
    foreach ($testFile in $testFiles) {
        if (Test-Path $testFile) {
            Write-Host "  ? Found: $testFile" -ForegroundColor Green
        } else {
            Write-Host "  ?? Missing: $testFile" -ForegroundColor Yellow
        }
    }
} else {
    Write-Host "  ? Test project not found" -ForegroundColor Red
    $allGood = $false
}
Write-Host ""

# Check 4: Core project
Write-Host "[4/8] Checking Core project..." -ForegroundColor Yellow
if (Test-Path "AztecQRGenerator.Core\AztecQRGenerator.Core.csproj") {
    Write-Host "  ? Core project found" -ForegroundColor Green
    
    if (Test-Path "AztecQRGenerator.Core\AztecQRGenerator.Core.nuspec") {
        Write-Host "  ? NuGet spec file found" -ForegroundColor Green
    } else {
        Write-Host "  ?? NuGet spec file not found" -ForegroundColor Yellow
    }
} else {
    Write-Host "  ? Core project not found" -ForegroundColor Red
    $allGood = $false
}
Write-Host ""

# Check 5: Git repository
Write-Host "[5/8] Checking Git repository..." -ForegroundColor Yellow
try {
    $remote = git remote -v | Select-String "origin"
    if ($remote) {
        Write-Host "  ? Git repository configured" -ForegroundColor Green
        Write-Host "     $($remote[0])" -ForegroundColor Gray
    }
    
    $branch = git branch --show-current
    Write-Host "  ? Current branch: $branch" -ForegroundColor Green
    
    $status = git status --short
    if ($status) {
        Write-Host "  ?? Uncommitted changes found" -ForegroundColor Yellow
    } else {
        Write-Host "  ? Working directory clean" -ForegroundColor Green
    }
} catch {
    Write-Host "  ? Git repository not properly configured" -ForegroundColor Red
    $allGood = $false
}
Write-Host ""

# Check 6: Documentation
Write-Host "[6/8] Checking CI/CD documentation..." -ForegroundColor Yellow
$docs = @(
    "TESTING_CICD_SETUP.md",
    "PUBLISH_TO_NUGET.md",
    "CICD_VERIFICATION_REPORT.md"
)

foreach ($doc in $docs) {
    if (Test-Path $doc) {
        Write-Host "  ? Found: $doc" -ForegroundColor Green
    } else {
        Write-Host "  ?? Missing: $doc" -ForegroundColor Yellow
    }
}
Write-Host ""

# Check 7: Build capability
Write-Host "[7/8] Checking build capability..." -ForegroundColor Yellow
$msbuild = Get-Command msbuild -ErrorAction SilentlyContinue
if ($msbuild) {
    Write-Host "  ? MSBuild available" -ForegroundColor Green
} else {
    Write-Host "  ?? MSBuild not in PATH (Visual Studio might still work)" -ForegroundColor Yellow
}

# Check for nuget
$nugetLocations = @(
    "nuget.exe",
    "$env:ProgramFiles\NuGet\nuget.exe"
)

$nugetFound = $false
foreach ($location in $nugetLocations) {
    if (Test-Path $location) {
        Write-Host "  ? NuGet.exe found at: $location" -ForegroundColor Green
        $nugetFound = $true
        break
    }
}

if (-not $nugetFound) {
    $nugetCmd = Get-Command nuget -ErrorAction SilentlyContinue
    if ($nugetCmd) {
        Write-Host "  ? NuGet available in PATH" -ForegroundColor Green
    } else {
        Write-Host "  ?? NuGet.exe not found (will be downloaded if needed)" -ForegroundColor Cyan
    }
}
Write-Host ""

# Check 8: GitHub connection
Write-Host "[8/8] Checking GitHub connection..." -ForegroundColor Yellow
try {
    $remoteCheck = git ls-remote --heads origin 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ? Can connect to GitHub" -ForegroundColor Green
    } else {
        Write-Host "  ?? Cannot connect to GitHub (check credentials)" -ForegroundColor Yellow
    }
} catch {
    Write-Host "  ?? Could not verify GitHub connection" -ForegroundColor Yellow
}
Write-Host ""

# Summary
Write-Host "========================================" -ForegroundColor Cyan
if ($allGood) {
    Write-Host "? CI/CD Setup: VERIFIED" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Your CI/CD pipeline is properly configured!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next Steps:" -ForegroundColor Yellow
    Write-Host "1. Verify GitHub Secret:" -ForegroundColor White
    Write-Host "   https://github.com/johanhenningsson4-hash/AztecQRGenerator/settings/secrets/actions" -ForegroundColor Cyan
    Write-Host "   Ensure NUGET_API_KEY is configured" -ForegroundColor Gray
    Write-Host ""
    Write-Host "2. Test CI Workflow:" -ForegroundColor White
    Write-Host "   Make a commit and push to main branch" -ForegroundColor Gray
    Write-Host "   Watch it run at: https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "3. Ready to Publish:" -ForegroundColor White
    Write-Host "   Run: .\Complete-NuGet-Publish.ps1" -ForegroundColor Cyan
    Write-Host ""
} else {
    Write-Host "?? CI/CD Setup: ISSUES FOUND" -ForegroundColor Yellow
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Some components are missing or misconfigured." -ForegroundColor Yellow
    Write-Host "Review the checks above and fix any issues." -ForegroundColor White
    Write-Host ""
    Write-Host "For help, see:" -ForegroundColor Yellow
    Write-Host "  CICD_VERIFICATION_REPORT.md" -ForegroundColor Cyan
    Write-Host "  TESTING_CICD_SETUP.md" -ForegroundColor Cyan
    Write-Host ""
}

# Quick links
Write-Host "?? Quick Links:" -ForegroundColor Cyan
Write-Host "  • Actions: https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions" -ForegroundColor White
Write-Host "  • Secrets: https://github.com/johanhenningsson4-hash/AztecQRGenerator/settings/secrets/actions" -ForegroundColor White
Write-Host "  • Releases: https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases" -ForegroundColor White
Write-Host ""

# Open verification report
$openReport = Read-Host "Open verification report? (Y/N)"
if ($openReport -eq "Y" -or $openReport -eq "y") {
    if (Test-Path "CICD_VERIFICATION_REPORT.md") {
        Start-Process "CICD_VERIFICATION_REPORT.md"
    }
}

Write-Host ""
Write-Host "Verification complete!" -ForegroundColor Green
Write-Host ""
