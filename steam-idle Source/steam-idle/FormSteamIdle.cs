using System;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using Steamworks;

namespace steam_idle
{
    public partial class FormSteamIdle : Form
    {
        private System.Timers.Timer steamTimer;
        private long appId;

        public FormSteamIdle(long appid)
        {
            this.appId = appid;
            InitializeComponent();
            
            // Previne que o formulário seja fechado acidentalmente
            this.FormClosing += FormSteamIdle_FormClosing;
            
            // Tenta carregar a imagem, mas não falha se não conseguir
            try
            {
                picApp.Load($"https://cdn.akamai.steamstatic.com/steam/apps/{appid}/header_292x136.jpg");
            }
            catch (Exception ex)
            {
                // Ignora erro ao carregar imagem - não é crítico
                try
                {
                    File.AppendAllText("steam-idle-error.log", 
                        $"{DateTime.Now} - Erro ao carregar imagem para AppID {appid}: {ex.Message}\n");
                }
                catch { }
            }
            
            // Timer para manter a Steam API ativa
            steamTimer = new System.Timers.Timer(5000); // 5 segundos
            steamTimer.Elapsed += SteamTimer_Elapsed;
            steamTimer.AutoReset = true;
            steamTimer.Start();
        }

        private void SteamTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                // Mantém a Steam API ativa chamando RunCallbacks periodicamente
                SteamAPI.RunCallbacks();
            }
            catch (Exception ex)
            {
                // Se a Steam API falhar, tenta reinicializar
                try
                {
                    File.AppendAllText("steam-idle-error.log", 
                        $"{DateTime.Now} - Erro ao executar SteamAPI.RunCallbacks(): {ex.Message}\n");
                    
                    // Tenta reinicializar
                    if (!SteamAPI.Init())
                    {
                        // Se não conseguir reinicializar, fecha o formulário
                        this.Invoke((MethodInvoker)delegate {
                            this.Close();
                        });
                    }
                }
                catch { }
            }
        }

        private void FormSteamIdle_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Permite fechar normalmente
            if (steamTimer != null)
            {
                steamTimer.Stop();
                steamTimer.Dispose();
            }
        }

        private void FormSteamIdle_Load(object sender, EventArgs e)
        {
            // Garante que o formulário está visível (mesmo que minimizado)
            // Isso é necessário para manter a aplicação rodando
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }
    }
}
