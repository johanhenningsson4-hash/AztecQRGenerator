param(
    [string]$Solution = "AztecQRGenerator.sln",
    [string]$Configuration = "Debug"
)

$ErrorActionPreference = 'Stop'

Write-Host "Restore + Build solution: $Solution (Configuration=$Configuration)"

# Restore and build using MSBuild. For classic .NET Framework projects this is the most reliable path.
msbuild $Solution /t:Restore,Build /p:Configuration=$Configuration /m

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
foreach ($dll in $testDlls) {
    Write-Host "Running tests: $dll"
    & $vstestExe $dll /EnableCodeCoverage:false
    $rc = $LASTEXITCODE
    if ($rc -ne 0) {
        Write-Host "Test run failed for $dll (exit code $rc)" -ForegroundColor Red
        $overallExit = $rc
    }
}

if ($overallExit -eq 0) { Write-Host "All test runs completed successfully." } else { Write-Host "One or more test runs failed." -ForegroundColor Red }
exit $overallExit
