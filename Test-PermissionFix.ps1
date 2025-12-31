# Test Script for Permission Fix
# This script tests the new safe location functionality

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Testing Permission Fix" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$appPath = "C:\Jobb\AztecQRGenerator\bin\Debug\AztecQRGenerator.exe"

# Test 1: Check if application runs without admin
Write-Host "[Test 1] Running application without admin privileges..." -ForegroundColor Yellow

# Generate a test QR code
$testData = "SGVsbG8gV29ybGQh"  # "Hello World!" in Base64

Write-Host "  Generating test QR code..." -ForegroundColor Gray
& $appPath QR $testData "test_output.png" 300 2

if ($LASTEXITCODE -eq 0) {
    Write-Host "  ? Application ran successfully!" -ForegroundColor Green
} else {
    Write-Host "  ? Application failed with exit code: $LASTEXITCODE" -ForegroundColor Red
}

Write-Host ""

# Test 2: Check log file location
Write-Host "[Test 2] Checking log file location..." -ForegroundColor Yellow

$possibleLogLocations = @(
    "$env:LocalAppData\AztecQRGenerator\Logs",
    "$env:UserProfile\Documents\AztecQRGenerator\Logs",
    "$env:Temp\AztecQRGenerator\Logs",
    $env:Temp
)

$logFound = $false
foreach ($location in $possibleLogLocations) {
    if (Test-Path $location) {
        $logFiles = Get-ChildItem -Path $location -Filter "AztecQR_*.log" -ErrorAction SilentlyContinue
        if ($logFiles) {
            Write-Host "  ? Log files found at: $location" -ForegroundColor Green
            foreach ($log in $logFiles) {
                Write-Host "    - $($log.Name) ($([math]::Round($log.Length/1KB, 2)) KB, Last modified: $($log.LastWriteTime))" -ForegroundColor Gray
            }
            $logFound = $true
            break
        }
    }
}

if (-not $logFound) {
    Write-Host "  ? No log files found in expected locations" -ForegroundColor Yellow
}

Write-Host ""

# Test 3: Check output file location
Write-Host "[Test 3] Checking output file location..." -ForegroundColor Yellow

$possibleOutputLocations = @(
    "$env:UserProfile\Documents\AztecQRGenerator\Output",
    "C:\Jobb\AztecQRGenerator\bin\Debug"
)

$outputFound = $false
foreach ($location in $possibleOutputLocations) {
    if (Test-Path $location) {
        $outputFiles = Get-ChildItem -Path $location -Filter "*.png" -ErrorAction SilentlyContinue | 
                       Where-Object { $_.LastWriteTime -gt (Get-Date).AddMinutes(-5) }
        if ($outputFiles) {
            Write-Host "  ? Output files found at: $location" -ForegroundColor Green
            foreach ($file in $outputFiles) {
                Write-Host "    - $($file.Name) ($([math]::Round($file.Length/1KB, 2)) KB)" -ForegroundColor Gray
            }
            $outputFound = $true
            break
        }
    }
}

if (-not $outputFound) {
    Write-Host "  ? No recent output files found" -ForegroundColor Yellow
}

Write-Host ""

# Test 4: Test permission scenario (try to write to protected location)
Write-Host "[Test 4] Testing protected location handling..." -ForegroundColor Yellow
Write-Host "  (This should fail gracefully and use fallback location)" -ForegroundColor Gray

# Try to save to Windows directory (should be denied)
& $appPath QR $testData "C:\Windows\test_protected.png" 300 2

if ($LASTEXITCODE -ne 0) {
    Write-Host "  ? Correctly handled protected location" -ForegroundColor Green
    
    # Check if file was saved to fallback location
    $fallbackLocation = "$env:UserProfile\Documents\AztecQRGenerator\Output"
    if (Test-Path $fallbackLocation) {
        $recentFiles = Get-ChildItem -Path $fallbackLocation -Filter "*.png" -ErrorAction SilentlyContinue |
                       Where-Object { $_.LastWriteTime -gt (Get-Date).AddMinutes(-2) }
        if ($recentFiles) {
            Write-Host "  ? File saved to fallback location: $fallbackLocation" -ForegroundColor Green
        }
    }
} else {
    Write-Host "  ? Unexpected: Was able to write to protected location" -ForegroundColor Yellow
}

Write-Host ""

# Summary
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Test Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Safe Locations:" -ForegroundColor Yellow
Write-Host "  Logs:    $env:LocalAppData\AztecQRGenerator\Logs" -ForegroundColor White
Write-Host "  Output:  $env:UserProfile\Documents\AztecQRGenerator\Output" -ForegroundColor White
Write-Host ""
Write-Host "To open these locations:" -ForegroundColor Yellow
Write-Host "  Win+R -> %LocalAppData%\AztecQRGenerator\Logs" -ForegroundColor Cyan
Write-Host "  Win+R -> %UserProfile%\Documents\AztecQRGenerator\Output" -ForegroundColor Cyan
Write-Host ""

# Cleanup option
Write-Host "========================================" -ForegroundColor Cyan
$cleanup = Read-Host "Delete test files? (Y/N)"
if ($cleanup -eq "Y" -or $cleanup -eq "y") {
    Write-Host "Cleaning up test files..." -ForegroundColor Gray
    
    # Remove test output files
    $documentsOutput = "$env:UserProfile\Documents\AztecQRGenerator\Output"
    if (Test-Path $documentsOutput) {
        Remove-Item "$documentsOutput\*test*.png" -ErrorAction SilentlyContinue
        Remove-Item "$documentsOutput\QRCode_*.png" -ErrorAction SilentlyContinue
    }
    
    # Remove from debug directory
    Remove-Item "C:\Jobb\AztecQRGenerator\bin\Debug\test*.png" -ErrorAction SilentlyContinue
    Remove-Item "C:\Jobb\AztecQRGenerator\bin\Debug\QRCode_*.png" -ErrorAction SilentlyContinue
    
    Write-Host "? Cleanup complete" -ForegroundColor Green
}

Write-Host ""
Write-Host "Testing complete!" -ForegroundColor Green
