# Steam Idle Manager

Aplicação simples e direta para gerenciar jogos em idle na Steam.

## Funcionalidades

- **Iniciar múltiplos jogos**: Digite os IDs dos jogos que deseja simular e clique em "Iniciar Jogos"
- **Parar todos os jogos**: Clique em "Parar Jogos" para encerrar todos os processos de idle
- **Monitoramento**: Visualize quais jogos estão rodando em tempo real

## Como usar

1. **Compile o projeto steam-idle primeiro** (necessário para gerar o `steam-idle.exe`)
2. Execute `SteamIdleManager.exe`
3. Digite os IDs dos jogos que deseja rodar (separados por vírgula, espaço ou quebra de linha)
   - Exemplo: `730, 440, 570`
   - Ou um por linha:
     ```
     730
     440
     570
     ```
4. Clique em **"Iniciar Jogos"**
5. Para parar todos os jogos, clique em **"Parar Jogos"**

## Requisitos

- Windows
- Steam instalado e rodando
- `steam-idle.exe` no mesmo diretório ou nos caminhos padrão
- .NET Framework 4.8

## Localização do steam-idle.exe

A aplicação procura o `steam-idle.exe` nos seguintes locais (nessa ordem):

1. Diretório atual
2. Diretório do executável
3. `../steam-idle Source/steam-idle/bin/Release/steam-idle.exe`
4. `../steam-idle Source/steam-idle/bin/Debug/steam-idle.exe`

## Notas

- A aplicação inicia os jogos com janelas ocultas
- Todos os processos steam-idle do usuário atual serão parados ao clicar em "Parar"
- A lista de jogos rodando é atualizada automaticamente a cada segundo

