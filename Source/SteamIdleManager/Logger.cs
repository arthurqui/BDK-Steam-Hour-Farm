using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace SteamIdleManager
{
    public static class Logger
    {
        private static readonly object LogLock = new object();
        private static readonly string LogPath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "steam_idle_manager.log");

        public static void Log(string message)
        {
            var contents = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {message}{Environment.NewLine}";
            Write(contents);
        }

        public static void LogError(string message, Exception ex = null)
        {
            var contents = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - [ERRO] {message}";
            if (ex != null)
            {
                contents += $"{Environment.NewLine}{ex.ToString()}";
            }
            contents += Environment.NewLine;
            Write(contents);
        }

        public static void LogWarning(string message)
        {
            var contents = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - [AVISO] {message}{Environment.NewLine}";
            Write(contents);
        }

        public static void LogInfo(string message)
        {
            var contents = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - [INFO] {message}{Environment.NewLine}";
            Write(contents);
        }

        public static void LogDebug(string message)
        {
            var contents = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - [DEBUG] {message}{Environment.NewLine}";
            Write(contents);
        }

        private static void Write(string contents)
        {
            Console.WriteLine(contents.TrimEnd());
            try
            {
                lock (LogLock)
                {
                    File.AppendAllText(LogPath, contents, Encoding.UTF8);
                }
            }
            catch
            {
                // Ignora erros de escrita no log
            }
        }

        public static void ClearLog()
        {
            try
            {
                if (File.Exists(LogPath))
                {
                    File.Delete(LogPath);
                }
            }
            catch
            {
                // Ignora erros
            }
        }
    }
}

