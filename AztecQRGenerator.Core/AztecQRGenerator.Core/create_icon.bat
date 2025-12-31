@echo off
echo Creating project icon...
echo.

REM Create a simple PowerShell script to generate the icon
powershell -Command "& { Add-Type -AssemblyName System.Drawing; $size = 128; $bmp = New-Object System.Drawing.Bitmap($size, $size); $g = [System.Drawing.Graphics]::FromImage($bmp); $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias; $g.Clear([System.Drawing.Color]::FromArgb(240, 240, 245)); $cellSize = 6; $margin = 10; $gridSize = [Math]::Floor(($size - 2 * $margin) / $cellSize); $rand = New-Object System.Random(12345); $blackBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(20, 20, 40)); $accentBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(65, 105, 225)); for ($y = 0; $y -lt $gridSize; $y++) { for ($x = 0; $x -lt $gridSize; $x++) { $isPositionMarker = ($x -lt 3 -and $y -lt 3) -or ($x -ge $gridSize - 3 -and $y -lt 3) -or ($x -lt 3 -and $y -ge $gridSize - 3); $isCorner = ($x -eq 0 -or $x -eq 2 -or $y -eq 0 -or $y -eq 2) -and $isPositionMarker; $isCenter = (($x -eq 1 -and $y -eq 1) -or ($x -eq $gridSize - 2 -and $y -eq 1) -or ($x -eq 1 -and $y -eq $gridSize - 2)) -and $isPositionMarker; if ($isPositionMarker -and ($isCorner -or $isCenter)) { $g.FillRectangle($accentBrush, $margin + $x * $cellSize, $margin + $y * $cellSize, $cellSize - 1, $cellSize - 1); } elseif (-not $isPositionMarker -and $rand.NextDouble() -lt 0.5) { $g.FillRectangle($blackBrush, $margin + $x * $cellSize, $margin + $y * $cellSize, $cellSize - 1, $cellSize - 1); } } }; $borderPen = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(200, 200, 210), 2); $g.DrawRectangle($borderPen, 1, 1, $size - 3, $size - 3); $bmp.Save('icon.png', [System.Drawing.Imaging.ImageFormat]::Png); $g.Dispose(); $bmp.Dispose(); $blackBrush.Dispose(); $accentBrush.Dispose(); $borderPen.Dispose(); Write-Host 'Icon created successfully: icon.png' -ForegroundColor Green }"

if exist icon.png (
    echo.
    echo ? Icon created successfully: icon.png
    echo   Size: 128x128 pixels
    echo   Format: PNG
    echo.
    echo The icon is now ready to be used in your NuGet package.
) else (
    echo.
    echo ? Failed to create icon
)

pause
