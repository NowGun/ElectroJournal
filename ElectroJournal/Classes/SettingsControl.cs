using ElectroJournal.Classes.DataBaseEJ;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ElectroJournal.Classes
{
    internal class SettingsControl
    {
        public string Theme { get; set; }

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
    }
}
