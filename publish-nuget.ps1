#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Publish PureSM NuGet package to NuGet.org
.DESCRIPTION
    This script publishes the PureSM NuGet package to NuGet.org
.PARAMETER ApiKey
    NuGet.org API key. If not provided, will look for NUGET_API_KEY environment variable
.PARAMETER PackageDir
    Directory containing the NuGet packages. Default: nupkgs
.PARAMETER SkipSymbols
    Skip publishing symbol packages (.snupkg)
.PARAMETER DryRun
    Simulate the publish without actually uploading
.PARAMETER Source
    NuGet source URL. Default: https://api.nuget.org/v3/index.json
.EXAMPLE
    .\publish-nuget.ps1 -ApiKey "your-api-key"
.EXAMPLE
    .\publish-nuget.ps1 -DryRun
.EXAMPLE
    # Using environment variable
    $env:NUGET_API_KEY = "your-api-key"
    .\publish-nuget.ps1
#>

param(
    [Parameter()]
    [string]$ApiKey,
    
    [Parameter()]
    [string]$PackageDir = 'nupkgs',
    
    [Parameter()]
    [switch]$SkipSymbols,
    
    [Parameter()]
    [switch]$DryRun,
    
    [Parameter()]
    [string]$Source = 'https://api.nuget.org/v3/index.json'
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

function Write-Warning-Message {
    param([string]$Message)
    Write-Host "⚠ $Message" -ForegroundColor Yellow
}

# Get script directory
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $ScriptDir

try {
    Write-Host "`n╔═══════════════════════════════════════════════════╗" -ForegroundColor Magenta
    Write-Host "║     PureSM NuGet Package Publish Script          ║" -ForegroundColor Magenta
    Write-Host "╚═══════════════════════════════════════════════════╝`n" -ForegroundColor Magenta

    # Get API Key
    if ([string]::IsNullOrWhiteSpace($ApiKey)) {
        $ApiKey = $env:NUGET_API_KEY
        if ([string]::IsNullOrWhiteSpace($ApiKey)) {
            Write-Error-Message "API key not provided!"
            Write-Host "`nPlease provide an API key using one of these methods:" -ForegroundColor Yellow
            Write-Host "  1. .\publish-nuget.ps1 -ApiKey 'your-api-key'" -ForegroundColor White
            Write-Host "  2. Set environment variable: `$env:NUGET_API_KEY = 'your-api-key'" -ForegroundColor White
            Write-Host "`nGet your API key from: https://www.nuget.org/account/apikeys`n" -ForegroundColor Cyan
            exit 1
        }
    }

    # Check if package directory exists
    if (-not (Test-Path $PackageDir)) {
        Write-Error-Message "Package directory not found: $PackageDir"
        Write-Host "Run .\build-package.ps1 first to create packages`n" -ForegroundColor Yellow
        exit 1
    }

    # Find packages
    $packages = Get-ChildItem $PackageDir -Filter "*.nupkg" -Exclude "*.snupkg"
    if ($packages.Count -eq 0) {
        Write-Error-Message "No packages found in $PackageDir"
        Write-Host "Run .\build-package.ps1 first to create packages`n" -ForegroundColor Yellow
        exit 1
    }

    # Display packages to be published
    Write-Host "Packages to publish:" -ForegroundColor Cyan
    foreach ($pkg in $packages) {
        $sizeKB = [math]::Round($pkg.Length / 1KB, 2)
        Write-Host "  📦 $($pkg.Name) - ${sizeKB} KB" -ForegroundColor White
    }

    # Confirmation (only in non-DryRun mode)
    if (-not $DryRun) {
        Write-Host "`nTarget: $Source" -ForegroundColor Cyan
        Write-Host "`n⚠ Are you sure you want to publish to NuGet.org?" -ForegroundColor Yellow
        $confirmation = Read-Host "Type 'yes' to continue"
        
        if ($confirmation -ne 'yes') {
            Write-Warning-Message "Publish cancelled by user"
            exit 0
        }
    } else {
        Write-Warning-Message "DRY RUN MODE - No packages will be published"
    }

    # Publish packages
    foreach ($pkg in $packages) {
        Write-Step "Publishing $($pkg.Name)..."
        
        if ($DryRun) {
            Write-Host "  [DRY RUN] Would execute:" -ForegroundColor Yellow
            Write-Host "  dotnet nuget push `"$($pkg.FullName)`" --api-key *** --source $Source --skip-duplicate" -ForegroundColor Gray
            Write-Success "Dry run completed for $($pkg.Name)"
        } else {
            try {
                dotnet nuget push "$($pkg.FullName)" --api-key $ApiKey --source $Source --skip-duplicate
                
                if ($LASTEXITCODE -eq 0) {
                    Write-Success "$($pkg.Name) published successfully"
                } else {
                    Write-Warning-Message "$($pkg.Name) may already exist (exit code: $LASTEXITCODE)"
                }
            } catch {
                Write-Error-Message "Failed to publish $($pkg.Name): $_"
                throw
            }
        }
    }

    # Publish symbol packages
    if (-not $SkipSymbols) {
        $symbolPackages = Get-ChildItem $PackageDir -Filter "*.snupkg" -ErrorAction SilentlyContinue
        
        if ($symbolPackages.Count -gt 0) {
            Write-Step "Publishing symbol packages..."
            
            foreach ($spkg in $symbolPackages) {
                if ($DryRun) {
                    Write-Host "  [DRY RUN] Would publish: $($spkg.Name)" -ForegroundColor Yellow
                } else {
                    try {
                        dotnet nuget push "$($spkg.FullName)" --api-key $ApiKey --source $Source --skip-duplicate
                        
                        if ($LASTEXITCODE -eq 0) {
                            Write-Success "$($spkg.Name) published successfully"
                        } else {
                            Write-Warning-Message "$($spkg.Name) may already exist (exit code: $LASTEXITCODE)"
                        }
                    } catch {
                        Write-Warning-Message "Failed to publish symbol package: $_"
                        # Don't throw on symbol package failure
                    }
                }
            }
        }
    }

    # Success message
    Write-Host "`n╔═══════════════════════════════════════════════════╗" -ForegroundColor Green
    Write-Host "║          Publish Completed Successfully!          ║" -ForegroundColor Green
    Write-Host "╚═══════════════════════════════════════════════════╝`n" -ForegroundColor Green

    if (-not $DryRun) {
        Write-Host "Your package will be available on NuGet.org shortly!" -ForegroundColor Cyan
        Write-Host "View your package at: https://www.nuget.org/packages/PureSM/`n" -ForegroundColor White
        Write-Host "Note: It may take a few minutes for the package to be indexed and searchable.`n" -ForegroundColor Yellow
    }

} catch {
    Write-Error-Message "Publish failed: $_"
    exit 1
}
