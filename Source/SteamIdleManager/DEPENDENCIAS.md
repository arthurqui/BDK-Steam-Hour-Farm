# DLLs Necessárias para o steam-idle

O `steam-idle.exe` precisa das seguintes DLLs no mesmo diretório para funcionar:

## DLLs Obrigatórias

### 1. `steam_api64.dll`
- **Descrição**: DLL nativa da Steam API (64-bit)
- **Localização**: `Dependencies/steam_api64.dll`
- **Função**: Interface nativa com a Steam
- **Arquitetura**: x64 (64-bit)

### 2. `Steamworks.NET.dll`
- **Descrição**: Biblioteca .NET wrapper para a Steam API
- **Localização**: `Dependencies/Steamworks.NET.dll`
- **Função**: Permite que código C# se comunique com a Steam API
- **Arquitetura**: .NET Framework (gerenciada)

## Estrutura de Arquivos

Após compilar, o diretório de saída deve conter:

```
Source/SteamIdleManager/bin/Release/
├── SteamIdleManager.exe
├── steam-idle.exe
├── steam_api64.dll          ← Obrigatória
└── Steamworks.NET.dll       ← Obrigatória
```

## Como o Script Copia as DLLs

O script `run.ps1` automaticamente:

1. **Durante a compilação do steam-idle**:
   - Copia `steam_api64.dll` de `Dependencies/` para `steam-idle Source/steam-idle/bin/Release/`
   - Copia `Steamworks.NET.dll` de `Dependencies/` para `steam-idle Source/steam-idle/bin/Release/`

2. **Antes de executar o SteamIdleManager**:
   - Copia `steam-idle.exe` para `Source/SteamIdleManager/bin/Release/`
   - Copia `steam_api64.dll` para `Source/SteamIdleManager/bin/Release/`
   - Copia `Steamworks.NET.dll` para `Source/SteamIdleManager/bin/Release/`
   - Copia qualquer outra DLL encontrada no diretório do steam-idle

## Verificação Manual

Se o steam-idle não funcionar, verifique:

1. **steam_api64.dll existe?**
   ```powershell
   Test-Path "Source\SteamIdleManager\bin\Release\steam_api64.dll"
   ```

2. **Steamworks.NET.dll existe?**
   ```powershell
   Test-Path "Source\SteamIdleManager\bin\Release\Steamworks.NET.dll"
   ```

3. **Todas as DLLs estão no mesmo diretório do steam-idle.exe?**
   - O steam-idle.exe precisa encontrar as DLLs no mesmo diretório onde ele está sendo executado
   - O `WorkingDirectory` é definido no código para garantir isso

## Problemas Comuns

### Erro: "SteamAPI.Init() falhou"
- **Causa**: `steam_api64.dll` não encontrada ou Steam não está rodando
- **Solução**: Verifique se a DLL está no mesmo diretório do executável

### Erro: "Could not load file or assembly 'Steamworks.NET'"
- **Causa**: `Steamworks.NET.dll` não encontrada
- **Solução**: Verifique se a DLL está no mesmo diretório do executável

### Erro: "BadImageFormatException"
- **Causa**: Arquitetura incompatível (x86 vs x64)
- **Solução**: Certifique-se de usar `steam_api64.dll` (64-bit) e compilar para AnyCPU ou x64

## Notas Técnicas

- O `steam-idle.exe` é compilado para **AnyCPU** (pode rodar em 32 ou 64-bit)
- O `steam_api64.dll` é **64-bit** apenas
- O `Steamworks.NET.dll` é uma DLL gerenciada (.NET) e funciona em ambas as arquiteturas
- Em sistemas 32-bit, seria necessário `steam_api.dll` (32-bit), mas este projeto usa apenas 64-bit

