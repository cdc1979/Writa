using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Writa.Models;
using Writa.Models.Install;
using Writa.Models.Settings;
using Writa.Models.Stats;
using Raven;
using Raven.Abstractions.Data;
using Raven.Database;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Raven.Database.Indexing;
using Raven.Database.Storage;
namespace Writa.Data
{
    /*
     * Raven Database Connector
     */

    public class RavenDataHelper : IDataHelper, IBlogSettingsLoader
    {
        public GlobalSettings _settings;
        public EmbeddableDocumentStore docStore;

        public RavenDataHelper(GlobalSettings s, string dir)
        {
            docStore = new EmbeddableDocumentStore { DataDirectory = dir };
            docStore.Conventions.RegisterIdConvention<WritaPost>((dbname, commands, user) => "writaposts/" + user.PostId);
            docStore.Conventions.RegisterIdConvention<WritaSettings>((dbname, commands, user) => "writasettings/" + user.SettingsId);
            docStore.Conventions.RegisterIdConvention<WritaPostContent>((dbname, commands, user) => "writapostcontents/" + user.PostContentId);
            docStore.Conventions.RegisterIdConvention<WritaUser>((dbname, commands, user) => "writausers/" + user.Id);
            docStore.Conventions.RegisterIdConvention<WritaPluginSetting>((dbname, commands, user) => "writapluginsettings/" + user.Id);
            docStore.Initialize();
            _settings = s;
        }

        public bool CheckInstall(GlobalSettings s)
        {
            try
            {
                using (var session = docStore.OpenSession())
                {
                    var results = session.Query<WritaPost>().Count();
                    if (results == 0)
                    {
                        InstallSet set = InstallHelper.GetInstall();
                        CreatePost(set.homepage);
                        CreatePost(set.firstpost);
                        //db.WritaSettings.Add(set.settings);
                        //db.SaveChanges();
                        using (var f = docStore.OpenSession())
                        {
                            f.Store(set.settings);
                            f.SaveChanges();
                        }
                        //install.
                    }
                    session.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
            System.Threading.Thread.Sleep(3000);
            
        }

        public WritaPost CreatePost(WritaPost p)
        {
            using (var session = docStore.OpenSession())
            {
                p.PostId = System.Guid.NewGuid().ToString();
                if (p.PostCreated == null)
                {
                    p.PostCreated = DateTime.Now;
                }
                if (p.PostLastEdited == null)
                {
                    p.PostLastEdited = DateTime.Now;
                }
                if (p.PostStartDate == null)
                {
                    p.PostStartDate = DateTime.Now;
                }
                p.PostSlug = p.PostTitle.ToLower().Replace(" ", "-");
                p.PostParent = "";
                session.Store(p);
                session.SaveChanges();

                WritaPostContent c = new WritaPostContent();
                c.PostContentId = System.Guid.NewGuid().ToString();
                c.PostId = p.PostId;
                c.PostEditDate = DateTime.Now;
                c.PostEditAuthor = p.PostAuthor;
                c.PostMarkdown = p.PostContent;

                session.Store(c);
                session.SaveChanges();
                return p;
            }
        }

        public void DeletePost(WritaPost p)
        {
            throw new NotImplementedException();
        }

        public WritaPost UpdatePost(WritaPost p)
        {
            using (var session = docStore.OpenSession())
            {
                WritaPost pl = session.Load<WritaPost>("writaposts/"+p.PostId);
                //pl.PostAuthor = p.PostAuthor;
                pl.PostContent = p.PostContent;
                pl.PostLastEdited = DateTime.Now;
                pl.PostSlug = p.PostSlug;
                pl.PostStatus = p.PostStatus;
                pl.PostSummary = p.PostSummary;
                pl.PostTags = p.PostTags;
                pl.PostThumbnail = p.PostThumbnail;
                pl.PostTitle = p.PostTitle;
                pl.PostType = p.PostType;
                pl.PostParent = p.PostParent;
                if (p.PostCreated != pl.PostCreated)
                {
                    pl.PostCreated = p.PostCreated;
                }
                session.SaveChanges();
                return p;
            }
            
        }

        public IQueryable<WritaPost> GetPosts()
        {
            using (var session = docStore.OpenSession())
            {
                // this only loads "active posts", i.e. those that are published and scheduled to be visible.
                return session.Query<WritaPost>().Customize(x => x.WaitForNonStaleResultsAsOfNow()).AsQueryable().Where(w => DateTime.Now > w.PostStartDate && w.PostStatus == WritaPostStatus.PUBLISHED);
            }
        }

        public IQueryable<WritaPost> GetAllPosts()
        {
            using (var session = docStore.OpenSession())
            {
                // this gets all posts, regardless of status or schedule.
                return session.Query<WritaPost>().Customize(x => x.WaitForNonStaleResultsAsOfNow()).AsQueryable();
            }
        }

        public IQueryable<WritaPost> GetAllPosts(string tagid)
        {
            using (var session = docStore.OpenSession())
            {
                return session.Query<WritaPost>().Customize(x => x.WaitForNonStaleResultsAsOfNow()).AsQueryable().Where(w => w.PostTags.Contains(tagid)).AsQueryable();
            }
        }

        public WritaPost GetPostFromSlug(string slug)
        {
            using (var session = docStore.OpenSession())
            {
                return session.Query<WritaPost>().Where(w => w.PostSlug == slug).SingleOrDefault();
            }
        }

        public WritaPost GetPostFromId(string id)
        {
            using (var session = docStore.OpenSession())
            {
                return session.Query<WritaPost>().Where(w => w.PostId == id).SingleOrDefault();
            }
        }

        public WritaUser CreateUser(WritaUser u)
        {
            using (var session = docStore.OpenSession())
            {
                u.Id = System.Guid.NewGuid().ToString();
                session.Store(u);
                session.SaveChanges();
                return u;
            }
        }

        public void DeleteUser(WritaUser u)
        {
            throw new NotImplementedException();
        }

        public WritaUser UpdateUser(WritaUser u)
        {
            using (var session = docStore.OpenSession())
            {
                WritaUser ux = session.Load<WritaUser>("writausers/" + u.Id);
                ux.UserPasswordEncrypted = u.UserPasswordEncrypted;
                session.SaveChanges();
                return ux;
            }
        }

        public WritaUser LogonUser(WritaUser u)
        {
            throw new NotImplementedException();
        }

        public WritaUser GetUserByEmail(WritaUser u)
        {
            using (var session = docStore.OpenSession())
            {
                return session.Query<WritaUser>().AsQueryable().Where(w => w.EmailAddress == u.EmailAddress).SingleOrDefault();
            }
        }

        public WritaUser GetUserById(WritaUser u)
        {
            using (var session = docStore.OpenSession())
            {
                return session.Query<WritaUser>().AsQueryable().Where(w => w.Id == u.Id).SingleOrDefault();
            }
        }

        public IQueryable<WritaUser> GetUsers()
        {
            using (var session = docStore.OpenSession())
            {
                return session.Query<WritaUser>().AsQueryable();
            }
            
        }



        public WritaSettings LoadSettings()
        {
            //throw new NotImplementedException();
            using (var session = docStore.OpenSession())
            {
                return session.Query<WritaSettings>().Take(1).ToList()[0];
            }
        }

        public WritaSettings SaveSettings(WritaSettings s)
        {
            using (var session = docStore.OpenSession())
            {
                var setts = session.Query<WritaSettings>().Take(1).SingleOrDefault();
                var x = session.Load<WritaSettings>("writasettings/"+setts.SettingsId);
                x.BlogTheme = s.BlogTheme;
                x.BlogSummary = s.BlogSummary;
                x.BlogTitle = s.BlogTitle;
                x.BlogDefaultEmail = s.BlogDefaultEmail;
                session.SaveChanges();
                return s;
            }
        }


        public WritaPost GetNextPost(WritaPost p)
        {
            using (var session = docStore.OpenSession())
            {
                var x = session.Query<WritaPost>().Where(w => w.PostCreated > p.PostCreated && w.PostType == WritaPostType.BLOGPOST && w.PostStatus == WritaPostStatus.PUBLISHED).OrderBy(w => w.PostCreated).Take(1).ToList();
                if (x.Count == 1)
                {
                    return x[0];
                }
                else
                {
                    return null;
                }
            }
        }

        public WritaPost GetPreviousPost(WritaPost p)
        {
            using (var session = docStore.OpenSession())
            {
                var x =  session.Query<WritaPost>().Where(w => w.PostCreated < p.PostCreated && w.PostType == WritaPostType.BLOGPOST && w.PostStatus == WritaPostStatus.PUBLISHED).OrderByDescending(w => w.PostCreated).Take(1).ToList();
                if (x.Count == 1)
                {
                    return x[0];
                }
                else
                {
                    return null;
                }
            }
        }

        public WritaPluginSetting GetPluginSettings(string PluginName, string Key, string DefaultValue)
        {
            using (var session = docStore.OpenSession())
            {
                var exists = session.Query<WritaPluginSetting>().AsQueryable().Where(w => w.PluginName == PluginName && w.Key == Key).Take(1).ToList();
                if (exists.Count == 1)
                {
                    return exists[0];
                }
                else
                {
                    WritaPluginSetting p = new WritaPluginSetting() { Id = System.Guid.NewGuid().ToString(), Key = Key, PluginName = PluginName, Value = DefaultValue };
                    session.Store(p);
                    session.SaveChanges();
                    return p;
                }
            }
        }
        public IQueryable<WritaPluginSetting> GetPluginSettings()
        {
            using (var session = docStore.OpenSession())
            {
                return session.Query<WritaPluginSetting>().AsQueryable();
            }
        }


        public void UpdatePluginValue(string sid, string value)
        {
            using (var session = docStore.OpenSession())
            {
                var exists = session.Load<WritaPluginSetting>(sid);
                exists.Value = value;
                session.SaveChanges();
            }
        }


        public void DeletePluginValue(string sid)
        {
            using (var session = docStore.OpenSession())
            {
                var exists = session.Load<WritaPluginSetting>(sid);
                session.Delete(exists);
                session.SaveChanges();
            }
        }


        public bool DeletePost(string postid)
        {
            using (var session = docStore.OpenSession())
            {
                session.Advanced.DocumentStore.DatabaseCommands.Delete("writaposts/"+postid, null);
                return true;
            }
        }


        public bool DeleteAllPosts()
        {
            docStore.DatabaseCommands.DeleteByIndex("Auto/WritaPosts", new IndexQuery(), true);
            return true;
        }

        public WritaStats GetStats()
        {
            WritaStats s = new WritaStats();
            using (var session = docStore.OpenSession())
            {
                s.NumberOfPosts = session.Query<WritaPost>().AsQueryable().Where(w => w.PostType == WritaPostType.BLOGPOST).Count();
                s.NumberOfStaticPages = session.Query<WritaPost>().AsQueryable().Where(w => w.PostType != WritaPostType.BLOGPOST).Count();
                s.LastPostDate = session.Query<WritaPost>().AsQueryable().Where(w => w.PostType == WritaPostType.BLOGPOST).OrderByDescending(w => w.PostCreated).Take(1).First().PostCreated;
            }
            var config = LoadSettings();
            s.ActiveTheme = config.BlogTheme;
            return s;
        }
    }

}