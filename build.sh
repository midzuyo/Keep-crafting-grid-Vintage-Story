#!/bin/bash
set -e

PROJECT_NAME="keepcraftinggrid"
DLL_NAME="keepcraftinggrid.dll"

RED='\033[0;31m'
GREEN='\033[0;32m'
NC='\033[0m'


print_success() {
    echo -e "${GREEN}✓${NC} $1"
}

print_error() {
    echo -e "${RED}✗${NC} $1"
}


rm -rf bin/Release
rm -rf obj

if dotnet build -c Release; then
    print_success "Compiled"
else
    print_error "Compilation error"
    exit 1
fi

if [ ! -f "bin/Release/$DLL_NAME" ]; then
    print_error "File bin/Release/$DLL_NAME not found"
    exit 1
fi

VERSION=$(grep -oP '(?<=<Version>)[^<]+' keepcraftinggrid.csproj)
ARCHIVE="bin/${PROJECT_NAME}-${VERSION}.zip"

cd bin/Release
zip -q -r "../../$ARCHIVE" * -x "*.deps.json"
cd ../..


ls -lh "$ARCHIVE"

rm -rf obj
echo "Done!"
