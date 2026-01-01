# AztecQRGenerator.Core.Tests

## Overview
This directory contains comprehensive unit tests for the AztecQRGenerator.Core library using MSTest framework.

## ? Current Status

**Build:** ? SUCCESSFUL  
**Tests:** 67/69 PASSING (97.1%)  
**Coverage:** ~85-90% estimated  
**Last Run:** 2025-01-01

## Test Coverage

### Test Files Created
1. **QRGeneratorTests.cs** - 23/23 tests passing ?
   - Bitmap generation tests
   - File generation tests (PNG, JPEG, BMP)
   - Input validation tests
   - Error handling tests
   - Integration tests

2. **AztecGeneratorTests.cs** - 22/24 tests passing ??
   - Bitmap generation tests
   - File generation tests (PNG, JPEG, BMP)
   - Input validation tests (2 failing - see TEST_RESULTS.md)
   - Error handling tests
   - Integration tests
   - Cross-generator compatibility tests

3. **LoggerTests.cs** - 22/22 tests passing ?
   - Singleton pattern tests
   - Log level filtering tests
   - Thread safety tests
   - Exception logging tests
   - Edge case tests

4. **TestHelpers.cs** - Shared test utilities
   - Base64 test data generation
   - File cleanup utilities
   - Bitmap validation helpers
   - Test output directory management

## Setup Instructions

### Prerequisites
- .NET Framework 4.7.2 or higher
- Visual Studio 2019/2022 or MSBuild tools
- NuGet package manager

### Installation Steps

**The project is now fully configured and builds successfully!**

1. **Build the Solution:**
   ```powershell
   cd C:\Jobb\AztecQRGenerator\AztecQRGenerator.Core
   msbuild AztecQRGenerator.Core.Tests\AztecQRGenerator.Core.Tests.csproj /t:Restore,Build
   ```

2. **Run Tests:**
   
   Using Visual Studio Test Explorer:
   - Open Test Explorer (Test > Test Explorer)
   - Click "Run All"

   Using command line:
   ```powershell
   vstest.console.exe AztecQRGenerator.Core.Tests\bin\Debug\AztecQRGenerator.Core.Tests.dll
   ```

## Quick Start

```powershell
# Clone and navigate to project
cd C:\Jobb\AztecQRGenerator\AztecQRGenerator.Core

# Build everything (main project + tests)
msbuild /t:Restore,Build

# Run tests
vstest.console.exe AztecQRGenerator.Core.Tests\bin\Debug\AztecQRGenerator.Core.Tests.dll
```

## Test Results

For detailed test results, see [TEST_RESULTS.md](TEST_RESULTS.md)

**Summary:**
- ? **QRGenerator:** 100% passing (23/23)
- ?? **AztecGenerator:** 91.7% passing (22/24) - 2 validation tests failing
- ? **Logger:** 100% passing (22/22)

The 2 failing tests identified actual bugs in production code (missing null format validation).
