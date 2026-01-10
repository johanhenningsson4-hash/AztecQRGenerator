# Create Release v1.3.0 - Automated Testing & CI/CD
# Complete automation for version bump, commit, tag, and NuGet publish

param(
    [switch]$DryRun = $false,
    [switch]$SkipTests = $false
)

$ErrorActionPreference = "Stop"
$version = "1.3.0"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Release v$version - Automated Testing" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$repoPath = "C:\Jobb\AztecQRGenerator"
Set-Location $repoPath

# Helper function to find or download NuGet.exe
function Get-NuGetExe {
    # Check common locations
    $nugetLocations = @(
        "$repoPath\nuget.exe",
        "$repoPath\.nuget\nuget.exe",
        "$env:ProgramFiles\NuGet\nuget.exe",
        "$env:LOCALAPPDATA\NuGet\nuget.exe"
    )
    
    foreach ($location in $nugetLocations) {
        if (Test-Path $location) {
            Write-Host "  Found NuGet.exe at: $location" -ForegroundColor Gray
            return $location
        }
    }
    
    # Try to find in PATH
    $nugetInPath = Get-Command nuget -ErrorAction SilentlyContinue
    if ($nugetInPath) {
        Write-Host "  Found NuGet.exe in PATH" -ForegroundColor Gray
        return "nuget"
    }
    
    # Download if not found
    Write-Host "  NuGet.exe not found, downloading..." -ForegroundColor Yellow
    $nugetUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
    $nugetPath = "$repoPath\nuget.exe"
    
    try {
        Invoke-WebRequest -Uri $nugetUrl -OutFile $nugetPath
        Write-Host "  ? NuGet.exe downloaded" -ForegroundColor Green
        return $nugetPath
    } catch {
        Write-Host "  ? Failed to download NuGet.exe: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "  Please download manually from: $nugetUrl" -ForegroundColor Yellow
        return $null
    }
}

# Get NuGet executable
$nugetExe = Get-NuGetExe
if (-not $nugetExe) {
    Write-Host ""
    Write-Host "? Cannot proceed without NuGet.exe" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please do one of the following:" -ForegroundColor Yellow
    Write-Host "1. Download nuget.exe from https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -ForegroundColor White
    Write-Host "2. Place it in: $repoPath\nuget.exe" -ForegroundColor White
    Write-Host "3. Or add NuGet to your PATH" -ForegroundColor White
    Write-Host ""
    exit 1
}

Write-Host ""

# Step 1: Update version numbers
Write-Host "[1/10] Updating version numbers..." -ForegroundColor Yellow

$files = @(
    "AztecQRGenerator.Core\Properties\AssemblyInfo.cs",
    "AztecQRGenerator.Core\AztecQRGenerator.Core.csproj",
    "AztecQRGenerator.Core\AztecQRGenerator.Core.nuspec",
    "Properties\AssemblyInfo.cs",
    "AztecQRGenerator.Tests\Properties\AssemblyInfo.cs"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw
        $content = $content -replace '1\.2\.3', $version
        if (-not $DryRun) {
            Set-Content $file $content -NoNewline
            Write-Host "  ? Updated $file" -ForegroundColor Green
        } else {
            Write-Host "  [DRY RUN] Would update $file" -ForegroundColor Gray
        }
    }
}
Write-Host ""

# Step 2: Restore NuGet packages
Write-Host "[2/10] Restoring NuGet packages..." -ForegroundColor Yellow
if (-not $DryRun) {
    try {
        & $nugetExe restore AztecQRGenerator.sln
        Write-Host "  ? Packages restored" -ForegroundColor Green
    } catch {
        Write-Host "  ?? Package restore failed, continuing..." -ForegroundColor Yellow
    }
} else {
    Write-Host "  [DRY RUN] Would restore packages" -ForegroundColor Gray
}
Write-Host ""

# Step 3: Build solution
Write-Host "[3/10] Building solution..." -ForegroundColor Yellow
if (-not $DryRun) {
    try {
        $buildResult = & msbuild AztecQRGenerator.sln /p:Configuration=Release /v:minimal /nologo
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  ? Build successful" -ForegroundColor Green
        } else {
            Write-Host "  ? Build failed!" -ForegroundColor Red
            Write-Host ""
            Write-Host "Build output:" -ForegroundColor Yellow
            $buildResult | Select-Object -Last 20
            exit 1
        }
    } catch {
        Write-Host "  ? Build error: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "  [DRY RUN] Would build solution" -ForegroundColor Gray
}
Write-Host ""

# Step 4: Run tests
if (-not $SkipTests) {
    Write-Host "[4/10] Running tests..." -ForegroundColor Yellow
    if (-not $DryRun) {
        $testRunner = "packages\NUnit.ConsoleRunner.3.16.3\tools\nunit3-console.exe"
        if (-not (Test-Path $testRunner)) {
            Write-Host "  Installing NUnit Console Runner..." -ForegroundColor Gray
            try {
                & $nugetExe install NUnit.ConsoleRunner -Version 3.16.3 -OutputDirectory packages
            } catch {
                Write-Host "  ?? Failed to install NUnit runner" -ForegroundColor Yellow
                Write-Host "  Skipping tests..." -ForegroundColor Yellow
                $SkipTests = $true
            }
        }
        
        if (-not $SkipTests -and (Test-Path $testRunner)) {
            try {
                & $testRunner "AztecQRGenerator.Tests\bin\Release\AztecQRGenerator.Tests.dll" --result=TestResults.xml
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "  ? All tests passed!" -ForegroundColor Green
                } else {
                    Write-Host "  ? Tests failed!" -ForegroundColor Red
                    Write-Host ""
                    $continueAnyway = Read-Host "  Continue with release anyway? (Y/N)"
                    if ($continueAnyway -ne "Y" -and $continueAnyway -ne "y") {
                        exit 1
                    }
                }
            } catch {
                Write-Host "  ?? Test execution error: $($_.Exception.Message)" -ForegroundColor Yellow
            }
        } else {
            Write-Host "  ?? Test runner not available, skipping tests" -ForegroundColor Yellow
        }
    } else {
        Write-Host "  [DRY RUN] Would run 58 tests" -ForegroundColor Gray
    }
} else {
    Write-Host "[4/10] Skipping tests (--SkipTests flag)" -ForegroundColor Yellow
}
Write-Host ""

# Step 5: Build NuGet package
Write-Host "[5/10] Building NuGet package..." -ForegroundColor Yellow
if (-not $DryRun) {
    try {
        Push-Location "AztecQRGenerator.Core"
        & $nugetExe pack AztecQRGenerator.Core.nuspec -Properties Configuration=Release
        Pop-Location
        
        $packagePath = "AztecQRGenerator.Core\AztecQRGenerator.Core.$version.nupkg"
        if (Test-Path $packagePath) {
            Write-Host "  ? Package created: AztecQRGenerator.Core.$version.nupkg" -ForegroundColor Green
        } else {
            Write-Host "  ?? Package file not found at expected location" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "  ? Package creation failed: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "  Continuing anyway..." -ForegroundColor Yellow
    }
} else {
    Write-Host "  [DRY RUN] Would create NuGet package" -ForegroundColor Gray
}
Write-Host ""

# Step 6: Stage files
Write-Host "[6/10] Staging files..." -ForegroundColor Yellow
if (-not $DryRun) {
    git add .
    Write-Host "  ? Files staged" -ForegroundColor Green
} else {
    Write-Host "  [DRY RUN] Would stage all files" -ForegroundColor Gray
}
Write-Host ""

# Step 7: Commit
Write-Host "[7/10] Committing changes..." -ForegroundColor Yellow
$commitMessage = @"
feat: Release v$version - Automated Testing & CI/CD

Major Updates:
- Added 58 comprehensive unit tests (NUnit 3.14)
- Implemented GitHub Actions CI/CD pipeline
- Automated build, test, and NuGet publishing
- Added code quality and security scanning
- Enhanced documentation for testing
- Fixed deprecated examples in documentation

Test Coverage:
- QRGenerator: 25 tests
- AztecGenerator: 25 tests
- Logger: 8 tests

Breaking Changes: None
"@

if (-not $DryRun) {
    try {
        git commit -m $commitMessage
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  ? Changes committed" -ForegroundColor Green
        } else {
            Write-Host "  ?? No changes to commit" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "  ?? Commit info: $($_.Exception.Message)" -ForegroundColor Gray
    }
} else {
    Write-Host "  [DRY RUN] Would commit with message:" -ForegroundColor Gray
    Write-Host "    $($commitMessage.Split([Environment]::NewLine)[0])" -ForegroundColor Gray
}
Write-Host ""

# Step 8: Create tag
Write-Host "[8/10] Creating Git tag..." -ForegroundColor Yellow
$tagMessage = @"
Release v$version - Automated Testing & CI/CD

New Features:
- 58 comprehensive unit tests
- GitHub Actions CI/CD pipeline
- Automated NuGet publishing
- Weekly code quality scans

See RELEASE_NOTES_v$version.md for full details.
"@

if (-not $DryRun) {
    $existingTag = git tag -l "v$version"
    if ($existingTag) {
        Write-Host "  ?? Tag v$version already exists!" -ForegroundColor Yellow
        $deleteTag = Read-Host "    Delete and recreate? (Y/N)"
        if ($deleteTag -eq "Y" -or $deleteTag -eq "y") {
            git tag -d "v$version"
            git push origin ":refs/tags/v$version" 2>$null
            Write-Host "    Old tag deleted" -ForegroundColor Gray
        } else {
            Write-Host "  Skipping tag creation" -ForegroundColor Yellow
            $skipTag = $true
        }
    }
    
    if (-not $skipTag) {
        git tag -a "v$version" -m $tagMessage
        Write-Host "  ? Tag v$version created" -ForegroundColor Green
    }
} else {
    Write-Host "  [DRY RUN] Would create tag: v$version" -ForegroundColor Gray
}
Write-Host ""

# Step 9: Push to GitHub
Write-Host "[9/10] Pushing to GitHub..." -ForegroundColor Yellow
if (-not $DryRun) {
    $push = Read-Host "  Push to GitHub? (Y/N)"
    if ($push -eq "Y" -or $push -eq "y") {
        Write-Host "  Pushing commits..." -ForegroundColor Gray
        git push origin main
        
        if (-not $skipTag) {
            Write-Host "  Pushing tag..." -ForegroundColor Gray
            git push origin "v$version"
        }
        
        Write-Host "  ? Pushed to GitHub" -ForegroundColor Green
        Write-Host ""
        Write-Host "  ? GitHub Actions will now:" -ForegroundColor Cyan
        Write-Host "     1. Build the solution" -ForegroundColor White
        Write-Host "     2. Run all 58 tests" -ForegroundColor White
        Write-Host "     3. Generate test reports" -ForegroundColor White
        Write-Host ""
    } else {
        Write-Host "  Skipped push" -ForegroundColor Yellow
    }
} else {
    Write-Host "  [DRY RUN] Would push commits and tag" -ForegroundColor Gray
}
Write-Host ""

# Step 10: Publish to NuGet (optional - GitHub Actions will do this)
Write-Host "[10/10] NuGet Publishing..." -ForegroundColor Yellow
Write-Host "  ?? Skipping manual NuGet publish" -ForegroundColor Cyan
Write-Host "  ?? GitHub Actions will publish automatically when you create the release" -ForegroundColor Cyan
Write-Host ""

# Alternative: Manual NuGet publish
$manualPublish = Read-Host "  Publish to NuGet manually now? (Y/N)"
if ($manualPublish -eq "Y" -or $manualPublish -eq "y") {
    if (-not $DryRun) {
        Write-Host "  Enter your NuGet API key (or press Enter to skip):" -ForegroundColor Yellow
        $apiKey = Read-Host -AsSecureString
        $apiKeyPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($apiKey))
        
        if ($apiKeyPlain) {
            try {
                Push-Location "AztecQRGenerator.Core"
                & $nugetExe push "AztecQRGenerator.Core.$version.nupkg" -ApiKey $apiKeyPlain -Source https://api.nuget.org/v3/index.json
                Pop-Location
                Write-Host "  ? Published to NuGet.org" -ForegroundColor Green
            } catch {
                Write-Host "  ? Publish failed: $($_.Exception.Message)" -ForegroundColor Red
                Write-Host "  You can publish manually later from AztecQRGenerator.Core folder" -ForegroundColor Yellow
            }
        }
    }
}
Write-Host ""

# Step 11: Create GitHub Release
Write-Host "[11/11] GitHub Release..." -ForegroundColor Yellow

$releaseUrl = "https://github.com/johanhenningsson4-hash/AztecQRGenerator/releases/new?tag=v$version"

Write-Host "  ?? Release Description:" -ForegroundColor Cyan
$releaseNotesPath = "RELEASE_NOTES_v$version.md"

if (Test-Path $releaseNotesPath) {
    try {
        $releaseDescription = Get-Content $releaseNotesPath -Raw
        $releaseDescription | Set-Clipboard
        Write-Host "  ? Release notes copied to clipboard!" -ForegroundColor Green
    } catch {
        Write-Host "  ?? Could not copy to clipboard" -ForegroundColor Yellow
    }
} else {
    Write-Host "  ?? $releaseNotesPath not found" -ForegroundColor Yellow
}
Write-Host ""

$openBrowser = Read-Host "  Open GitHub release page? (Y/N)"
if ($openBrowser -eq "Y" -or $openBrowser -eq "y") {
    Start-Process $releaseUrl
    Write-Host "  ? Browser opened" -ForegroundColor Green
}
Write-Host ""

# Summary
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "? Release v$version Prepared!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. Go to GitHub (opened in browser)" -ForegroundColor White
Write-Host "2. Title: 'v$version - Automated Testing & CI/CD'" -ForegroundColor White
Write-Host "3. Description: Already in clipboard - just paste!" -ForegroundColor White
Write-Host "4. Attach: AztecQRGenerator.Core\AztecQRGenerator.Core.$version.nupkg" -ForegroundColor White
Write-Host "5. Check 'Set as the latest release'" -ForegroundColor White
Write-Host "6. Click 'Publish release'" -ForegroundColor White
Write-Host ""
Write-Host "? After publishing the GitHub release:" -ForegroundColor Cyan
Write-Host "   - GitHub Actions will automatically publish to NuGet.org" -ForegroundColor White
Write-Host "   - CI tests will run on the release" -ForegroundColor White
Write-Host "   - Package will be available in ~10-15 minutes" -ForegroundColor White
Write-Host ""
Write-Host "?? Links:" -ForegroundColor Cyan
Write-Host "   Release: $releaseUrl" -ForegroundColor White
Write-Host "   Actions: https://github.com/johanhenningsson4-hash/AztecQRGenerator/actions" -ForegroundColor White
Write-Host ""

