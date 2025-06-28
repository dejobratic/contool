#!/bin/bash
# Usage: ./pack_and_install_contool.sh

set -euo pipefail

PROJECT_PATH="./src/Contool.Console"
NUPKG_OUTPUT="$PROJECT_PATH/bin/Release"

# Step 1: Pack the .NET tool project in Release configuration
echo "📦 Packing 'contool' in Release configuration..."
dotnet pack "$PROJECT_PATH" -c Release

# Step 2: Uninstall the tool if it's already installed
echo "🧹 Uninstalling 'contool' if installed..."
dotnet tool uninstall --global contool || echo "'contool' was not previously installed."

# Step 3: Verify the .nupkg file exists
echo "🔍 Looking for .nupkg files in: $NUPKG_OUTPUT"
ls -la "$NUPKG_OUTPUT"/*.nupkg 2>/dev/null || {
    echo "❌ No .nupkg files found in $NUPKG_OUTPUT"
    echo "Available files:"
    ls -la "$NUPKG_OUTPUT" || echo "Directory doesn't exist"
    exit 1
}

# Step 4: Install the tool from the local nupkg folder
echo "📥 Installing 'contool' from: $NUPKG_OUTPUT"
dotnet tool install --global contool --add-source "$NUPKG_OUTPUT"

# Step 5: Verify the tool was installed successfully
echo "🔍 Verifying 'contool' installation..."
if dotnet tool list --global | grep -q "contool"; then
    echo "✅ 'contool' installed successfully from local source."
    echo "📋 Tool details:"
    dotnet tool list --global | grep "contool"
    echo ""
    echo "🚀 You can now run: contool --help"
else
    echo "❌ Installation verification failed. 'contool' not found in global tools."
    echo "📋 Currently installed global tools:"
    dotnet tool list --global
    exit 1
fi