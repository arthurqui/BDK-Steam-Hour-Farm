# Guia de Compilação - Steam Idle Manager

## Pré-requisitos

1. **Visual Studio 2017 ou superior** (recomendado) OU **MSBuild** (linha de comando)
2. **.NET Framework 4.8** instalado
3. **Steamworks.NET.dll** e **steam_api64.dll** (já estão na pasta `Dependencies`)

## Passo a Passo

### Opção 1: Compilar com Visual Studio (Recomendado)

#### 1. Compilar o steam-idle primeiro (OBRIGATÓRIO)

O `SteamIdleManager` precisa do `steam-idle.exe` para funcionar.

1. Abra o Visual Studio
2. Abra a solução `steam-idle.sln` em `steam-idle Source/steam-idle.sln`
3. Selecione a configuração **Release** (ou Debug para testes)
4. Clique em **Build > Build Solution** (ou pressione `Ctrl+Shift+B`)
5. O executável será gerado em:
   - `steam-idle Source/steam-idle/bin/Release/steam-idle.exe` (Release)
   - `steam-idle Source/steam-idle/bin/Debug/steam-idle.exe` (Debug)

#### 2. Compilar o SteamIdleManager

1. Abra a solução `IdleMasterExtended.sln` em `Source/IdleMasterExtended.sln`
2. No Solution Explorer, você verá dois projetos:
   - `IdleMasterExtended`
   - `SteamIdleManager` ← Este é o novo projeto
3. Clique com o botão direito em **SteamIdleManager** e selecione **Set as StartUp Project** (opcional)
4. Selecione a configuração **Release** (ou Debug)
5. Clique em **Build > Build Solution** (ou `Ctrl+Shift+B`)
6. O executável será gerado em:
   - `Source/SteamIdleManager/bin/Release/SteamIdleManager.exe` (Release)
   - `Source/SteamIdleManager/bin/Debug/SteamIdleManager.exe` (Debug)

#### 3. Copiar o steam-idle.exe (Opcional mas recomendado)

Para facilitar o uso, copie o `steam-idle.exe` para o mesmo diretório do `SteamIdleManager.exe`:

```
Source/SteamIdleManager/bin/Release/steam-idle.exe
```

A aplicação também procura automaticamente em outros caminhos, mas é mais fácil se estiver no mesmo diretório.

---

### Opção 2: Compilar com MSBuild (Linha de Comando)

#### 1. Compilar o steam-idle

Abra o **Developer Command Prompt for VS** ou **PowerShell** e execute:

```powershell
# Navegue até o diretório do projeto
cd "steam-idle Source\steam-idle"

# Compile em Release
msbuild steam-idle.csproj /p:Configuration=Release /p:Platform=AnyCPU

# Ou compile em Debug
msbuild steam-idle.csproj /p:Configuration=Debug /p:Platform=AnyCPU
```

O executável será gerado em `bin/Release/steam-idle.exe` ou `bin/Debug/steam-idle.exe`

#### 2. Compilar o SteamIdleManager

```powershell
# Navegue até o diretório do projeto
cd "Source\SteamIdleManager"

# Compile em Release
msbuild SteamIdleManager.csproj /p:Configuration=Release /p:Platform=AnyCPU

# Ou compile em Debug
msbuild SteamIdleManager.csproj /p:Configuration=Debug /p:Platform=AnyCPU
```

O executável será gerado em `bin/Release/SteamIdleManager.exe` ou `bin/Debug/SteamIdleManager.exe`

#### 3. Copiar o steam-idle.exe

```powershell
# Copie o steam-idle.exe para o diretório de saída
Copy-Item "..\..\steam-idle Source\steam-idle\bin\Release\steam-idle.exe" -Destination "bin\Release\steam-idle.exe"
```

---

## Compilação Rápida (Script PowerShell)

Crie um arquivo `compilar.ps1` no diretório raiz do projeto:

```powershell
# Compilar steam-idle
Write-Host "Compilando steam-idle..." -ForegroundColor Yellow
cd "steam-idle Source\steam-idle"
msbuild steam-idle.csproj /p:Configuration=Release /p:Platform=AnyCPU
if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro ao compilar steam-idle!" -ForegroundColor Red
    exit 1
}

# Compilar SteamIdleManager
Write-Host "Compilando SteamIdleManager..." -ForegroundColor Yellow
cd "..\..\Source\SteamIdleManager"
msbuild SteamIdleManager.csproj /p:Configuration=Release /p:Platform=AnyCPU
if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro ao compilar SteamIdleManager!" -ForegroundColor Red
    exit 1
}

# Copiar steam-idle.exe
Write-Host "Copiando steam-idle.exe..." -ForegroundColor Yellow
Copy-Item "..\..\steam-idle Source\steam-idle\bin\Release\steam-idle.exe" -Destination "bin\Release\steam-idle.exe" -Force

Write-Host "Compilação concluída com sucesso!" -ForegroundColor Green
Write-Host "Executável: Source\SteamIdleManager\bin\Release\SteamIdleManager.exe" -ForegroundColor Cyan
```

Execute com:
```powershell
.\compilar.ps1
```

---

## Verificação

Após compilar, verifique se os arquivos foram gerados:

- ✅ `Source/SteamIdleManager/bin/Release/SteamIdleManager.exe`
- ✅ `steam-idle Source/steam-idle/bin/Release/steam-idle.exe` (ou copiado para o diretório do SteamIdleManager)

---

## Solução de Problemas

### Erro: "steam-idle.exe não encontrado"

- Certifique-se de compilar o projeto `steam-idle` primeiro
- Copie o `steam-idle.exe` para o mesmo diretório do `SteamIdleManager.exe`
- Ou coloque o `steam-idle.exe` em um dos caminhos que a aplicação procura automaticamente

### Erro: "MSBuild não encontrado"

- Instale o Visual Studio Build Tools
- Ou use o Visual Studio completo
- Ou adicione o MSBuild ao PATH do sistema

### Erro: ".NET Framework 4.8 não encontrado"

- Baixe e instale o .NET Framework 4.8 Developer Pack
- Link: https://dotnet.microsoft.com/download/dotnet-framework/net48

---

## Estrutura de Diretórios Após Compilação

```
idle_master_extended/
├── Source/
│   └── SteamIdleManager/
│       └── bin/
│           └── Release/
│               ├── SteamIdleManager.exe  ← Executável principal
│               └── steam-idle.exe        ← Copiado manualmente (recomendado)
└── steam-idle Source/
    └── steam-idle/
        └── bin/
            └── Release/
                └── steam-idle.exe        ← Executável do steam-idle
```

