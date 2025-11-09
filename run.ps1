# Script para compilar e executar o SteamIdleManager
# Uso: .\run.ps1

param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Steam Idle Manager - Build & Run" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Encontrar MSBuild
$msbuild = $null
$vsPaths = @(
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
)

foreach ($path in $vsPaths) {
    if (Test-Path $path) {
        $msbuild = $path
        break
    }
}

if (-not $msbuild) {
    Write-Host "ERRO: MSBuild não encontrado!" -ForegroundColor Red
    Write-Host "Por favor, instale o Visual Studio ou Visual Studio Build Tools." -ForegroundColor Yellow
    exit 1
}

Write-Host "MSBuild encontrado: $msbuild" -ForegroundColor Gray
Write-Host ""

# 1. Compilar steam-idle (se necessário)
$steamIdleExe = "steam-idle Source\steam-idle\bin\$Configuration\steam-idle.exe"
if (-not (Test-Path $steamIdleExe)) {
    Write-Host "[1/3] Compilando steam-idle..." -ForegroundColor Yellow
    $steamIdleProj = "steam-idle Source\steam-idle\steam-idle.csproj"
    
    if (-not (Test-Path $steamIdleProj)) {
        Write-Host "AVISO: steam-idle.csproj não encontrado. Pulando compilação do steam-idle." -ForegroundColor Yellow
    } else {
        & $msbuild $steamIdleProj /p:Configuration=$Configuration /p:Platform=AnyCPU /v:minimal /nologo
        if ($LASTEXITCODE -ne 0) {
            Write-Host "ERRO: Falha ao compilar steam-idle!" -ForegroundColor Red
            exit 1
        }
        Write-Host "✓ steam-idle compilado" -ForegroundColor Green
        
        # Copiar DLLs necessárias para o diretório de saída do steam-idle
        $steamIdleOutputDir = "steam-idle Source\steam-idle\bin\$Configuration"
        
        # Copiar steam_api64.dll
        $dllSource = "Dependencies\steam_api64.dll"
        $dllTarget = "$steamIdleOutputDir\steam_api64.dll"
        if (Test-Path $dllSource) {
            Copy-Item $dllSource -Destination $dllTarget -Force
            Write-Host "✓ steam_api64.dll copiado para steam-idle" -ForegroundColor Gray
        }
        
        # Copiar Steamworks.NET.dll (necessário para o steam-idle funcionar)
        $steamworksSource = "Dependencies\Steamworks.NET.dll"
        $steamworksTarget = "$steamIdleOutputDir\Steamworks.NET.dll"
        if (Test-Path $steamworksSource) {
            Copy-Item $steamworksSource -Destination $steamworksTarget -Force
            Write-Host "✓ Steamworks.NET.dll copiado para steam-idle" -ForegroundColor Gray
        } else {
            Write-Host "AVISO: Steamworks.NET.dll não encontrado em Dependencies!" -ForegroundColor Yellow
        }
    }
    Write-Host ""
}

# 2. Compilar SteamIdleManager
Write-Host "[2/3] Compilando SteamIdleManager..." -ForegroundColor Yellow
$managerProj = "Source\SteamIdleManager\SteamIdleManager.csproj"

if (-not (Test-Path $managerProj)) {
    Write-Host "ERRO: SteamIdleManager.csproj não encontrado!" -ForegroundColor Red
    exit 1
}

& $msbuild $managerProj /p:Configuration=$Configuration /p:Platform=AnyCPU /v:minimal /nologo
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERRO: Falha ao compilar SteamIdleManager!" -ForegroundColor Red
    exit 1
}
Write-Host "✓ SteamIdleManager compilado" -ForegroundColor Green
Write-Host ""

# 3. Copiar arquivos necessários para o diretório de saída
Write-Host "[3/3] Preparando arquivos..." -ForegroundColor Yellow
$targetDir = "Source\SteamIdleManager\bin\$Configuration"
$steamIdleDir = "steam-idle Source\steam-idle\bin\$Configuration"

if (-not (Test-Path $targetDir)) {
    New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
}

# Copiar steam-idle.exe
$targetExe = "$targetDir\steam-idle.exe"
if (Test-Path $steamIdleExe) {
    Copy-Item $steamIdleExe -Destination $targetExe -Force
    Write-Host "✓ steam-idle.exe copiado" -ForegroundColor Green
} else {
    Write-Host "AVISO: steam-idle.exe não encontrado. A aplicação pode não funcionar." -ForegroundColor Yellow
}

# Copiar steam_api64.dll
$dllSource = "$steamIdleDir\steam_api64.dll"
if (-not (Test-Path $dllSource)) {
    $dllSource = "Dependencies\steam_api64.dll"
}
$dllTarget = "$targetDir\steam_api64.dll"
if (Test-Path $dllSource) {
    Copy-Item $dllSource -Destination $dllTarget -Force
    Write-Host "✓ steam_api64.dll copiado" -ForegroundColor Green
} else {
    Write-Host "AVISO: steam_api64.dll não encontrado!" -ForegroundColor Yellow
}

# Copiar Steamworks.NET.dll (OBRIGATÓRIO para steam-idle funcionar)
$steamworksSource = "$steamIdleDir\Steamworks.NET.dll"
if (-not (Test-Path $steamworksSource)) {
    $steamworksSource = "Dependencies\Steamworks.NET.dll"
}
$steamworksTarget = "$targetDir\Steamworks.NET.dll"
if (Test-Path $steamworksSource) {
    Copy-Item $steamworksSource -Destination $steamworksTarget -Force
    Write-Host "✓ Steamworks.NET.dll copiado" -ForegroundColor Green
} else {
    Write-Host "ERRO: Steamworks.NET.dll não encontrado! O steam-idle não funcionará." -ForegroundColor Red
}

# Verificar se há outras DLLs no diretório do steam-idle que precisam ser copiadas
$otherDlls = Get-ChildItem -Path $steamIdleDir -Filter "*.dll" -ErrorAction SilentlyContinue
foreach ($dll in $otherDlls) {
    $targetDll = "$targetDir\$($dll.Name)"
    if (-not (Test-Path $targetDll)) {
        Copy-Item $dll.FullName -Destination $targetDll -Force
        Write-Host "✓ $($dll.Name) copiado" -ForegroundColor Gray
    }
}

Write-Host ""

# 4. Executar
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Executando SteamIdleManager..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$exePath = "$targetDir\SteamIdleManager.exe"
if (Test-Path $exePath) {
    & $exePath
} else {
    Write-Host "ERRO: Executável não encontrado em: $exePath" -ForegroundColor Red
    exit 1
}

