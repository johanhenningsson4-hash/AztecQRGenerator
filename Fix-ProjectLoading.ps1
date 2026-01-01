# Complete Fix for Visual Studio Project Loading

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Comprehensive VS Project Fix" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$projectPath = "C:\Jobb\AztecQRGenerator"
Set-Location $projectPath

# Step 1: Close all Visual Studio instances
Write-Host "[1/4] Closing Visual Studio..." -ForegroundColor Yellow
$vsProcesses = Get-Process -Name "devenv" -ErrorAction SilentlyContinue
if ($vsProcesses) {
    Write-Host "  Found $($vsProcesses.Count) VS instance(s)" -ForegroundColor Gray
    $closeVS = Read-Host "  Close all Visual Studio instances? (Y/N)"
    if ($closeVS -eq "Y" -or $closeVS -eq "y") {
        $vsProcesses | Stop-Process -Force
        Start-Sleep -Seconds 2
        Write-Host "  ? Visual Studio closed" -ForegroundColor Green
    } else {
        Write-Host "  ? Please close Visual Studio manually" -ForegroundColor Yellow
        Write-Host "  Press Enter after closing..."
        Read-Host
    }
} else {
    Write-Host "  ? No VS instances running" -ForegroundColor Green
}

# Step 2: Clean all cache and build artifacts
Write-Host ""
Write-Host "[2/4] Cleaning cache and build artifacts..." -ForegroundColor Yellow

$itemsToClean = @(
    ".vs",
    "bin",
    "obj",
    "*.suo",
    "*.user",
    ".vs\*\*",
    "obj\*\*"
)

$cleanedCount = 0
foreach ($item in $itemsToClean) {
    if (Test-Path $item) {
        Remove-Item -Path $item -Recurse -Force -ErrorAction SilentlyContinue
        $cleanedCount++
    }
}

Write-Host "  ? Cleaned $cleanedCount items" -ForegroundColor Green

# Step 3: Restore NuGet packages using MSBuild
Write-Host ""
Write-Host "[3/4] Restoring NuGet packages..." -ForegroundColor Yellow
try {
    msbuild /t:Restore AztecQRGenerator.sln /nologo /verbosity:quiet 2>&1 | Out-Null
    Write-Host "  ? Packages restored" -ForegroundColor Green
} catch {
    Write-Host "  ? Package restore failed (may not be needed)" -ForegroundColor Yellow
}

# Step 4: Verify project structure
Write-Host ""
Write-Host "[4/4] Verifying project structure..." -ForegroundColor Yellow

$checks = @(
    @{ File = "AztecQRGenerator.sln"; Name = "Solution file" },
    @{ File = "AztecQRGenerator.csproj"; Name = "Project file" },
    @{ File = "Program.cs"; Name = "Program.cs" },
    @{ File = "packages.config"; Name = "Packages config" }
)

$allGood = $true
foreach ($check in $checks) {
    if (Test-Path $check.File) {
        Write-Host "  ? $($check.Name)" -ForegroundColor Green
    } else {
        Write-Host "  ? $($check.Name) missing" -ForegroundColor Red
        $allGood = $false
    }
}

# Final result
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan

if ($allGood) {
    Write-Host "? FIX COMPLETE!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Now open Visual Studio using ONE of these methods:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Method 1 - Double-click (Recommended):" -ForegroundColor White
    Write-Host "  1. Navigate to: C:\Jobb\AztecQRGenerator" -ForegroundColor Gray
    Write-Host "  2. Double-click: AztecQRGenerator.sln" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Method 2 - From this script:" -ForegroundColor White
    
    $openVS = Read-Host "Open Visual Studio now? (Y/N)"
    if ($openVS -eq "Y" -or $openVS -eq "y") {
        Write-Host "  Opening Visual Studio..." -ForegroundColor Gray
        Start-Process "$projectPath\AztecQRGenerator.sln"
        Write-Host "  ? Visual Studio launched" -ForegroundColor Green
    }
    
    Write-Host ""
    Write-Host "Method 3 - As Administrator:" -ForegroundColor White
    Write-Host "  Right-click AztecQRGenerator.sln ? Run as Administrator" -ForegroundColor Gray
    
} else {
    Write-Host "? ISSUES FOUND" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Some files are missing. Please check the project structure." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Additional Help" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "If Visual Studio still won't load the project:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Check VS Output window:" -ForegroundColor White
Write-Host "   View ? Output ? Select 'Show output from: Build'" -ForegroundColor Gray
Write-Host ""
Write-Host "2. Try loading just the project (not solution):" -ForegroundColor White
Write-Host "   File ? Open ? Project/Solution ? Select .csproj" -ForegroundColor Gray
Write-Host ""
Write-Host "3. Repair Visual Studio:" -ForegroundColor White
Write-Host "   Settings ? Apps ? Visual Studio ? Modify ? Repair" -ForegroundColor Gray
Write-Host ""
Write-Host "4. Check for Visual Studio updates:" -ForegroundColor White
Write-Host "   Help ? Check for Updates" -ForegroundColor Gray
Write-Host ""

# Create a batch file for easy reopening
Write-Host "Creating quick-open batch file..." -ForegroundColor Gray
$batchContent = @"
@echo off
cd /d "C:\Jobb\AztecQRGenerator"
start AztecQRGenerator.sln
"@
$batchContent | Out-File "Open-Project.bat" -Encoding ASCII
Write-Host "? Created Open-Project.bat (double-click to open VS)" -ForegroundColor Green

Write-Host ""
