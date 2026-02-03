param(
    [string]$Solution = "AztecQRGenerator.sln",
    [string]$Configuration = "Debug",
    [switch]$UseTestShims
)

$ErrorActionPreference = 'Stop'

Write-Host "Restore + Build solution: $Solution (Configuration=$Configuration)"

# Restore and build using MSBuild. For classic .NET Framework projects this is the most reliable path.
$msbuildExe = 'msbuild'
if (-not (Get-Command $msbuildExe -ErrorAction SilentlyContinue)) {
    Write-Host "msbuild not found on PATH. Attempting to locate via vswhere..."
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (Test-Path $vswhere) {
        $installPath = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath 2>$null
        if ($installPath) {
            # Try typical MSBuild locations under the installation
            $candidates = @(
                Join-Path $installPath 'MSBuild\Current\Bin\MSBuild.exe',
                Join-Path $installPath 'MSBuild\15.0\Bin\MSBuild.exe',
                Join-Path $installPath 'VC\Auxiliary\Build\msbuild.exe'
            )
            foreach ($cand in $candidates) {
                if (Test-Path $cand) { $msbuildExe = $cand; break }
            }
        }
    }
}

if (-not (Get-Command $msbuildExe -ErrorAction SilentlyContinue)) {
    Write-Error "msbuild not found on PATH and could not be located via vswhere. Ensure MSBuild is installed on the runner."
    exit 2
}

# Build with optional extra MSBuild properties
$msbuildArgs = @("$Solution", "/t:Restore,Build", "/p:Configuration=$Configuration", "/m")
if ($UseTestShims) {
    Write-Host "Enabling test shims via DefineConstants=USE_TEST_SHIMS"
    $msbuildArgs += "/p:DefineConstants=USE_TEST_SHIMS"
}

& $msbuildExe @msbuildArgs

Write-Host "Locating test assemblies (Configuration=$Configuration)..."

# find test assemblies under bin\<Configuration>
$testDlls = Get-ChildItem -Path . -Filter "*Tests.dll" -Recurse -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -match "\\bin\\$Configuration\\" } |
    Select-Object -ExpandProperty FullName -ErrorAction SilentlyContinue

if (-not $testDlls -or $testDlls.Count -eq 0) {
    Write-Host "No test assemblies found for configuration '$Configuration'."
    exit 0
}

$vstestExe = 'vstest.console.exe'

if (-not (Get-Command $vstestExe -ErrorAction SilentlyContinue)) {
    Write-Host "vstest.console.exe not found in PATH. Attempting to locate via vswhere..."
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (Test-Path $vswhere) {
        $installPath = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath 2>$null
        if ($installPath) {
            $possible = Join-Path $installPath 'Common7\IDE\Extensions\TestPlatform\vstest.console.exe'
            if (Test-Path $possible) { $vstestExe = $possible }
        }
    }
}

if (-not (Get-Command $vstestExe -ErrorAction SilentlyContinue)) {
    Write-Error "vstest.console.exe not found. Please ensure Visual Studio Test Platform is installed or vstest.console.exe is on PATH."
    exit 2
}

$overallExit = 0

# Prepare results directory for test runs and coverage
$resultsRoot = Join-Path (Get-Location) 'TestResults'
if (-Not (Test-Path $resultsRoot)) { New-Item -ItemType Directory -Path $resultsRoot | Out-Null }

foreach ($dll in $testDlls) {
    $name = [System.IO.Path]::GetFileNameWithoutExtension($dll)
    $timestamp = Get-Date -Format "yyyyMMddHHmmss"
    $resultsDir = Join-Path $resultsRoot "$name_$timestamp"
    New-Item -ItemType Directory -Path $resultsDir | Out-Null

    $trxName = "$name_$timestamp.trx"
    Write-Host "Running tests: $dll -> results: $resultsDir"

    & $vstestExe $dll /ResultsDirectory:$resultsDir /Logger:"trx;LogFileName=$trxName" /EnableCodeCoverage:true
    $rc = $LASTEXITCODE

    if ($rc -ne 0) {
        Write-Host "Test run failed for $dll (exit code $rc)" -ForegroundColor Red
        $overallExit = $rc
    }
}

if ($overallExit -eq 0) { Write-Host "All test runs completed successfully." } else { Write-Host "One or more test runs failed." -ForegroundColor Red }
exit $overallExit
