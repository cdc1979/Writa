using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Writa.Models.Settings
{
    public interface ISettingsLoader
    {
        GlobalSettings LoadSettings();
        GlobalSettings SaveSettings(GlobalSettings s);
    }

    public interface IBlogSettingsLoader
    {
        WritaSettings LoadSettings();
        WritaSettings SaveSettings(WritaSettings s);
    }
}
