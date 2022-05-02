using ElectroJournal.Classes.DataBaseEJ;
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace ElectroJournal.Classes
{
    internal class SettingsControl
    {
        public string? Theme { get; set; }
        private const string name = "ElectroJournal";
        string? currentPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        public async Task<bool> CheckVersionAsync(string version)
        {
            using (ejContext db = new ejContext())
            {
                var versionNew = await db.Versions.FirstOrDefaultAsync();

                return versionNew.VersionName == version ? true : false;
            }
        }
        public async Task<string> VersionAsync()
        {
            using (ejContext db = new ejContext())
            {
                var versionNew = await db.Versions.FirstOrDefaultAsync();

                return versionNew.VersionName;
            }
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
        public void CompletionLogin()
        {
            if (Properties.Settings.Default.RememberData)
            {
                ((MainWindow)Application.Current.MainWindow).TextBoxLogin.Text = Properties.Settings.Default.Login;
                ((MainWindow)Application.Current.MainWindow).TextBoxPassword.Password = Properties.Settings.Default.PassProfile;
            }
        }
        public async Task<bool> ExitUser()
        {
            using (zhirovContext db = new())
            {
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
    }
}
