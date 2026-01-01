# Quick Fix for Visual Studio Project Loading
# Closes VS, cleans cache, and reopens project

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Quick Fix: VS Project Loading" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$projectPath = "C:\Jobb\AztecQRGenerator"
$solutionFile = Join-Path $projectPath "AztecQRGenerator.sln"

# Step 1: Close Visual Studio
Write-Host "[1/4] Closing Visual Studio..." -ForegroundColor Yellow
$vsProcesses = Get-Process devenv -ErrorAction SilentlyContinue
if ($vsProcesses) {
    Write-Host "  Found $($vsProcesses.Count) instance(s)" -ForegroundColor Gray
    Write-Host "  Closing gracefully..." -ForegroundColor Gray
    $vsProcesses | ForEach-Object { $_.CloseMainWindow() | Out-Null }
    Start-Sleep -Seconds 3
    
    # Force close if still running
    $vsProcesses = Get-Process devenv -ErrorAction SilentlyContinue
    if ($vsProcesses) {
        Write-Host "  Force closing..." -ForegroundColor Gray
        $vsProcesses | Stop-Process -Force
        Start-Sleep -Seconds 2
    }
    Write-Host "  ? Visual Studio closed" -ForegroundColor Green
} else {
    Write-Host "  ? Visual Studio not running" -ForegroundColor Green
}

Write-Host ""

# Step 2: Clean cache
Write-Host "[2/4] Cleaning Visual Studio cache..." -ForegroundColor Yellow
$vsFolder = Join-Path $projectPath ".vs"
if (Test-Path $vsFolder) {
    Remove-Item $vsFolder -Recurse -Force -ErrorAction SilentlyContinue
    Write-Host "  ? Deleted .vs folder" -ForegroundColor Green
} else {
    Write-Host "  ? Already clean" -ForegroundColor Green
}

# Clean other cache files
$cacheFiles = @(
    "*.suo",
    "*.user"
)

foreach ($pattern in $cacheFiles) {
    $files = Get-ChildItem (Join-Path $projectPath $pattern) -Recurse -ErrorAction SilentlyContinue
    if ($files) {
        $files | Remove-Item -Force -ErrorAction SilentlyContinue
        Write-Host "  ? Deleted $($files.Count) $pattern files" -ForegroundColor Green
    }
}

Write-Host ""

# Step 3: Verify files
Write-Host "[3/4] Verifying project files..." -ForegroundColor Yellow
if (Test-Path $solutionFile) {
    Write-Host "  ? Solution file exists" -ForegroundColor Green
} else {
    Write-Host "  ? Solution file not found!" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Step 4: Open Visual Studio
Write-Host "[4/4] Opening Visual Studio..." -ForegroundColor Yellow
Write-Host ""
Write-Host "Opening: $solutionFile" -ForegroundColor Cyan
Write-Host ""

try {
    # Try to find Visual Studio
    $vsPath = Get-ChildItem "C:\Program Files\Microsoft Visual Studio" -Recurse -Filter "devenv.exe" -ErrorAction Stop | 
              Select-Object -First 1 -ExpandProperty FullName
    
    if ($vsPath) {
        Write-Host "Starting Visual Studio..." -ForegroundColor Gray
        Start-Process $vsPath -ArgumentList "`"$solutionFile`"" -WindowStyle Normal
        Write-Host "? Visual Studio launched" -ForegroundColor Green
    } else {
        Write-Host "? Could not find Visual Studio" -ForegroundColor Yellow
        Write-Host "Please open manually:" -ForegroundColor White
        Write-Host "  $solutionFile" -ForegroundColor Cyan
    }
} catch {
    Write-Host "? Could not auto-launch VS" -ForegroundColor Yellow
    Write-Host "Please open manually:" -ForegroundColor White
    Write-Host "  $solutionFile" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Done!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "If project still won't load:" -ForegroundColor Yellow
Write-Host "1. In Visual Studio: View ? Output" -ForegroundColor White
Write-Host "2. Dropdown: 'Show output from: Build'" -ForegroundColor White
Write-Host "3. Look for specific error messages" -ForegroundColor White
Write-Host "4. Share the error for more help" -ForegroundColor White
Write-Host ""
