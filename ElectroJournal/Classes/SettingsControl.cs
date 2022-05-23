using ElectroJournal.Classes.DataBaseEJ;
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ElectroJournal.Classes
{
    internal class SettingsControl
    {
        public string? Theme { get; set; }
        private const string name = "ElectroJournal";
        private bool _isDarkTheme = false;
        string? currentPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        public static async Task<bool> CheckVersionAsync(string version)
        {
            using ejContext db = new();
            var versionNew = await db.Versions.FirstOrDefaultAsync();
            return versionNew.VersionName == version ? true : false;
        }
        public static async Task<string> VersionAsync()
        {
            using ejContext db = new();
            var versionNew = await db.Versions.FirstOrDefaultAsync();
            return versionNew.VersionName;
        }
        public void CheckAutoRun()
        {
            bool autorun = Properties.Settings.Default.AutoRun;

            if (autorun)
            {
                SetAutorunValue(true);
            }
            else
            {
                SetAutorunValue(false);
            }
        }
        private bool SetAutorunValue(bool autorun)
        {
            string ExePath = System.Windows.Forms.Application.ExecutablePath;
            RegistryKey reg;
            reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");
            try
            {
                if (autorun)
                {
                    reg.SetValue(name, ExePath);
                }
                else
                {
                    reg.DeleteValue(name);
                }

                reg.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool CheckTray()
        {
            bool tray = Properties.Settings.Default.Tray;
            return tray;
        }
        public (string login, string pass) CompletionLogin()
        {
            if (Properties.Settings.Default.RememberData) return (Properties.Settings.Default.Login, Properties.Settings.Default.PassProfile);
            else return ("", "");
        }
        public async Task<bool> ExitUser()
        {
            using zhirovContext db = new();

            bool isAvalaible = await db.Database.CanConnectAsync();
            if (isAvalaible)
            {
                Teacher? teacher = await db.Teachers.FirstOrDefaultAsync(p => p.Idteachers == Properties.Settings.Default.UserID);

                if (teacher != null)
                {
                    teacher.TeachersStatus = 0;
                }

                await db.SaveChangesAsync();

                return true;
            }
            return false;
        }
        public void LogFileCreate()
        {
            DateTime now = DateTime.Now;
            string path = $@"{currentPath}/logs/{now:d}.txt";

            if (!File.Exists(path))
            {
                File.Create(path);
            }
        }
        public void CreateDirLogs()
        {
            string path = $@"{currentPath}/logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        public async void InputLog(string text)
        {
            DateTime now = DateTime.Now;
            await File.AppendAllTextAsync($@"{currentPath}/logs/{now:d}.txt", $"{text} | {now:T}\n");
        }
        public void ChangeTheme()
        {
            int theme = Properties.Settings.Default.Theme;

            _isDarkTheme = theme == 1;

            var newTheme = theme == 0
            ? WPFUI.Appearance.ThemeType.Light
            : WPFUI.Appearance.ThemeType.Dark;

            if (Environment.OSVersion.Version.Build >= 22000)
            {
                WPFUI.Appearance.Theme.Apply(
           themeType: newTheme,
           backgroundEffect: WPFUI.Appearance.BackgroundType.Mica,
           updateAccent: true,
           forceBackground: false);
            }
            else
            {
                WPFUI.Appearance.Theme.Apply(
           themeType: newTheme,
           updateAccent: true,
           forceBackground: false);
            }
        }
        public static string Hash(string password)
        {
            MD5 md5hasher = MD5.Create();

            var data = md5hasher.ComputeHash(Encoding.Default.GetBytes(password));

            return Convert.ToBase64String(data);
        }
    }
}
