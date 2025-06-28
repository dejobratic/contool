#!/bin/bash
# Usage: ./pack_and_install_contool.sh

set -euo pipefail

# Color definitions
RED="\033[0;31m"
GREEN="\033[0;32m"
YELLOW="\033[1;33m"
BLUE="\033[1;34m"
BOLD="\033[1m"
RESET="\033[0m"

# Path to the .NET tool project
PROJECT_PATH="./src/Contool.Console"
NUPKG_OUTPUT="$PROJECT_PATH/bin/Release"

# Step 1: Pack the .NET tool project in Release configuration
echo -e "${BLUE}${BOLD}📦 Packing 'contool' in Release configuration...${RESET}"
if dotnet pack "$PROJECT_PATH" -c Release; then
    echo -e "${GREEN}✅ Packing completed successfully.${RESET}"
else
    echo -e "${RED}❌ Packing failed.${RESET}"
    exit 1
fi

# Step 2: Uninstall the tool if it's already installed
echo -e "${BLUE}${BOLD}🧹 Uninstalling 'contool' if installed...${RESET}"
if dotnet tool uninstall --global contool 2>/dev/null; then
    echo -e "${GREEN}✅ Previous version uninstalled successfully.${RESET}"
else
    echo -e "${GREEN}ℹ️  'contool' was not previously installed.${RESET}"
fi

# Step 3: Verify the .nupkg file exists
echo -e "${BLUE}${BOLD}🔍 Looking for .nupkg files in: $NUPKG_OUTPUT${RESET}"
if ls "$NUPKG_OUTPUT"/*.nupkg 1>/dev/null 2>&1; then
    echo -e "${GREEN}✅ Found .nupkg file:${RESET}"
    ls -la "$NUPKG_OUTPUT"/*.nupkg
else
    echo -e "${RED}❌ No .nupkg files found in $NUPKG_OUTPUT${RESET}"
    echo -e "${RED}Available files:${RESET}"
    ls -la "$NUPKG_OUTPUT" 2>/dev/null || echo -e "${RED}Directory doesn't exist${RESET}"
    exit 1
fi

# Step 4: Install the tool from the local nupkg folder
echo -e "${BLUE}${BOLD}📥 Installing 'contool' from: $NUPKG_OUTPUT${RESET}"
if dotnet tool install --global contool --add-source "$NUPKG_OUTPUT"; then
    echo -e "${GREEN}✅ Installation completed successfully.${RESET}"
else
    echo -e "${RED}❌ Installation failed.${RESET}"
    exit 1
fi

# Step 5: Verify the tool was installed successfully
echo -e "${BLUE}${BOLD}🔍 Verifying 'contool' installation...${RESET}"
if dotnet tool list --global | grep -q "contool"; then
    echo -e "${GREEN}✅ 'contool' installed successfully from local source.${RESET}"
    echo -e "${GREEN}📋 Tool details:${RESET}"
    dotnet tool list --global | grep "contool"
    echo ""
fi