<#
    Fix-ZXing-Project.ps1

    Usage:
      - Open PowerShell in repository root (where the solution is located)
      - Run: .\Fix-ZXing-Project.ps1

    What it does:
      - Backs up the AztecQRGenerator.Core csproj
      - Removes PackageReference Include="ZXing" if present
      - Ensures PackageReference Include="ZXing.Net" Version="0.16.11" exists
      - Adds the conditional compilation symbol HAS_ZXING to PropertyGroup DefineConstants entries (Debug/Release)
      - Attempts to run `dotnet restore`, `nuget restore`, and `msbuild /t:Restore` (whichever is available)
      - Prints summary at the end
#>

param(
    [string]$ProjectPath = "AztecQRGenerator.Core\AztecQRGenerator.Core.csproj",
    [string]$ZXingNetVersion = "0.16.11",
    [switch]$RunRestore
)

function Write-Info { param($m) Write-Host "[INFO]  $m" -ForegroundColor Cyan }
function Write-Warn { param($m) Write-Host "[WARN]  $m" -ForegroundColor Yellow }
function Write-Err  { param($m) Write-Host "[ERROR] $m" -ForegroundColor Red }

if (-not (Test-Path $ProjectPath)) {
    Write-Err "Project file not found: $ProjectPath"
    exit 1
}

$timestamp = Get-Date -Format "yyyyMMddHHmmss"
$backupPath = "$ProjectPath.bak.$timestamp"
Write-Info "Backing up project to: $backupPath"
Copy-Item -Path $ProjectPath -Destination $backupPath -Force

[xml]$doc = New-Object System.Xml.XmlDocument
$doc.PreserveWhitespace = $true
$doc.Load($ProjectPath)

$nsMgr = New-Object System.Xml.XmlNamespaceManager($doc.NameTable)
$nsMgr.AddNamespace('msb','http://schemas.microsoft.com/developer/msbuild/2003') | Out-Null
$ns = 'http://schemas.microsoft.com/developer/msbuild/2003'

# Remove PackageReference Include="ZXing"
$removeNodes = $doc.SelectNodes("//msb:PackageReference[@Include='ZXing']", $nsMgr)
$removed = $false
if ($removeNodes -and $removeNodes.Count -gt 0) {
    foreach ($n in $removeNodes) {
        $parent = $n.ParentNode
        $parent.RemoveChild($n) | Out-Null
        $removed = $true
        Write-Info "Removed PackageReference Include=\"ZXing\""
    }
} else {
    Write-Info "No PackageReference Include=\"ZXing\" found"
}

# Ensure ZXing.Net PackageReference exists
$zxnetNode = $doc.SelectSingleNode("//msb:PackageReference[@Include='ZXing.Net']", $nsMgr)
if (-not $zxnetNode) {
    # Try to find an ItemGroup that contains other PackageReference nodes
    $itemGroup = $doc.SelectSingleNode('//msb:ItemGroup[msb:PackageReference]', $nsMgr)
    if (-not $itemGroup) {
        # Create a new ItemGroup under Project
        $itemGroup = $doc.CreateElement('ItemGroup', $ns)
        $doc.DocumentElement.AppendChild($itemGroup) | Out-Null
    }

    $pr = $doc.CreateElement('PackageReference', $ns)
    $incAttr = $doc.CreateAttribute('Include')
    $incAttr.Value = 'ZXing.Net'
    $pr.Attributes.Append($incAttr) | Out-Null

    $ver = $doc.CreateElement('Version', $ns)
    $ver.InnerText = $ZXingNetVersion
    $pr.AppendChild($ver) | Out-Null

    $itemGroup.AppendChild($pr) | Out-Null
    Write-Info "Added PackageReference Include=\"ZXing.Net\" Version=\"$ZXingNetVersion\""
} else {
    $verNode = $zxnetNode.SelectSingleNode('msb:Version', $nsMgr)
    if ($verNode) {
        if ($verNode.InnerText -ne $ZXingNetVersion) {
            Write-Warn "ZXing.Net version differs (found $($verNode.InnerText)). Updating to $ZXingNetVersion"
            $verNode.InnerText = $ZXingNetVersion
        } else {
            Write-Info "ZXing.Net PackageReference already present and version matches"
        }
    } else {
        $ver = $doc.CreateElement('Version', $ns)
        $ver.InnerText = $ZXingNetVersion
        $zxnetNode.AppendChild($ver) | Out-Null
        Write-Info "Added Version element to existing ZXing.Net PackageReference"
    }
}

# Ensure DefineConstants contains HAS_ZXING in PropertyGroups with DefineConstants
$pgNodes = $doc.SelectNodes('//msb:PropertyGroup[msb:DefineConstants]', $nsMgr)
if ($pgNodes -and $pgNodes.Count -gt 0) {
    foreach ($pg in $pgNodes) {
        $dc = $pg.SelectSingleNode('msb:DefineConstants', $nsMgr)
        if ($dc -and ($dc.InnerText -notmatch '\bHAS_ZXING\b')) {
            # Append HAS_ZXING
            $existing = $dc.InnerText.Trim()
            if ($existing -ne '') {
                $dc.InnerText = "$existing;HAS_ZXING"
            } else {
                $dc.InnerText = 'HAS_ZXING'
            }
            Write-Info "Added HAS_ZXING to DefineConstants in a PropertyGroup"
        } else {
            Write-Info "DefineConstants already contains HAS_ZXING or not present"
        }
    }
} else {
    # No DefineConstants found; add one to the first PropertyGroup
    $firstPG = $doc.SelectSingleNode('//msb:PropertyGroup', $nsMgr)
    if ($firstPG) {
        $dc = $doc.CreateElement('DefineConstants', $ns)
        $dc.InnerText = 'HAS_ZXING'
        $firstPG.AppendChild($dc) | Out-Null
        Write-Info "Created DefineConstants with HAS_ZXING in first PropertyGroup"
    }
}

# Save changes
$doc.Save($ProjectPath)
Write-Info "Saved project file: $ProjectPath"

# Optionally run restores/build to verify
if ($RunRestore) {
    $restored = $false
    Write-Info "Attempting dotnet restore..."
    try {
        $proc = Start-Process -FilePath dotnet -ArgumentList 'restore' -NoNewWindow -Wait -PassThru -ErrorAction Stop
        if ($proc.ExitCode -eq 0) { $restored = $true; Write-Info 'dotnet restore succeeded' }
    } catch {
        Write-Warn 'dotnet restore not available or failed'
    }

    if (-not $restored) {
        Write-Info 'Attempting nuget restore...'
        try {
            $proc = Start-Process -FilePath nuget -ArgumentList "restore `"$ProjectPath`"" -NoNewWindow -Wait -PassThru -ErrorAction Stop
            if ($proc.ExitCode -eq 0) { $restored = $true; Write-Info 'nuget restore succeeded' }
        } catch {
            Write-Warn 'nuget restore not available or failed'
        }
    }

    if (-not $restored) {
        Write-Info 'Attempting msbuild -t:Restore...'
        try {
            $proc = Start-Process -FilePath msbuild -ArgumentList "`"$ProjectPath`" /t:Restore" -NoNewWindow -Wait -PassThru -ErrorAction Stop
            if ($proc.ExitCode -eq 0) { $restored = $true; Write-Info 'msbuild restore succeeded' }
        } catch {
            Write-Warn 'msbuild restore not available or failed'
        }
    }

    if (-not $restored) {
        Write-Warn 'Could not automatically restore packages. Please run `dotnet restore` or `nuget restore` manually.'
    }
}

Write-Info 'Done. Please rebuild the solution in your IDE to verify all errors are fixed.'
Write-Info "Backup saved at: $backupPath"

exit 0
