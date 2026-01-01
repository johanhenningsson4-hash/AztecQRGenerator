# Diagnose Visual Studio Project Loading Issue

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Visual Studio Project Loading Diagnostic" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$projectPath = "C:\Jobb\AztecQRGenerator"
Set-Location $projectPath

# Check 1: Project files exist
Write-Host "[1/8] Checking project files..." -ForegroundColor Yellow
$sln = Test-Path "AztecQRGenerator.sln"
$csproj = Test-Path "AztecQRGenerator.csproj"

if ($sln) {
    Write-Host "  ? Solution file exists" -ForegroundColor Green
} else {
    Write-Host "  ? Solution file missing" -ForegroundColor Red
}

if ($csproj) {
    Write-Host "  ? Project file exists" -ForegroundColor Green
} else {
    Write-Host "  ? Project file missing" -ForegroundColor Red
}

# Check 2: XML validity
Write-Host ""
Write-Host "[2/8] Validating XML..." -ForegroundColor Yellow
try {
    $xml = [xml](Get-Content "AztecQRGenerator.csproj")
    Write-Host "  ? Project XML is valid" -ForegroundColor Green
} catch {
    Write-Host "  ? XML validation failed: $_" -ForegroundColor Red
}

# Check 3: Target framework
Write-Host ""
Write-Host "[3/8] Checking .NET Framework..." -ForegroundColor Yellow
$targetFramework = $xml.Project.PropertyGroup.TargetFrameworkVersion | Select-Object -First 1
Write-Host "  Target: $targetFramework" -ForegroundColor White

$frameworks = Get-ChildItem "HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" -ErrorAction SilentlyContinue
if ($frameworks) {
    Write-Host "  ? .NET Framework 4.x installed" -ForegroundColor Green
} else {
    Write-Host "  ? Cannot detect .NET Framework installation" -ForegroundColor Yellow
}

# Check 4: NuGet packages
Write-Host ""
Write-Host "[4/8] Checking NuGet packages..." -ForegroundColor Yellow
if (Test-Path "packages.config") {
    Write-Host "  ? packages.config exists" -ForegroundColor Green
    $packagesRestored = Test-Path "packages"
    if ($packagesRestored) {
        $packageCount = (Get-ChildItem "packages" -Directory).Count
        Write-Host "  ? Packages folder exists ($packageCount packages)" -ForegroundColor Green
    } else {
        Write-Host "  ? Packages folder missing - restore needed" -ForegroundColor Yellow
    }
} else {
    Write-Host "  ? No packages.config (using PackageReference?)" -ForegroundColor Gray
}

# Check 5: VS cache
Write-Host ""
Write-Host "[5/8] Checking Visual Studio cache..." -ForegroundColor Yellow
if (Test-Path ".vs") {
    Write-Host "  ? .vs folder exists (may need cleaning)" -ForegroundColor Yellow
} else {
    Write-Host "  ? No .vs cache" -ForegroundColor Green
}

# Check 6: Build folders
Write-Host ""
Write-Host "[6/8] Checking build output..." -ForegroundColor Yellow
$hasDebug = Test-Path "bin\Debug\AztecQRGenerator.exe"
$hasRelease = Test-Path "bin\Release\AztecQRGenerator.exe"

if ($hasDebug) {
    Write-Host "  ? Debug build exists" -ForegroundColor Green
} else {
    Write-Host "  ? No Debug build" -ForegroundColor Gray
}

if ($hasRelease) {
    Write-Host "  ? Release build exists" -ForegroundColor Green
} else {
    Write-Host "  ? No Release build" -ForegroundColor Gray
}

# Check 7: File references
Write-Host ""
Write-Host "[7/8] Checking file references..." -ForegroundColor Yellow
$csFiles = $xml.Project.ItemGroup.Compile | Where-Object { $_.Include }
$missingFiles = @()
foreach ($file in $csFiles) {
    if ($file.Include -and !(Test-Path $file.Include)) {
        $missingFiles += $file.Include
    }
}

if ($missingFiles.Count -eq 0) {
    Write-Host "  ? All source files exist" -ForegroundColor Green
} else {
    Write-Host "  ? Missing files:" -ForegroundColor Yellow
    foreach ($file in $missingFiles) {
        Write-Host "    - $file" -ForegroundColor Gray
    }
}

# Check 8: VS process
Write-Host ""
Write-Host "[8/8] Checking Visual Studio..." -ForegroundColor Yellow
$vsProcesses = Get-Process -Name "devenv" -ErrorAction SilentlyContinue
if ($vsProcesses) {
    Write-Host "  ? Visual Studio is running ($($vsProcesses.Count) instance(s))" -ForegroundColor Gray
    Write-Host "    Recommendation: Close and reopen VS" -ForegroundColor Yellow
} else {
    Write-Host "  ? Visual Studio not currently running" -ForegroundColor Gray
}

# Summary and recommendations
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Recommendations" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Try these solutions in order:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Restore NuGet packages:" -ForegroundColor White
Write-Host "   nuget restore AztecQRGenerator.sln" -ForegroundColor Cyan
Write-Host ""
Write-Host "2. Clean VS cache:" -ForegroundColor White
Write-Host "   Remove-Item -Recurse -Force .vs" -ForegroundColor Cyan
Write-Host "   Remove-Item -Recurse -Force bin, obj" -ForegroundColor Cyan
Write-Host ""
Write-Host "3. Close all VS instances and reopen:" -ForegroundColor White
Write-Host "   Get-Process devenv | Stop-Process" -ForegroundColor Cyan
Write-Host "   Start-Process 'AztecQRGenerator.sln'" -ForegroundColor Cyan
Write-Host ""
Write-Host "4. Open as Administrator:" -ForegroundColor White
Write-Host "   Right-click AztecQRGenerator.sln ? Run as Administrator" -ForegroundColor Cyan
Write-Host ""
Write-Host "5. Try loading project only (not solution):" -ForegroundColor White
Write-Host "   File ? Open ? Project ? AztecQRGenerator.csproj" -ForegroundColor Cyan
Write-Host ""

# Quick fix option
Write-Host "========================================" -ForegroundColor Cyan
$quickFix = Read-Host "Run quick fix (restore packages + clean cache)? (Y/N)"
if ($quickFix -eq "Y" -or $quickFix -eq "y") {
    Write-Host ""
    Write-Host "Running quick fix..." -ForegroundColor Yellow
    
    # Restore packages
    Write-Host "  Restoring NuGet packages..." -ForegroundColor Gray
    nuget restore AztecQRGenerator.sln 2>&1 | Out-Null
    
    # Clean cache
    Write-Host "  Cleaning VS cache..." -ForegroundColor Gray
    Remove-Item -Recurse -Force .vs -ErrorAction SilentlyContinue
    
    # Clean build folders
    Write-Host "  Cleaning build folders..." -ForegroundColor Gray
    Remove-Item -Recurse -Force bin -ErrorAction SilentlyContinue
    Remove-Item -Recurse -Force obj -ErrorAction SilentlyContinue
    
    Write-Host ""
    Write-Host "? Quick fix complete!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Now try opening Visual Studio again." -ForegroundColor Yellow
}

Write-Host ""
