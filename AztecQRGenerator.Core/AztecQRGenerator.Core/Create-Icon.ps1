# Create icon.png for AztecQRGenerator project
# Run this script to generate a 128x128 PNG icon

Write-Host "Creating project icon..." -ForegroundColor Cyan
Write-Host ""

Add-Type -AssemblyName System.Drawing

$size = 128
$margin = 10
$cellSize = 6

# Create bitmap and graphics
$bmp = New-Object System.Drawing.Bitmap($size, $size)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

# Background gradient
$bgBrush = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
    (New-Object System.Drawing.Rectangle(0, 0, $size, $size)),
    [System.Drawing.Color]::FromArgb(240, 240, 245),
    [System.Drawing.Color]::FromArgb(250, 250, 255),
    [System.Drawing.Drawing2D.LinearGradientMode]::Vertical
)
$g.FillRectangle($bgBrush, 0, 0, $size, $size)

# Calculate grid
$gridSize = [Math]::Floor(($size - 2 * $margin) / $cellSize)

# Create brushes
$blackBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(20, 20, 40))
$accentBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(65, 105, 225))

# Fixed random seed for consistency
$rand = New-Object System.Random(12345)

# Draw QR-like pattern
for ($y = 0; $y -lt $gridSize; $y++) {
    for ($x = 0; $x -lt $gridSize; $x++) {
        # Position markers (corners)
        $isTopLeft = ($x -lt 3 -and $y -lt 3)
        $isTopRight = ($x -ge $gridSize - 3 -and $y -lt 3)
        $isBottomLeft = ($x -lt 3 -and $y -ge $gridSize - 3)
        $isPositionMarker = $isTopLeft -or $isTopRight -or $isBottomLeft
        
        # Position marker borders
        $isCornerBorder = $false
        if ($isTopLeft) {
            $isCornerBorder = ($x -eq 0 -or $x -eq 2 -or $y -eq 0 -or $y -eq 2)
        } elseif ($isTopRight) {
            $isCornerBorder = ($x -eq $gridSize - 1 -or $x -eq $gridSize - 3 -or $y -eq 0 -or $y -eq 2)
        } elseif ($isBottomLeft) {
            $isCornerBorder = ($x -eq 0 -or $x -eq 2 -or $y -eq $gridSize - 1 -or $y -eq $gridSize - 3)
        }
        
        # Position marker centers
        $isCenterSquare = $false
        if ($isTopLeft -and $x -eq 1 -and $y -eq 1) { $isCenterSquare = $true }
        if ($isTopRight -and $x -eq $gridSize - 2 -and $y -eq 1) { $isCenterSquare = $true }
        if ($isBottomLeft -and $x -eq 1 -and $y -eq $gridSize - 2) { $isCenterSquare = $true }
        
        # Decide whether to fill
        $shouldFill = $false
        $brush = $blackBrush
        
        if ($isPositionMarker) {
            if ($isCornerBorder -or $isCenterSquare) {
                $shouldFill = $true
                $brush = $accentBrush
            }
        } else {
            # Random pattern for data area
            $probability = 0.5
            # Denser in center
            if ([Math]::Abs($x - $gridSize / 2) -lt 2 -and [Math]::Abs($y - $gridSize / 2) -lt 2) {
                $probability = 0.65
            }
            $shouldFill = $rand.NextDouble() -lt $probability
        }
        
        if ($shouldFill) {
            $g.FillRectangle(
                $brush,
                $margin + $x * $cellSize,
                $margin + $y * $cellSize,
                $cellSize - 1,
                $cellSize - 1
            )
        }
    }
}

# Add border
$borderPen = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(200, 200, 210), 2)
$g.DrawRectangle($borderPen, 1, 1, $size - 3, $size - 3)

# Save
$bmp.Save("$PSScriptRoot\icon.png", [System.Drawing.Imaging.ImageFormat]::Png)

# Cleanup
$g.Dispose()
$bmp.Dispose()
$bgBrush.Dispose()
$blackBrush.Dispose()
$accentBrush.Dispose()
$borderPen.Dispose()

Write-Host "? Icon created successfully!" -ForegroundColor Green
Write-Host "  Location: $PSScriptRoot\icon.png" -ForegroundColor Gray
Write-Host "  Size: 128x128 pixels" -ForegroundColor Gray
Write-Host "  Format: PNG" -ForegroundColor Gray
Write-Host ""
Write-Host "The icon is ready to be included in your NuGet package." -ForegroundColor Cyan
