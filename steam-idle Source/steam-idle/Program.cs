﻿using System;
using System.IO;
using System.Windows.Forms;
using Steamworks;

namespace steam_idle
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                long appId = long.Parse(args[0]);
                Environment.SetEnvironmentVariable("SteamAppId", appId.ToString());

                if (!SteamAPI.Init())
                {
                    // Log erro se possível
                    try
                    {
                        File.AppendAllText("steam-idle-error.log", 
                            $"{DateTime.Now} - SteamAPI.Init() falhou para AppID {appId}\n");
                    }
                    catch { }
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                
                var form = new FormSteamIdle(appId);
                Application.Run(form);
            }
            catch (Exception ex)
            {
                // Log exceção se possível
                try
                {
                    File.AppendAllText("steam-idle-error.log", 
                        $"{DateTime.Now} - Exceção: {ex.ToString()}\n");
                }
                catch { }
            }
        }
    }
}