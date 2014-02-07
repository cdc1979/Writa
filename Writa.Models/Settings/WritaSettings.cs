using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Writa.Models.Settings
{
    /*
     * Wrapper to combined both kinds of settings
     */

    public class AllSettings
    {
        public GlobalSettings globalSettings { get; set; }
        public WritaSettings writaSettings { get; set; }
    }

    /*
     * Global settings are stored outside of any DB in App_Data/WritaSettings.json 
     * 
     */
    public class GlobalSettings
    {
        public DbType BlogDb { get; set; }
        public string DbConnectionString { get; set; }
        public string DbName { get; set; }
        public string LocalDbPath { get; set; }
        public EmailType BlogEmailMethod { get; set; }
        public string EmailServer { get; set; }
        public bool EmailUseSSL { get; set; }
        public string EmailFromAddress { get; set; }
        public bool EmailRequireAuth { get; set; }
        public string EmailUsername { get; set; }
        public string EmailPassword { get; set; }
    }
    public enum EmailType { SMTP, GMAIL, AMAZONSES, API }
    public enum DbType { MONGODB, EF, RAVENDB } //0 = mongodb, 1 = EF, 2 = Ravendb

    /*
     * These settings are stored in the DB. 
     */

    public class WritaSettings
    {
        public string SettingsId { get; set; }
        public string BlogTitle { get; set; }
        public string BlogSummary { get; set; }
        public string BlogDefaultEmail { get; set; }
        public string BlogTheme { get; set; }
    }

    
    
}
