# Fix for Duplicate Definition Errors

## Problem

The main project (`AztecQRGenerator.csproj`) is including files from BOTH:
1. The main project folder (correct)
2. The `AztecQRGenerator.Core` folder (incorrect - causes duplicates)

This causes CS0101 "already contains a definition" errors.

## Solution

Remove these lines from `AztecQRGenerator.csproj`:

```xml
<!-- REMOVE THESE LINES: -->
<Compile Include="AztecQRGenerator.Core\AztecGenerator.cs" />
<Compile Include="AztecQRGenerator.Core\Logger.cs" />
<Compile Include="AztecQRGenerator.Core\Properties\AssemblyInfo.cs" />
<Compile Include="AztecQRGenerator.Core\QRGenerator.cs" />
```

## Steps to Fix

1. **Close Visual Studio** (required to edit .csproj files)

2. **Open** `AztecQRGenerator.csproj` in a text editor (Notepad, VS Code, etc.)

3. **Find** the `<ItemGroup>` section with `<Compile Include=...` entries (around line 114)

4. **Remove** these 4 lines:
   ```xml
   <Compile Include="AztecQRGenerator.Core\AztecGenerator.cs" />
   <Compile Include="AztecQRGenerator.Core\Logger.cs" />
   <Compile Include="AztecQRGenerator.Core\Properties\AssemblyInfo.cs" />
   <Compile Include="AztecQRGenerator.Core\QRGenerator.cs" />
   ```

5. **Keep** these lines (your main project files):
   ```xml
   <Compile Include="QRGenerator.cs" />
   <Compile Include="AztecGenerator.cs" />
   <Compile Include="Logger.cs" />
   <Compile Include="Program.cs" />
   ```

6. **Save** the file

7. **Reopen** Visual Studio and rebuild

## Alternative: Use the Fixed File

See `AztecQRGenerator.csproj.fixed` in this directory for a corrected version.

## Verification

After fixing, run build - you should see **0 errors**.

The Core project files in `AztecQRGenerator.Core/` are meant to be built separately for NuGet packaging, not as part of the main solution.
