using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Writa.Models.Settings;
using MarkdownSharp;
namespace Writa.Models.Install
{
    public class InstallSet
    {
        public WritaPost homepage = new WritaPost();
        public WritaPost firstpost = new WritaPost();
        public WritaPostContent content = new WritaPostContent();
        public WritaUser user = new WritaUser();
        public WritaSettings settings = new WritaSettings();
    }

    public static class InstallHelper
    {
        public static InstallSet GetInstall()
        {
            InstallSet s = new InstallSet();

            var md = new MarkdownSharp.Markdown();
            var welcomemsg = "You have successfully installed _Writa_ - a publishing and blogging platform, that focuses on simplicity.  Built from the ground up to be *easy to customise* and with all the features you would expect.\n\n### Editing Posts \n\n Editing content is done using Markdown, a simple syntax for adding rich content to your pages.  You can add and edit pages using your admin control panel.  Checkout the help pages at <a href=\"http://writa.org\">http://writa.org</a> for more information.";
            var welcomemsghtml = md.Transform(welcomemsg);
            //s.u = new WritaUser() {  };
            s.homepage = new WritaPost() { PostStatus = WritaPostStatus.PUBLISHED, PostType = WritaPostType.HOMEPAGE, PostParent = "", PostSummary = "This is your blog homepage", PostId = System.Guid.NewGuid().ToString(), Homepage = true, PostContent = "", PostAuthor = "", PostCreated = DateTime.Now, PostLastEdited = DateTime.Now, PostStartDate = DateTime.Now, PostThumbnail = "", PostSlug = "homepage", PostTitle = "Welcome to Writa - Your Blog Homepage" };
            s.firstpost = new WritaPost() { PostStatus = WritaPostStatus.PUBLISHED, PostType = WritaPostType.BLOGPOST, PostParent = "", PostSummary = "Congratulations, you have just installed <b>Writa</b> - a beautifully simple and easy to use blogging platform built with customisation in mind from the ground up. ", PostStartDate = DateTime.Now, PostThumbnail = "/content/logo.jpg", PostId = System.Guid.NewGuid().ToString(), Homepage = true, PostContent = welcomemsg, PostAuthor = "", PostCreated = DateTime.Now, PostLastEdited = DateTime.Now, PostSlug = "welcome to writa", PostTitle = "Welcome to Writa" };
            s.settings = new WritaSettings() { SettingsId = System.Guid.NewGuid().ToString(), BlogTitle = "Welcome To Writa", BlogTheme = "Default", BlogSummary = "Welcome to Writa" };
            return s;
        }
    }
}
