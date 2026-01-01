# Diagnose Visual Studio Project Loading Issue
# This script checks everything that could prevent VS from loading the project

param(
    [switch]$AutoFix = $false
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "VS Project Loading Diagnostics" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$projectPath = "C:\Jobb\AztecQRGenerator"
$hasIssues = $false

# Test 1: Check if files exist
Write-Host "[1] Checking project files..." -ForegroundColor Yellow
$sln = Join-Path $projectPath "AztecQRGenerator.sln"
$csproj = Join-Path $projectPath "AztecQRGenerator.csproj"

if (Test-Path $sln) {
    Write-Host "  ? Solution file exists" -ForegroundColor Green
} else {
    Write-Host "  ? Solution file missing!" -ForegroundColor Red
    $hasIssues = $true
}

if (Test-Path $csproj) {
    Write-Host "  ? Project file exists" -ForegroundColor Green
} else {
    Write-Host "  ? Project file missing!" -ForegroundColor Red
    $hasIssues = $true
}

Write-Host ""

# Test 2: Check file encoding
Write-Host "[2] Checking file encoding..." -ForegroundColor Yellow
try {
    $slnContent = Get-Content $sln -Raw -ErrorAction Stop
    $csprojContent = Get-Content $csproj -Raw -ErrorAction Stop
    Write-Host "  ? Files are readable" -ForegroundColor Green
} catch {
    Write-Host "  ? Cannot read files: $_" -ForegroundColor Red
    $hasIssues = $true
}

Write-Host ""

# Test 3: Check for BOM or special characters
Write-Host "[3] Checking for file issues..." -ForegroundColor Yellow
$slnBytes = [System.IO.File]::ReadAllBytes($sln)
if ($slnBytes[0] -eq 0xEF -and $slnBytes[1] -eq 0xBB -and $slnBytes[2] -eq 0xBF) {
    Write-Host "  ? Solution has UTF-8 BOM (this is OK)" -ForegroundColor Gray
}

# Check for null bytes or other issues
if ($slnContent -match "`0") {
    Write-Host "  ? Solution file contains null bytes!" -ForegroundColor Red
    $hasIssues = $true
} else {
    Write-Host "  ? No null bytes in solution" -ForegroundColor Green
}

Write-Host ""

# Test 4: Check .vs folder
Write-Host "[4] Checking Visual Studio cache..." -ForegroundColor Yellow
$vsFolder = Join-Path $projectPath ".vs"
if (Test-Path $vsFolder) {
    $size = (Get-ChildItem $vsFolder -Recurse -File -ErrorAction SilentlyContinue | Measure-Object -Property Length -Sum).Sum
    $sizeMB = [math]::Round($size / 1MB, 2)
    Write-Host "  ? .vs folder exists ($sizeMB MB)" -ForegroundColor Yellow
    
    if ($AutoFix) {
        Write-Host "  ? Deleting .vs folder..." -ForegroundColor Gray
        Remove-Item $vsFolder -Recurse -Force -ErrorAction SilentlyContinue
        Write-Host "  ? Deleted" -ForegroundColor Green
    } else {
        Write-Host "  ?? Run with -AutoFix to clean this" -ForegroundColor Cyan
    }
} else {
    Write-Host "  ? No .vs folder (clean state)" -ForegroundColor Green
}

Write-Host ""

# Test 5: Check for lock files
Write-Host "[5] Checking for lock files..." -ForegroundColor Yellow
$lockFiles = @(
    "*.suo",
    "*.user",
    ".vs\*\*.suo",
    "*.lock.json"
)

$found = $false
foreach ($pattern in $lockFiles) {
    $files = Get-ChildItem (Join-Path $projectPath $pattern) -Recurse -ErrorAction SilentlyContinue
    if ($files) {
        Write-Host "  ? Found: $($files.Count) $pattern files" -ForegroundColor Yellow
        $found = $true
        
        if ($AutoFix) {
            $files | Remove-Item -Force -ErrorAction SilentlyContinue
            Write-Host "  ? Deleted" -ForegroundColor Green
        }
    }
}

if (-not $found) {
    Write-Host "  ? No lock files found" -ForegroundColor Green
}

Write-Host ""

# Test 6: Check NuGet packages
Write-Host "[6] Checking NuGet packages..." -ForegroundColor Yellow
$packagesFolder = Join-Path $projectPath "packages"
if (Test-Path $packagesFolder) {
    $packageCount = (Get-ChildItem $packagesFolder -Directory).Count
    Write-Host "  ? Packages folder exists ($packageCount packages)" -ForegroundColor Green
} else {
    Write-Host "  ? Packages folder missing" -ForegroundColor Yellow
    Write-Host "  ?? Run: nuget restore AztecQRGenerator.sln" -ForegroundColor Cyan
    $hasIssues = $true
}

Write-Host ""

# Test 7: Verify build works
Write-Host "[7] Testing MSBuild..." -ForegroundColor Yellow
try {
    $buildOutput = & msbuild "$sln" /t:Clean /nologo /v:q 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ? MSBuild can read the solution" -ForegroundColor Green
    } else {
        Write-Host "  ? MSBuild failed: $buildOutput" -ForegroundColor Red
        $hasIssues = $true
    }
} catch {
    Write-Host "  ? MSBuild error: $_" -ForegroundColor Red
    $hasIssues = $true
}

Write-Host ""

# Test 8: Check Visual Studio processes
Write-Host "[8] Checking Visual Studio processes..." -ForegroundColor Yellow
$vsProcesses = Get-Process devenv -ErrorAction SilentlyContinue
if ($vsProcesses) {
    Write-Host "  ? Visual Studio is running ($($vsProcesses.Count) instances)" -ForegroundColor Yellow
    Write-Host "  ?? Close Visual Studio and try reopening" -ForegroundColor Cyan
} else {
    Write-Host "  ? No Visual Studio processes running" -ForegroundColor Green
}

Write-Host ""

# Test 9: Check .NET Framework
Write-Host "[9] Checking .NET Framework..." -ForegroundColor Yellow
$netFrameworkPath = "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2"
if (Test-Path $netFrameworkPath) {
    Write-Host "  ? .NET Framework 4.7.2 installed" -ForegroundColor Green
} else {
    Write-Host "  ? .NET Framework 4.7.2 not found" -ForegroundColor Red
    Write-Host "  ?? Download: https://dotnet.microsoft.com/download/dotnet-framework/net472" -ForegroundColor Cyan
    $hasIssues = $true
}

Write-Host ""

# Test 10: Check project GUID
Write-Host "[10] Checking project GUID..." -ForegroundColor Yellow
if ($slnContent -match '\{36C17DE2-A271-47FC-989A-CA2165BF3639\}') {
    Write-Host "  ? Project GUID found in solution" -ForegroundColor Green
    
    if ($csprojContent -match '<ProjectGuid>\{36C17DE2-A271-47FC-989A-CA2165BF3639\}</ProjectGuid>') {
        Write-Host "  ? Project GUID matches" -ForegroundColor Green
    } else {
        Write-Host "  ? Project GUID mismatch or missing in .csproj" -ForegroundColor Yellow
        Write-Host "  ? This might be OK for newer project formats" -ForegroundColor Gray
    }
} else {
    Write-Host "  ? Project GUID not found in solution!" -ForegroundColor Red
    $hasIssues = $true
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan

if (-not $hasIssues) {
    Write-Host "? NO ISSUES FOUND!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Project files appear to be fine." -ForegroundColor White
    Write-Host ""
    Write-Host "Try these steps:" -ForegroundColor Yellow
    Write-Host "1. Close ALL Visual Studio instances" -ForegroundColor White
    Write-Host "2. Run this script with -AutoFix flag" -ForegroundColor White
    Write-Host "3. Open Visual Studio as Administrator" -ForegroundColor White
    Write-Host "4. File ? Open ? Project/Solution" -ForegroundColor White
    Write-Host "5. Navigate to: $sln" -ForegroundColor White
    Write-Host ""
    Write-Host "If it still doesn't load, check:" -ForegroundColor Yellow
    Write-Host "- View ? Output ? Show output from: 'Build'" -ForegroundColor White
    Write-Host "- Look for specific error messages" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host "? ISSUES FOUND" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Run this command to attempt automatic fixes:" -ForegroundColor Yellow
    Write-Host ".\Diagnose-VSProject.ps1 -AutoFix" -ForegroundColor Cyan
    Write-Host ""
}

# Summary
Write-Host "Quick Fix Commands:" -ForegroundColor Cyan
Write-Host "? Clean cache:       Remove-Item .vs -Recurse -Force" -ForegroundColor Gray
Write-Host "? Restore packages:  nuget restore AztecQRGenerator.sln" -ForegroundColor Gray
Write-Host "? Clean build:       msbuild AztecQRGenerator.sln /t:Clean" -ForegroundColor Gray
Write-Host ""

Write-Host "Alternative: Open project file directly" -ForegroundColor Cyan
Write-Host "File ? Open ? Project/Solution ? AztecQRGenerator.csproj" -ForegroundColor Gray
Write-Host ""
