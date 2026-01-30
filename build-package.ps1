#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Build and pack PureSM NuGet package
.DESCRIPTION
    This script cleans, restores, builds, tests, and packs the PureSM library
.PARAMETER Configuration
    Build configuration (Debug or Release). Default: Release
.PARAMETER SkipTests
    Skip running tests before packing
.PARAMETER OutputDir
    Output directory for NuGet packages. Default: nupkgs
.EXAMPLE
    .\build-package.ps1
.EXAMPLE
    .\build-package.ps1 -Configuration Debug -SkipTests
#>

param(
    [Parameter()]
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    
    [Parameter()]
    [switch]$SkipTests,
    
    [Parameter()]
    [string]$OutputDir = 'nupkgs'
)

$ErrorActionPreference = 'Stop'

# Colors for output
function Write-Step {
    param([string]$Message)
    Write-Host "`n===> $Message" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "✓ $Message" -ForegroundColor Green
}

function Write-Error-Message {
    param([string]$Message)
    Write-Host "✗ $Message" -ForegroundColor Red
}

# Get script directory
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $ScriptDir

try {
    Write-Host "`n╔═══════════════════════════════════════════════════╗" -ForegroundColor Magenta
    Write-Host "║     PureSM NuGet Package Build Script            ║" -ForegroundColor Magenta
    Write-Host "╚═══════════════════════════════════════════════════╝`n" -ForegroundColor Magenta

    # Step 1: Clean
    Write-Step "Cleaning previous builds..."
    dotnet clean --configuration $Configuration
    if (Test-Path $OutputDir) {
        Remove-Item $OutputDir -Recurse -Force
    }
    Write-Success "Clean completed"

    # Step 2: Restore
    Write-Step "Restoring dependencies..."
    dotnet restore src/PureSM/PureSM.csproj
    Write-Success "Dependencies restored"

    # Step 3: Build
    Write-Step "Building project ($Configuration)..."
    dotnet build src/PureSM/PureSM.csproj --configuration $Configuration --no-restore
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed with exit code $LASTEXITCODE"
    }
    Write-Success "Build completed"

    # Step 4: Test (optional)
    if (-not $SkipTests) {
        Write-Step "Running tests..."
        dotnet test tests/PureSM.Tests/PureSM.Tests.csproj --configuration $Configuration --no-build --verbosity normal
        if ($LASTEXITCODE -ne 0) {
            throw "Tests failed with exit code $LASTEXITCODE"
        }
        Write-Success "All tests passed"
    } else {
        Write-Host "`n⚠ Skipping tests" -ForegroundColor Yellow
    }

    # Step 5: Pack
    Write-Step "Creating NuGet package..."
    dotnet pack src/PureSM/PureSM.csproj --configuration $Configuration --no-build --output $OutputDir
    if ($LASTEXITCODE -ne 0) {
        throw "Pack failed with exit code $LASTEXITCODE"
    }
    Write-Success "Package created"

    # Display results
    Write-Host "`n╔═══════════════════════════════════════════════════╗" -ForegroundColor Green
    Write-Host "║            Build Completed Successfully!          ║" -ForegroundColor Green
    Write-Host "╚═══════════════════════════════════════════════════╝`n" -ForegroundColor Green

    Write-Host "Generated packages:" -ForegroundColor Cyan
    Get-ChildItem $OutputDir -Filter "*.nupkg" | ForEach-Object {
        $sizeKB = [math]::Round($_.Length / 1KB, 2)
        Write-Host "  📦 $($_.Name) - ${sizeKB} KB" -ForegroundColor White
    }

    Write-Host "`nNext steps:" -ForegroundColor Cyan
    Write-Host "  1. Review the package contents" -ForegroundColor White
    Write-Host "  2. Run .\publish-nuget.ps1 to publish to NuGet.org`n" -ForegroundColor White

} catch {
    Write-Error-Message "Build failed: $_"
    exit 1
}
