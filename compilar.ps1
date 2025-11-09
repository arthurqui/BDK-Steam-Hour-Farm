# Script de Compilação - Steam Idle Manager
# Execute este script no diretório raiz do projeto

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Compilando Steam Idle Manager" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se MSBuild está disponível
$msbuild = Get-Command msbuild -ErrorAction SilentlyContinue
if (-not $msbuild) {
    # Tentar encontrar MSBuild no Visual Studio
    $vsPath = "${env:ProgramFiles}\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
    if (Test-Path $vsPath) {
        $msbuild = $vsPath
    } else {
        $vsPath = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
        if (Test-Path $vsPath) {
            $msbuild = $vsPath
        } else {
            Write-Host "ERRO: MSBuild não encontrado!" -ForegroundColor Red
            Write-Host "Por favor, instale o Visual Studio ou Visual Studio Build Tools." -ForegroundColor Yellow
            exit 1
        }
    }
}

# 1. Compilar steam-idle
Write-Host "[1/3] Compilando steam-idle..." -ForegroundColor Yellow
$steamIdlePath = "steam-idle Source\steam-idle\steam-idle.csproj"
if (-not (Test-Path $steamIdlePath)) {
    Write-Host "ERRO: Arquivo steam-idle.csproj não encontrado!" -ForegroundColor Red
    exit 1
}

& $msbuild $steamIdlePath /p:Configuration=Release /p:Platform=AnyCPU /v:minimal /nologo
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERRO: Falha ao compilar steam-idle!" -ForegroundColor Red
    exit 1
}
Write-Host "✓ steam-idle compilado com sucesso" -ForegroundColor Green
Write-Host ""

# 2. Compilar SteamIdleManager
Write-Host "[2/3] Compilando SteamIdleManager..." -ForegroundColor Yellow
$managerPath = "Source\SteamIdleManager\SteamIdleManager.csproj"
if (-not (Test-Path $managerPath)) {
    Write-Host "ERRO: Arquivo SteamIdleManager.csproj não encontrado!" -ForegroundColor Red
    exit 1
}

& $msbuild $managerPath /p:Configuration=Release /p:Platform=AnyCPU /v:minimal /nologo
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERRO: Falha ao compilar SteamIdleManager!" -ForegroundColor Red
    exit 1
}
Write-Host "✓ SteamIdleManager compilado com sucesso" -ForegroundColor Green
Write-Host ""

# 3. Copiar steam-idle.exe
Write-Host "[3/3] Copiando steam-idle.exe..." -ForegroundColor Yellow
$sourceExe = "steam-idle Source\steam-idle\bin\Release\steam-idle.exe"
$targetDir = "Source\SteamIdleManager\bin\Release"
$targetExe = "$targetDir\steam-idle.exe"

if (Test-Path $sourceExe) {
    if (-not (Test-Path $targetDir)) {
        New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
    }
    Copy-Item $sourceExe -Destination $targetExe -Force
    Write-Host "✓ steam-idle.exe copiado com sucesso" -ForegroundColor Green
} else {
    Write-Host "AVISO: steam-idle.exe não encontrado em $sourceExe" -ForegroundColor Yellow
    Write-Host "      Você precisará copiar manualmente ou colocar no mesmo diretório do executável." -ForegroundColor Yellow
}
Write-Host ""

# Resumo
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Compilação Concluída!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Executável gerado em:" -ForegroundColor White
Write-Host "  $targetDir\SteamIdleManager.exe" -ForegroundColor Cyan
Write-Host ""
Write-Host "Para executar:" -ForegroundColor White
Write-Host "  cd $targetDir" -ForegroundColor Yellow
Write-Host "  .\SteamIdleManager.exe" -ForegroundColor Yellow
Write-Host ""

