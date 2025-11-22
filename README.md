# BDK Steam Hour Farm

Uma aplicação simples e eficiente para gerenciar jogos em idle na Steam, permitindo ganhar horas de jogo automaticamente.

## 📥 Download

### **🚀 Versão Mais Recente: v1.1.0**
**[⬇️ Baixar BDK Steam Hour Farm v1.1.0](https://github.com/arthurqui/BDK-Steam-Hour-Farm/raw/main/Release/Release1.1.zip)** (935 KB)

**✨ Novidades v1.1.0:**
- 💾 **Persistência de códigos**: Os códigos dos jogos digitados são salvos automaticamente e carregados ao abrir o programa
- 🔄 **Experiência aprimorada**: Não precisa redigitar os códigos toda vez que abre o aplicativo

*Requisitos: Windows 10/11, .NET Framework 4.8, Steam instalada*

### **📦 Versões Anteriores**
- **[⬇️ Baixar BDK Steam Hour Farm v1.0.0](https://github.com/arthurqui/BDK-Steam-Hour-Farm/raw/main/Release/BDK_Steam_Hour_Farm_v1.0.0.zip)** (935 KB)

## Funcionalidades

- **Interface amigável** com ícone personalizado
- **Gerenciamento múltiplo** de jogos simultaneamente
- **Monitoramento em tempo real** com tempo de execução
- **Verificação automática** da Steam rodando
- **Logs detalhados** para diagnóstico
- **Sistema robusto** de detecção de falhas

## Requisitos

- **Windows 10/11**
- **.NET Framework 4.8** ou superior
- **Steam instalada** e configurada

## Como Compilar

### Opção 1: Script Automático (Recomendado)

```powershell
# Compilar tudo automaticamente
.\run.ps1
```

### Opção 2: Scripts Individuais

```powershell
# Compilar apenas
.\compilar.ps1

# Ou executar diretamente
.\criar-release.ps1
```

### Opção 3: Visual Studio

1. Abra `Source/IdleMasterExtended.sln`
2. Selecione projeto `SteamIdleManager`
3. Build → Build Solution
4. Execute `.\criar-release.ps1` para criar release

## Como Usar

1. Certifique-se de que a Steam está rodando
2. Execute `BDKSteamHourFarm.exe`
3. Digite os IDs dos jogos separados por vírgula
4. Clique em "Iniciar Jogos"
5. Veja o progresso em tempo real

### Exemplo

```
Digite: 730, 440, 570
```

## Arquitetura

O projeto consiste em dois componentes principais:

- **BDKSteamHourFarm.exe** - Interface principal em C#
- **steam-idle.exe** - Processo auxiliar que simula jogos

## Logs e Diagnóstico

- `steam_idle_manager.log` - Log da aplicação principal
- `steam-idle-error.log` - Log de erros do steam-idle

## Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature
3. Faça commit das mudanças
4. Push para a branch
5. Abra um Pull Request

## Licença

Este projeto é distribuído sob a licença MIT.

## Suporte

Para suporte ou dúvidas, abra uma issue no GitHub.