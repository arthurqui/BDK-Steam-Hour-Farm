using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Principal;
using System.Windows.Forms;

namespace SteamIdleManager
{
    public class ProcessInfo
    {
        public Process Process { get; set; }
        public long AppId { get; set; }
        public DateTime StartTime { get; set; }
        
        public TimeSpan ElapsedTime => DateTime.Now - StartTime;
        
        public string GetDisplayText()
        {
            TimeSpan elapsed = ElapsedTime;
            string timeStr = $"{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
            return $"AppID: {AppId} - Tempo: {timeStr}";
        }
    }

    public partial class frmMain : Form
    {
        private List<ProcessInfo> runningProcesses = new List<ProcessInfo>();
        private string steamIdlePath = "steam-idle.exe";

        public frmMain()
        {
            InitializeComponent();

            // Carregar ícone personalizado
            try
            {
                string iconPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "logo.ico");
                if (File.Exists(iconPath))
                {
                    this.Icon = new System.Drawing.Icon(iconPath);
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Erro ao carregar ícone: {ex.Message}");
            }

            // Carregar game IDs salvos
            LoadSavedGameIds();

            Logger.LogInfo("BDK Steam Hour Farm iniciado");
            Logger.LogInfo($"Diretório do executável: {Path.GetDirectoryName(Application.ExecutablePath)}");
            Logger.LogInfo($"Diretório atual: {Directory.GetCurrentDirectory()}");
            CheckSteamIdleExists();
            UpdateStatus();
        }

        private void CheckSteamIdleExists()
        {
            // Verifica se steam-idle.exe existe no diretório atual ou no diretório do executável
            string currentDir = Directory.GetCurrentDirectory();
            string exeDir = Path.GetDirectoryName(Application.ExecutablePath);
            
            string[] searchPaths = {
                Path.Combine(currentDir, steamIdlePath),
                Path.Combine(exeDir, steamIdlePath),
                Path.Combine(exeDir, "..", "steam-idle Source", "steam-idle", "bin", "Release", steamIdlePath),
                Path.Combine(exeDir, "..", "steam-idle Source", "steam-idle", "bin", "Debug", steamIdlePath)
            };

            bool found = false;
            foreach (string path in searchPaths)
            {
                if (File.Exists(path))
                {
                    steamIdlePath = path;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Logger.LogWarning("steam-idle.exe não encontrado em nenhum dos caminhos pesquisados");
                MessageBox.Show(
                    "steam-idle.exe não encontrado!\n\n" +
                    "Por favor, certifique-se de que steam-idle.exe está no mesmo diretório desta aplicação ou compile o projeto steam-idle primeiro.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            else
            {
                Logger.LogInfo($"steam-idle.exe encontrado em: {steamIdlePath}");
            }
        }

        private bool IsSteamRunning()
        {
            try
            {
                Process[] steamProcesses = Process.GetProcessesByName("steam");
                bool running = steamProcesses.Length > 0;
                Logger.LogDebug($"Steam rodando: {running} (processos encontrados: {steamProcesses.Length})");
                return running;
            }
            catch (Exception ex)
            {
                Logger.LogError("Erro ao verificar se Steam está rodando", ex);
                return false;
            }
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            // Salvar game IDs automaticamente quando iniciar
            SaveGameIds();

            string input = txtGameIds.Text.Trim();
            
            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Por favor, digite pelo menos um ID de jogo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Verifica se a Steam está rodando
            if (!IsSteamRunning())
            {
                DialogResult result = MessageBox.Show(
                    "A Steam não parece estar rodando!\n\n" +
                    "O steam-idle.exe precisa da Steam rodando para funcionar.\n\n" +
                    "Deseja continuar mesmo assim?",
                    "Steam não detectada",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            // Verifica se steam-idle.exe existe
            if (!File.Exists(steamIdlePath))
            {
                MessageBox.Show(
                    $"steam-idle.exe não encontrado em:\n{steamIdlePath}\n\n" +
                    "Por favor, certifique-se de que o arquivo existe.",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Parse dos IDs (separados por vírgula, ponto e vírgula, espaço ou quebra de linha)
            string[] separators = { ",", ";", " ", "\r\n", "\n", "\r" };
            string[] gameIds = input.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(id => id.Trim())
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .ToArray();

            if (gameIds.Length == 0)
            {
                MessageBox.Show("Nenhum ID de jogo válido encontrado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Limpa processos anteriores se necessário
            Logger.LogInfo("Limpando processos anteriores");
            PararTodosJogos();

            // Inicia os jogos
            Logger.LogInfo($"Iniciando {gameIds.Length} jogo(s): {string.Join(", ", gameIds)}");
            int sucessos = 0;
            int falhas = 0;
            List<string> mensagensErro = new List<string>();

            foreach (string gameId in gameIds)
            {
                if (long.TryParse(gameId, out long appId))
                {
                    try
                    {
                        // Define o diretório de trabalho como o diretório do steam-idle.exe
                        string workingDir = Path.GetDirectoryName(steamIdlePath);
                        Logger.LogInfo($"Iniciando jogo AppID {appId} - WorkingDir: {workingDir}");
                        
                        // Verifica se as DLLs necessárias existem
                        string dllSteamApi = Path.Combine(workingDir, "steam_api64.dll");
                        string dllSteamworks = Path.Combine(workingDir, "Steamworks.NET.dll");
                        string configFile = Path.Combine(workingDir, "steam-idle.exe.config");
                        
                        Logger.LogDebug($"Verificando DLLs - steam_api64.dll: {File.Exists(dllSteamApi)}, Steamworks.NET.dll: {File.Exists(dllSteamworks)}, steam-idle.exe.config: {File.Exists(configFile)}");
                        
                        // Copia app.config se não existir
                        if (!File.Exists(configFile))
                        {
                            string sourceConfig = Path.Combine(Path.GetDirectoryName(steamIdlePath), "..", "..", "..", "steam-idle Source", "steam-idle", "app.config");
                            if (File.Exists(sourceConfig))
                            {
                                File.Copy(sourceConfig, configFile, true);
                                Logger.LogInfo($"Copiado app.config para steam-idle.exe.config");
                            }
                            else
                            {
                                Logger.LogWarning($"app.config não encontrado em: {sourceConfig}");
                            }
                        }
                        
                        if (!File.Exists(dllSteamApi))
                        {
                            Logger.LogError($"steam_api64.dll não encontrada em: {dllSteamApi}");
                            mensagensErro.Add($"steam_api64.dll não encontrada em: {dllSteamApi}");
                            falhas++;
                            continue;
                        }
                        
                        if (!File.Exists(dllSteamworks))
                        {
                            Logger.LogError($"Steamworks.NET.dll não encontrada em: {dllSteamworks}");
                            mensagensErro.Add($"Steamworks.NET.dll não encontrada em: {dllSteamworks}");
                            falhas++;
                            continue;
                        }
                        
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = steamIdlePath,
                            Arguments = appId.ToString(),
                            WorkingDirectory = workingDir,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        };

                        Logger.LogDebug($"ProcessStartInfo - FileName: {startInfo.FileName}, Arguments: {startInfo.Arguments}, WorkingDirectory: {startInfo.WorkingDirectory}");
                        
                        Process process = Process.Start(startInfo);
                        if (process != null)
                        {
                            Logger.LogInfo($"Processo iniciado - PID: {process.Id}, AppID: {appId}");
                            
                            // Captura saída padrão e de erro de forma assíncrona
                            string output = "";
                            string error = "";
                            
                            process.OutputDataReceived += (s, evt) => {
                                if (!string.IsNullOrEmpty(evt.Data))
                                {
                                    output += evt.Data + Environment.NewLine;
                                    Logger.LogDebug($"[steam-idle stdout] {evt.Data}");
                                }
                            };
                            
                            process.ErrorDataReceived += (s, evt) => {
                                if (!string.IsNullOrEmpty(evt.Data))
                                {
                                    error += evt.Data + Environment.NewLine;
                                    Logger.LogError($"[steam-idle stderr] {evt.Data}");
                                }
                            };
                            
                            process.BeginOutputReadLine();
                            process.BeginErrorReadLine();
                            
                            // Aguarda um pouco para verificar se o processo não termina imediatamente
                            System.Threading.Thread.Sleep(1000);
                            
                            // Verifica se o processo ainda está rodando
                            if (process.HasExited)
                            {
                                // Processo terminou imediatamente - provavelmente falhou
                                int exitCode = process.ExitCode;
                                Logger.LogError($"Processo {process.Id} (AppID {appId}) terminou imediatamente. ExitCode: {exitCode} (0x{exitCode:X8})");
                                
                                // Aguarda um pouco para capturar toda a saída
                                process.WaitForExit(2000);
                                
                                if (!string.IsNullOrEmpty(output))
                                {
                                    Logger.LogInfo($"Saída padrão do processo: {output}");
                                }
                                
                                if (!string.IsNullOrEmpty(error))
                                {
                                    Logger.LogError($"Saída de erro do processo: {error}");
                                }
                                
                                string erroMsg = $"Jogo {appId} falhou ao iniciar. ";
                                
                                // Decodifica o exit code
                                if (exitCode == -532462766 || exitCode == unchecked((int)0xE0434352))
                                {
                                    erroMsg += "Erro: Exceção não tratada no .NET Framework (0xE0434352). ";
                                    erroMsg += "Isso geralmente indica que uma DLL necessária não foi encontrada ou houve um problema ao carregar dependências.\n\n";
                                    erroMsg += "Verifique:\n";
                                    erroMsg += "- steam_api64.dll está no mesmo diretório do steam-idle.exe\n";
                                    erroMsg += "- Steamworks.NET.dll está no mesmo diretório do steam-idle.exe\n";
                                    erroMsg += "- .NET Framework 4.8 está instalado\n";
                                    erroMsg += "- Todas as dependências do .NET estão disponíveis";
                                }
                                else if (exitCode != 0)
                                {
                                    erroMsg += $"Código de saída: {exitCode} (0x{exitCode:X8}). ";
                                }
                                
                                if (!string.IsNullOrEmpty(error))
                                {
                                    erroMsg += $"\nErro do processo:\n{error}";
                                }
                                
                                mensagensErro.Add(erroMsg);
                                process.Dispose();
                                falhas++;
                            }
                            else
                            {
                                Logger.LogInfo($"Processo {process.Id} (AppID {appId}) está rodando. Adicionado à lista.");
                                var processInfo = new ProcessInfo
                                {
                                    Process = process,
                                    AppId = appId,
                                    StartTime = DateTime.Now
                                };
                                runningProcesses.Add(processInfo);
                                sucessos++;
                            }
                        }
                        else
                        {
                            Logger.LogError($"Não foi possível iniciar o processo para o jogo {appId} - Process.Start retornou null");
                            mensagensErro.Add($"Não foi possível iniciar o processo para o jogo {appId}");
                            falhas++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Exceção ao iniciar jogo {appId}", ex);
                        mensagensErro.Add($"Erro ao iniciar jogo {appId}:\n{ex.Message}");
                        falhas++;
                    }
                }
                else
                {
                    Logger.LogWarning($"ID inválido: {gameId}");
                    mensagensErro.Add($"ID inválido: {gameId}");
                    falhas++;
                }
            }

            UpdateStatus();

            // Mostra mensagens de resultado
            if (sucessos > 0 && falhas == 0)
            {
                MessageBox.Show(
                    $"Jogos iniciados com sucesso!\n\n" +
                    $"Iniciados: {sucessos}",
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else if (sucessos > 0 && falhas > 0)
            {
                string mensagem = $"Alguns jogos foram iniciados.\n\n" +
                                $"Iniciados: {sucessos}\n" +
                                $"Falhas: {falhas}\n\n" +
                                "Detalhes dos erros:\n" +
                                string.Join("\n\n", mensagensErro);
                
                MessageBox.Show(mensagem, "Resultado Parcial", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (falhas > 0)
            {
                string mensagem = "Nenhum jogo foi iniciado com sucesso.\n\n" +
                                "Detalhes dos erros:\n" +
                                string.Join("\n\n", mensagensErro);
                
                MessageBox.Show(mensagem, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnParar_Click(object sender, EventArgs e)
        {
            PararTodosJogos();
            UpdateStatus();
            MessageBox.Show("Todos os jogos foram parados.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PararTodosJogos()
        {
            Logger.LogInfo($"Parando todos os jogos. Processos na lista: {runningProcesses.Count}");
            
            // Para processos iniciados por esta aplicação
            foreach (var processInfo in runningProcesses)
            {
                try
                {
                    if (!processInfo.Process.HasExited)
                    {
                        Logger.LogInfo($"Matando processo {processInfo.Process.Id}");
                        processInfo.Process.Kill();
                        processInfo.Process.WaitForExit(1000);
                        Logger.LogInfo($"Processo {processInfo.Process.Id} terminado");
                    }
                    else
                    {
                        Logger.LogDebug($"Processo {processInfo.Process.Id} já havia terminado");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Erro ao parar processo {processInfo.Process.Id}", ex);
                }
                finally
                {
                    processInfo.Process.Dispose();
                }
            }
            runningProcesses.Clear();
            Logger.LogInfo("Lista de processos limpa");

            // Para todos os processos steam-idle do usuário atual
            try
            {
                string username = WindowsIdentity.GetCurrent().Name;
                foreach (var process in Process.GetProcessesByName("steam-idle"))
                {
                    try
                    {
                        ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                            "Select * From Win32_Process Where ProcessID = " + process.Id);
                        ManagementObjectCollection processList = searcher.Get();

                        foreach (ManagementObject obj in processList)
                        {
                            string[] argList = new string[] { string.Empty, string.Empty };
                            int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                            if (returnVal == 0)
                            {
                                if (argList[1] + "\\" + argList[0] == username)
                                {
                                    process.Kill();
                                    process.WaitForExit(1000);
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Ignora erros
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao parar processos steam-idle: {ex.Message}");
            }

            lstJogosRodando.Items.Clear();
        }

        private void UpdateStatus()
        {
            int count = runningProcesses.Count(p => !p.Process.HasExited);
            int total = runningProcesses.Count;
            lblStatus.Text = $"Jogos rodando: {count}";
            btnParar.Enabled = count > 0;
            
            if (total > 0 && count != total)
            {
                Logger.LogDebug($"UpdateStatus: {count} rodando de {total} processos na lista");
            }
        }

        private void timerStatus_Tick(object sender, EventArgs e)
        {
            int processosAntes = runningProcesses.Count;
            
            // Remove processos que já terminaram da lista
            var processosTerminados = runningProcesses.Where(p => p.Process.HasExited).ToList();
            foreach (var processInfo in processosTerminados)
            {
                try
                {
                    int exitCode = processInfo.Process.ExitCode;
                    Logger.LogWarning($"Processo {processInfo.Process.Id} (AppID {processInfo.AppId}) terminou. ExitCode: {exitCode}, Tempo decorrido: {processInfo.ElapsedTime}");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Erro ao obter informações do processo {processInfo.Process.Id} que terminou", ex);
                }
            }
            
            runningProcesses.RemoveAll(p => p.Process.HasExited);
            
            int processosDepois = runningProcesses.Count;
            if (processosTerminados.Count > 0)
            {
                Logger.LogInfo($"Processos removidos: {processosTerminados.Count} (antes: {processosAntes}, depois: {processosDepois})");
            }
            
            // Atualiza a lista visual de forma otimizada (evita flickering)
            lstJogosRodando.BeginUpdate();
            try
            {
                // Limpa apenas se o número de itens mudou
                var processosRodando = runningProcesses.Where(p => !p.Process.HasExited).ToList();
                
                // Se o número de itens mudou, recria a lista
                if (lstJogosRodando.Items.Count != processosRodando.Count)
                {
                    lstJogosRodando.Items.Clear();
                    foreach (var processInfo in processosRodando)
                    {
                        lstJogosRodando.Items.Add(processInfo.GetDisplayText());
                    }
                }
                else
                {
                    // Atualiza apenas os textos (tempo decorrido) sem recriar a lista
                    for (int i = 0; i < processosRodando.Count && i < lstJogosRodando.Items.Count; i++)
                    {
                        string newText = processosRodando[i].GetDisplayText();
                        if (lstJogosRodando.Items[i].ToString() != newText)
                        {
                            lstJogosRodando.Items[i] = newText;
                        }
                    }
                }
            }
            finally
            {
                lstJogosRodando.EndUpdate();
            }

            UpdateStatus();
        }

        private string GetCommandLine(int processId)
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + processId);
                foreach (ManagementObject obj in searcher.Get())
                {
                    return obj["CommandLine"]?.ToString() ?? "";
                }
            }
            catch
            {
                // Ignora erros
            }
            return "";
        }

        private void LoadSavedGameIds()
        {
            try
            {
                string savedIds = Properties.Settings.Default.LastGameIds;
                if (!string.IsNullOrEmpty(savedIds))
                {
                    txtGameIds.Text = savedIds;
                    Logger.LogInfo("Game IDs salvos carregados com sucesso");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Erro ao carregar game IDs salvos", ex);
            }
        }

        private void SaveGameIds()
        {
            try
            {
                Properties.Settings.Default.LastGameIds = txtGameIds.Text.Trim();
                Properties.Settings.Default.Save();
                Logger.LogDebug("Game IDs salvos no disco");
            }
            catch (Exception ex)
            {
                Logger.LogError("Erro ao salvar game IDs", ex);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Salvar game IDs antes de fechar
            SaveGameIds();

            if (runningProcesses.Any(p => !p.Process.HasExited))
            {
                DialogResult result = MessageBox.Show(
                    "Há jogos rodando. Deseja parar todos antes de fechar?",
                    "Confirmar",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    PararTodosJogos();
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnFormClosing(e);
        }
    }
}

