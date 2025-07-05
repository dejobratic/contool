# pack_and_install_contool.ps1
# Usage: .\pack_and_install_contool.ps1
$ErrorActionPreference = "Stop"

# Color definitions (using Write-Host with colors)
function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = "White",
        [switch]$NoNewLine
    )
    
    if ($NoNewLine) {
        Write-Host $Message -ForegroundColor $Color -NoNewLine
    } else {
        Write-Host $Message -ForegroundColor $Color
    }
}

# Path to the .NET tool project
$PROJECT_PATH = "./src/Contool.Console"
$NUPKG_OUTPUT = "$PROJECT_PATH/bin/Release"

# Step 1: Pack the .NET tool project in Release configuration
Write-ColorOutput "[PACK] Packing 'contool' in Release configuration..." -Color "Blue"
try {
    dotnet pack "$PROJECT_PATH" -c Release
    if ($LASTEXITCODE -eq 0) {
        Write-ColorOutput "[SUCCESS] Packing completed successfully." -Color "Green"
    } else {
        throw "Packing failed with exit code $LASTEXITCODE"
    }
} catch {
    Write-ColorOutput "[ERROR] Packing failed." -Color "Red"
    Write-ColorOutput $_.Exception.Message -Color "Red"
    exit 1
}

# Step 2: Uninstall the tool if it's already installed
Write-ColorOutput "[CLEAN] Uninstalling 'contool' if installed..." -Color "Blue"
$output = dotnet tool uninstall --global contool 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-ColorOutput "[SUCCESS] Previous version uninstalled successfully." -Color "Green"
} else {
    Write-ColorOutput "[INFO] 'contool' was not previously installed." -Color "Green"
}

# Step 3: Verify the .nupkg file exists
Write-ColorOutput "[VERIFY] Looking for .nupkg files in: $NUPKG_OUTPUT" -Color "Blue"
try {
    $nupkgFiles = Get-ChildItem -Path "$NUPKG_OUTPUT/*.nupkg" -ErrorAction Stop
    if ($nupkgFiles.Count -gt 0) {
        Write-ColorOutput "[SUCCESS] Found .nupkg file:" -Color "Green"
        foreach ($file in $nupkgFiles) {
            $sizeInMB = [math]::Round($file.Length / 1MB, 2)
            $fileInfo = "    $($file.Name) (${sizeInMB} MB)"
            Write-Host $fileInfo
        }
    } else {
        throw "No .nupkg files found"
    }
} catch {
    Write-ColorOutput "[ERROR] No .nupkg files found in $NUPKG_OUTPUT" -Color "Red"
    Write-ColorOutput "Available files:" -Color "Red"
    try {
        Get-ChildItem -Path $NUPKG_OUTPUT | Format-Table Name, Length, LastWriteTime
    } catch {
        Write-ColorOutput "Directory doesn't exist" -Color "Red"
    }
    exit 1
}

# Step 4: Install the tool from the local nupkg folder
Write-ColorOutput "[INSTALL] Installing 'contool' from: $NUPKG_OUTPUT" -Color "Blue"
try {
    dotnet tool install --global contool --add-source "$NUPKG_OUTPUT"
    if ($LASTEXITCODE -eq 0) {
        Write-ColorOutput "[SUCCESS] Installation completed successfully." -Color "Green"
    } else {
        throw "Installation failed with exit code $LASTEXITCODE"
    }
} catch {
    Write-ColorOutput "[ERROR] Installation failed." -Color "Red"
    Write-ColorOutput $_.Exception.Message -Color "Red"
    exit 1
}

# Step 5: Verify the tool was installed successfully
Write-ColorOutput "[VERIFY] Verifying 'contool' installation..." -Color "Blue"
try {
    $toolList = dotnet tool list --global | Out-String
    if ($toolList -match "contool") {
        Write-ColorOutput "[SUCCESS] 'contool' installed successfully from local source." -Color "Green"
        Write-ColorOutput "[INFO] Tool details:" -Color "Green"
        
        # Extract and display the contool line
        $contoolLine = $toolList -split "`n" | Where-Object { $_ -match "contool" }
        if ($contoolLine) {
            Write-Host $contoolLine.Trim()
        }
        Write-Host ""
    } else {
        Write-ColorOutput "[WARNING] 'contool' not found in global tools list." -Color "Yellow"
    }
} catch {
    Write-ColorOutput "[WARNING] Could not verify installation." -Color "Yellow"
}

# Optional: Display the tool path
try {
    $toolPath = dotnet tool list --global --format json | ConvertFrom-Json
    $contoolEntry = $toolPath | Where-Object { $_.Id -eq "contool" }
    if ($contoolEntry) {
        Write-ColorOutput "[INFO] Tool location: $($contoolEntry.Path)" -Color "Cyan"
    }
} catch {
    # Silently ignore if JSON format is not supported
}