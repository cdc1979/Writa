using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Writa.Models.Settings;
using Writa.Models.Stats;
namespace Writa.Models
{
    public interface IDataHelper
    {

        bool CheckInstall(GlobalSettings s);

        WritaPost CreatePost(WritaPost p);
        void DeletePost(WritaPost p);
        WritaPost UpdatePost(WritaPost p);
        // Gets posts that are "Active" i.e. with the status of PUBSLIHED, and Start Date > Current Date.
        IQueryable<WritaPost> GetPosts();
        IQueryable<WritaPost> GetAllPosts();
        IQueryable<WritaPost> GetAllPosts(string tagid);
        WritaPost GetPostFromSlug(string slug);
        WritaPost GetPostFromId(string id);
        WritaPost GetNextPost(WritaPost p);
        WritaPost GetPreviousPost(WritaPost p);
        bool DeletePost(string postid);
        bool DeleteAllPosts();

        //user functions
        WritaUser CreateUser(WritaUser u);
        void DeleteUser(WritaUser u);
        WritaUser UpdateUser(WritaUser u);
        WritaUser LogonUser(WritaUser u);
        WritaUser GetUserByEmail(WritaUser u);
        WritaUser GetUserById(WritaUser u);
        IQueryable<WritaUser> GetUsers();

        //plugin functions
        //List<IWritaPlugin> GetEnabledPlugins();
        WritaPluginSetting GetPluginSettings(string PluginName, string Key, string DefaultValue);
        void UpdatePluginValue(string sid, string value);
        void DeletePluginValue(string sid);
        IQueryable<WritaPluginSetting> GetPluginSettings();

        WritaStats GetStats();

    }
}
