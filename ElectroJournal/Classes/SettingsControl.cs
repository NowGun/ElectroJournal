﻿using ElectroJournal.Classes.DataBaseEJ;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace ElectroJournal.Classes
{
    internal class SettingsControl
    {
        public string Theme { get; set; }
        private const string name = "ElectroJournal";

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
            else SetAutorunValue(false);
        }

        private bool SetAutorunValue(bool autorun)
        {
            string ExePath = System.Windows.Forms.Application.ExecutablePath;
            RegistryKey reg;
            reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");
            try
            {
                if (autorun)
                    reg.SetValue(name, ExePath);
                else
                    reg.DeleteValue(name);

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
                string username = Properties.Settings.Default.UserName;
                string password = Properties.Settings.Default.Password;

                ((MainWindow)Application.Current.MainWindow).TextBoxLogin.Text = username;
                ((MainWindow)Application.Current.MainWindow).TextBoxPassword.Password = password;
            }
        }
    }
}
