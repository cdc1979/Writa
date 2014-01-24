using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Writa.Models;
using Writa.Models.Install;
using Writa.Models.Settings;
using Writa.Models.Stats;

namespace Writa.Data
{
    public class WritaBlogContext : DbContext
    {

        public WritaBlogContext() : base("Writa")
        {

        }

        public DbSet<WritaPost> WritaPosts { get; set; }
        public DbSet<WritaUser> WritaUsers { get; set; }
        public DbSet<WritaPostContent> WritaPostContents { get; set; }
        public DbSet<WritaSettings> WritaSettings { get; set; }
        public DbSet<WritaPluginSetting> WritaPluginSettings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<WritaPost>().HasKey(w => w.PostId);
            modelBuilder.Entity<WritaUser>().HasKey(w => w.Id);
            modelBuilder.Entity<WritaPostContent>().HasKey(w => w.PostContentId);
            modelBuilder.Entity<WritaSettings>().HasKey(w => w.SettingsId);
            modelBuilder.Entity<WritaPluginSetting>().HasKey(w => w.Id);
            
        }
    }

    public class EfDataHelper : IDataHelper, IBlogSettingsLoader
    {

        public GlobalSettings _settings;

        public EfDataHelper(GlobalSettings s)
        {
            _settings = s;
        }

        public WritaSettings LoadSettings()
        {
            using (var db = new WritaBlogContext())
            {
                return db.WritaSettings.Take(1).SingleOrDefault();
            }
        }

        public WritaSettings SaveSettings(WritaSettings s)
        {
            using (var db = new WritaBlogContext())
            {
                var x = db.WritaSettings.Take(1).SingleOrDefault();
                x.BlogTheme = s.BlogTheme;
                x.BlogSummary = s.BlogSummary;
                x.BlogTitle = s.BlogTitle;
                x.BlogDefaultEmail = s.BlogDefaultEmail;
                db.SaveChanges();
            }

            return s;
        }

        public void CheckInstall(GlobalSettings s)
        {
            using (var db = new WritaBlogContext())
            {
                if (db.WritaPosts.Count() == 0)
                {
                    InstallSet set = InstallHelper.GetInstall();
                    CreatePost(set.homepage);
                    CreatePost(set.firstpost);
                    db.WritaSettings.Add(set.settings);
                    db.SaveChanges();
                    //install.
                }
            }
        }

        public WritaPost CreatePost(WritaPost p)
        {
            using (var db = new WritaBlogContext())
            {

                p.PostId = System.Guid.NewGuid().ToString();
                p.PostCreated = DateTime.Now;
                p.PostLastEdited = DateTime.Now;
                p.PostSlug = p.PostTitle.ToLower().Replace(" ", "-");
                db.WritaPosts.Add(p);
                db.SaveChanges();

                WritaPostContent c = new WritaPostContent();
                c.PostContentId = System.Guid.NewGuid().ToString();
                c.PostId = p.PostId;
                c.PostEditDate = DateTime.Now;
                c.PostEditAuthor = p.PostAuthor;
                c.PostMarkdown = p.PostContent;

                db.WritaPostContents.Add(c);
                db.SaveChanges();
                return p;
            }
        }

        public void DeletePost(WritaPost p)
        {
            throw new NotImplementedException();
        }

        public WritaPost UpdatePost(WritaPost p)
        {
            throw new NotImplementedException();
        }

        public IQueryable<WritaPost> GetPosts()
        {
            var db = new WritaBlogContext();
            return db.WritaPosts.AsQueryable().Where(w => w.PostType == WritaPostType.BLOGPOST).AsQueryable();
        }

        public IQueryable<WritaPost> GetAllPosts()
        {
            var db = new WritaBlogContext();
            return db.WritaPosts.AsQueryable();
        }

        public IQueryable<WritaPost> GetAllPosts(string tagid)
        {
            throw new NotImplementedException();
        }

        public WritaPost GetPostFromSlug(string slug)
        {
            var db = new WritaBlogContext();
            return db.WritaPosts.AsQueryable().Where(w => w.PostSlug == slug.ToLower()).SingleOrDefault();
        }

        public WritaPost GetPostFromId(string id)
        {
            var db = new WritaBlogContext();
            return db.WritaPosts.AsQueryable().Where(w => w.PostId == id).SingleOrDefault();
        }

        public WritaUser CreateUser(WritaUser u)
        {
            var db = new WritaBlogContext();
            u.Id = System.Guid.NewGuid().ToString();
            db.WritaUsers.Add(u);
            db.SaveChanges();
            return u;
        }

        public void DeleteUser(WritaUser u)
        {
            throw new NotImplementedException();
        }

        public WritaUser UpdateUser(WritaUser u)
        {
            throw new NotImplementedException();
        }

        public WritaUser LogonUser(WritaUser u)
        {
            throw new NotImplementedException();
        }

        public WritaUser GetUserByEmail(WritaUser u)
        {
            var db = new WritaBlogContext();
            return db.WritaUsers.AsQueryable().Where(w => w.Id == u.EmailAddress).SingleOrDefault();
        }

        public WritaUser GetUserById(WritaUser u)
        {
            var db = new WritaBlogContext();
            return db.WritaUsers.AsQueryable().Where(w=>w.Id == u.Id).SingleOrDefault();
        }

        public IQueryable<WritaUser> GetUsers()
        {
            var db = new WritaBlogContext();
            return db.WritaUsers.AsQueryable();
        }

        public WritaPluginSetting GetPluginSettings(string PluginName, string Key, string DefaultValue)
        {
            var db = new WritaBlogContext();
            var exists = db.WritaPluginSettings.AsQueryable().Where(w => w.PluginName == PluginName && w.Key == Key).SingleOrDefault();
            if (exists != null)
            {
                return exists;
            }
            else
            {
                WritaPluginSetting p = new WritaPluginSetting() { Id =System.Guid.NewGuid().ToString(), Key = Key, PluginName = PluginName, Value = DefaultValue };
                db.WritaPluginSettings.Add(p);
                db.SaveChanges();

                return p;
            }
        }


        public WritaPost GetNextPost(WritaPost p)
        {
            var db = new WritaBlogContext();
            var x = db.WritaPosts.AsQueryable().Where(w => w.PostCreated > p.PostCreated && w.PostType == WritaPostType.BLOGPOST && w.PostStatus == WritaPostStatus.PUBLISHED).OrderBy(w => w.PostCreated).Take(1).ToList();
            if (x.Count == 1)
            {
                return x[0];
            }
            else
            {
                return null;
            }
        }

        public WritaPost GetPreviousPost(WritaPost p)
        {
            var db = new WritaBlogContext();
            var x = db.WritaPosts.AsQueryable().Where(w => w.PostCreated < p.PostCreated && w.PostType == WritaPostType.BLOGPOST && w.PostStatus == WritaPostStatus.PUBLISHED).OrderByDescending(w => w.PostCreated).Take(1).ToList();
            if (x.Count == 1)
            {
                return x[0];
            }
            else
            {
                return null;
            }
        }


        public IQueryable<WritaPluginSetting> GetPluginSettings()
        {
            var db = new WritaBlogContext();
            return db.WritaPluginSettings.AsQueryable();
        }


        public void UpdatePluginValue(string sid, string value)
        {
            var db = new WritaBlogContext();
            var plugin = db.WritaPluginSettings.AsQueryable().Where(w=>w.Id == sid).SingleOrDefault();
            plugin.Value = value;
            db.SaveChanges();
        }


        public void DeletePluginValue(string sid)
        {
            var db = new WritaBlogContext();
            var plugin = db.WritaPluginSettings.AsQueryable().Where(w=>w.Id == sid).SingleOrDefault();
            db.WritaPluginSettings.Remove(plugin);
            db.SaveChanges();
        }


        public bool DeletePost(string postid)
        {
            var db = new WritaBlogContext();
            var plugin = db.WritaPosts.AsQueryable().Where(w => w.PostId == postid).SingleOrDefault();
            db.WritaPosts.Remove(plugin);
            db.SaveChanges();
            return true;
        }


        public bool DeleteAllPosts()
        {
            var db = new WritaBlogContext();
            var plugin = db.WritaPosts.AsQueryable().ToList();
            db.WritaPosts.RemoveRange(plugin);
            db.SaveChanges();
            return true;
        }

        public WritaStats GetStats()
        {

            WritaStats s = new WritaStats();
            var db = new WritaBlogContext();

                s.NumberOfPosts = db.WritaPosts.AsQueryable().Where(w => w.PostType == WritaPostType.BLOGPOST).Count();
                s.NumberOfStaticPages = db.WritaPosts.AsQueryable().Where(w => w.PostType != WritaPostType.BLOGPOST).Count();
                s.LastPostDate = db.WritaPosts.AsQueryable().Where(w => w.PostType == WritaPostType.BLOGPOST).OrderByDescending(w => w.PostCreated).Take(1).First().PostCreated;
            
            var config = LoadSettings();
            s.ActiveTheme = config.BlogTheme;
            return s;
        }
    }
}
