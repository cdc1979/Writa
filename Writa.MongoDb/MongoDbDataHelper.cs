using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Writa.Models;
using Writa.Models.Settings;
using Writa.Models.Stats;
using Writa.Models.Install;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
namespace Writa.Data
{
    public class MongoDbDataHelper : IDataHelper, IBlogSettingsLoader
    {
        public MongoServer server;
        public MongoClient client;
        public MongoDatabase database;
        public GlobalSettings _settings;

        public MongoDbDataHelper(GlobalSettings s)
        {
            BsonClassMap.RegisterClassMap<WritaSettings>(cm =>
            {
                cm.AutoMap();
                cm.SetIdMember(cm.GetMemberMap(c => c.SettingsId));
            });
            BsonClassMap.RegisterClassMap<WritaPost>(cm =>
            {
                cm.AutoMap();
                cm.SetIdMember(cm.GetMemberMap(c => c.PostId));
            });
            BsonClassMap.RegisterClassMap<WritaPostContent>(cm =>
            {
                cm.AutoMap();
                cm.SetIdMember(cm.GetMemberMap(c => c.PostContentId));
            });
            BsonClassMap.RegisterClassMap<WritaPluginSetting>(cm =>
            {
                cm.AutoMap();
                cm.SetIdMember(cm.GetMemberMap(c => c.Id));
            });

            _settings = s;
            client = new MongoClient(s.DbConnectionString);
            server = client.GetServer();
            database = server.GetDatabase(s.DbName);
        }

        public WritaPost CreatePost(WritaPost p)
        {
            p.PostId = ObjectId.GenerateNewId().ToString();
            p.PostCreated = DateTime.Now;
            p.PostLastEdited = DateTime.Now;
            p.PostSlug = p.PostTitle.ToLower().Replace(" ", "-");
            database.GetCollection<WritaPost>("Posts").Save(p);

            WritaPostContent c = new WritaPostContent();
            c.PostContentId = ObjectId.GenerateNewId().ToString();
            c.PostId = p.PostId;
            c.PostEditDate = DateTime.Now;
            c.PostEditAuthor = p.PostAuthor;
            c.PostMarkdown = p.PostContent;
                
            database.GetCollection<WritaPostContent>("Content").Save(c);
            return p;
        }

        public void DeletePost(WritaPost p)
        {
            throw new NotImplementedException();
        }

        public WritaPost UpdatePost(WritaPost p)
        {
            database.GetCollection<WritaPost>("Posts").Save(p);
            return p;
        }

        public IQueryable<WritaPost> GetPosts()
        {
            return database.GetCollection<WritaPost>("Posts").AsQueryable().Where(w=>w.PostType == WritaPostType.BLOGPOST).AsQueryable();
        }
        public IQueryable<WritaPost> GetAllPosts()
        {
            return database.GetCollection<WritaPost>("Posts").AsQueryable();
        }
        public IQueryable<WritaPost> GetAllPosts(string tag)
        {
            return database.GetCollection<WritaPost>("Posts").AsQueryable().Where(w=>w.PostTags.Contains(tag)).AsQueryable();
        }

        public WritaPost GetPostFromSlug(string slug)
        {
            return database.GetCollection<WritaPost>("Posts").AsQueryable().Where(w=>w.PostSlug == slug.ToLower()).SingleOrDefault();
        }
        public WritaPost GetPostFromId(string id)
        {
            return database.GetCollection<WritaPost>("Posts").AsQueryable().Where(w => w.PostId == id).SingleOrDefault();
        }
        public WritaUser CreateUser(WritaUser u)
        {
            u.Id = System.Guid.NewGuid().ToString();
            database.GetCollection<WritaUser>("Users").Save(u);
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

        public IQueryable<WritaUser> GetUsers()
        {
            return database.GetCollection<WritaUser>("Users").AsQueryable();
        }

        public void CheckInstall(GlobalSettings s)
        {
            if (!database.GetCollection<WritaPost>("Posts").Exists())
            {
                InstallSet set = InstallHelper.GetInstall();
                CreatePost(set.homepage);
                CreatePost(set.firstpost);
                database.GetCollection<WritaSettings>("Settings").Save(set.settings);
                //install.
            }

        }

        public WritaSettings LoadSettings()
        {

            return database.GetCollection<WritaSettings>("Settings").AsQueryable().Take(1).SingleOrDefault();
        }

        public WritaSettings SaveSettings(WritaSettings s)
        {

            var setts = database.GetCollection<WritaSettings>("Settings").AsQueryable().Take(1).SingleOrDefault();
            setts.BlogTitle = s.BlogTitle;
            setts.BlogTheme = s.BlogTheme;
            setts.BlogSummary = s.BlogSummary;
            database.GetCollection<WritaSettings>("Settings").Save(setts);
            return setts;
        }


        public WritaUser GetUserByEmail(WritaUser u)
        {
            return database.GetCollection<WritaUser>("Users").AsQueryable().Where(w=>w.EmailAddress == u.EmailAddress).SingleOrDefault();
        }

        public WritaUser GetUserById(WritaUser u)
        {
            return database.GetCollection<WritaUser>("Users").AsQueryable().Where(w => w.Id == u.Id).SingleOrDefault();
        }


        public WritaPluginSetting GetPluginSettings(string PluginName, string Key, string DefaultValue)
        {
            var exists = database.GetCollection<WritaPluginSetting>("PluginSettings").AsQueryable().Where(w => w.PluginName == PluginName && w.Key == Key).SingleOrDefault();
            if (exists != null)
            {
                return exists;
            }
            else
            {
                WritaPluginSetting p = new WritaPluginSetting() { Id = ObjectId.GenerateNewId().ToString(), Key = Key, PluginName = PluginName, Value = DefaultValue } ;
                database.GetCollection<WritaPluginSetting>("PluginSettings").Save(p);

                return p;
            }
        }


        public WritaPost GetNextPost(WritaPost p)
        {
            var x = database.GetCollection<WritaPost>("Posts").AsQueryable().Where(w => w.PostCreated > p.PostCreated && w.PostType == WritaPostType.BLOGPOST && w.PostStatus == WritaPostStatus.PUBLISHED).OrderBy(w => w.PostCreated).Take(1).ToList();
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
            var x = database.GetCollection<WritaPost>("Posts").AsQueryable().Where(w => w.PostCreated < p.PostCreated && w.PostType == WritaPostType.BLOGPOST && w.PostStatus == WritaPostStatus.PUBLISHED).OrderByDescending(w => w.PostCreated).Take(1).ToList();
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
            return database.GetCollection<WritaPluginSetting>("PluginSettings").AsQueryable();
        }


        public void UpdatePluginValue(string sid, string value)
        {
            var x = database.GetCollection<WritaPluginSetting>("PluginSettings").AsQueryable().Where(w => w.Id == sid).SingleOrDefault();
            x.Value = value;
            database.GetCollection<WritaPluginSetting>("PluginSettings").Save(x);
        }


        public void DeletePluginValue(string sid)
        {
            database.GetCollection<WritaPluginSetting>("PluginSettings").Remove(Query.EQ("_id", sid));
        }


        public bool DeletePost(string postid)
        {
            database.GetCollection<WritaPost>("Posts").Remove(Query.EQ("_id", postid));
            return true;
        }


        public bool DeleteAllPosts()
        {
            database.GetCollection<WritaPost>("Posts").RemoveAll();
            return true;
        }

        public WritaStats GetStats()
        {
            WritaStats s = new WritaStats();
            s.NumberOfPosts = database.GetCollection<WritaPost>("Posts").AsQueryable().Where(w => w.PostType == WritaPostType.BLOGPOST).Count();
            s.NumberOfStaticPages = database.GetCollection<WritaPost>("Posts").AsQueryable().Where(w => w.PostType != WritaPostType.BLOGPOST).Count();
            s.LastPostDate = database.GetCollection<WritaPost>("Posts").AsQueryable().Where(w => w.PostType == WritaPostType.BLOGPOST).OrderByDescending(w => w.PostCreated).Take(1).First().PostCreated;
            var config = LoadSettings();
            s.ActiveTheme = config.BlogTheme;
            return s;
        }
    }
}
