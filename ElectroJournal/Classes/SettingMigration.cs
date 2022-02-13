using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ElectroJournal.Classes
{
    internal class SettingMigration
    {
        

        public void SettingLoad()
        {
            try
            {
                try
                {
                    XmlDocument xmlDocument = new XmlDocument();

                    xmlDocument.Load("setting.xml");

                    Properties.Settings.Default.Server = xmlDocument.GetElementsByTagName("server")[0].InnerText;
                    Properties.Settings.Default.UserName = xmlDocument.GetElementsByTagName("username")[0].InnerText;
                    Properties.Settings.Default.Password = xmlDocument.GetElementsByTagName("password")[0].InnerText;
                    Properties.Settings.Default.Theme = Int32.Parse(xmlDocument.GetElementsByTagName("theme")[0].InnerText);
                    Properties.Settings.Default.StartEJ = 1;

                    Properties.Settings.Default.Save();

                    FileInfo fi = new FileInfo("setting.xml");

                    if (fi.Exists) fi.Delete();

                }
                catch (System.FormatException) { }
            } 
            catch (System.IO.FileNotFoundException)
            {

            }
        }

        public void CheckStart()
        {
            if (Properties.Settings.Default.StartEJ == 0)
            {
                try
                {
                    Process.Start("StartEJ.exe");

                    Properties.Settings.Default.StartEJ = 1;

                    Properties.Settings.Default.Save();

                    foreach (var process in Process.GetProcessesByName("ElectroJournal"))
                    {
                        process.Kill();
                    }

                }
                catch (System.ComponentModel.Win32Exception)
                {

                }


            }
            else SettingLoad();
        }
    }
}
