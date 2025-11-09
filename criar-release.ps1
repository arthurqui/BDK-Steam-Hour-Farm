# Script para criar pasta de release com apenas os arquivos essenciais

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Criando Release - BDK Steam Hour Farm" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$sourceDir = "Source\SteamIdleManager\bin\Release"
$releaseDir = "Release\BDK Steam Hour Farm"

if (-not (Test-Path $sourceDir)) {
    Write-Host "ERRO: Diretorio de origem nao encontrado!" -ForegroundColor Red
    Write-Host "Por favor, compile o projeto primeiro." -ForegroundColor Yellow
    exit 1
}

if (Test-Path $releaseDir) {
    Write-Host "Removendo diretorio de release existente..." -ForegroundColor Yellow
    Remove-Item $releaseDir -Recurse -Force
}

Write-Host "Criando diretorio de release..." -ForegroundColor Yellow
New-Item -ItemType Directory -Path $releaseDir -Force | Out-Null

Write-Host ""
Write-Host "Copiando arquivos essenciais..." -ForegroundColor Yellow

$arquivos = @("BDKSteamHourFarm.exe", "steam-idle.exe", "steam-idle.exe.config", "steam_api64.dll", "Steamworks.NET.dll", "logo.ico")

foreach ($arquivo in $arquivos) {
    $sourcePath = Join-Path $sourceDir $arquivo
    if (Test-Path $sourcePath) {
        $destPath = Join-Path $releaseDir $arquivo
        Copy-Item $sourcePath -Destination $destPath -Force
        Write-Host "  OK: $arquivo" -ForegroundColor Green
    } else {
        Write-Host "  ERRO: $arquivo (NAO ENCONTRADO)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Release Criado!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Localizacao: $releaseDir" -ForegroundColor Cyan
Write-Host ""

Get-ChildItem $releaseDir | Format-Table Name, Length -AutoSize

$tamanhoTotal = (Get-ChildItem $releaseDir -Recurse | Measure-Object -Property Length -Sum).Sum
$tamanhoMB = [math]::Round($tamanhoTotal / 1MB, 2)
Write-Host "Tamanho total: $tamanhoMB MB" -ForegroundColor Cyan
Write-Host ""
