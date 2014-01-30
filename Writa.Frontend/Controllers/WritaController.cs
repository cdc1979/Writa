using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using Writa.Models;
using Writa.Models.Settings;
using Writa.Models.Email;
using BCrypt;
using BCrypt.Net;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using MarkdownSharp;
using Writa.Frontend.Models;
using RestSharp;
namespace Writa.Frontend.Controllers
{
    public class WritaContent 
    {
        public List<WritaPostItems> posts = new List<WritaPostItems>();
        public WritaPostItems SelectedPost { get; set; }
    }

    public class WritaPostItems
    {
        public string PostId { get; set; }
        public string PostTitle { get;set; }
        public DateTime? PostCreated { get; set; }
        public string CurrentPostMarkdown { get; set; }
    }

    public class WritaPostSettingsUpdate
    {
        public WritaPost post { get; set; }
        public IDataHelper _db { get; set; }
    }

    public class WritaController : Controller
    {
        //
        // GET: /Writa/

        public IDataHelper _dbhelper;
        public IBlogSettingsLoader _blogsettings;
        public ISettingsLoader _globalsettings;
        public IEmailSend _email;
        public WritaController(IDataHelper d, IBlogSettingsLoader b, ISettingsLoader i, IEmailSend e)
        {
            _dbhelper = d;
            _blogsettings = b;
            _globalsettings = i;
            _email = e;

        }

        // POST api/<controller>
        [HttpPost]
        [Authorize]
        public string GetHtml(string postid)
        {
            var md = new MarkdownSharp.Markdown();
            var g = _dbhelper.GetPostFromId(postid);

            return md.Transform(g.PostContent);
        }
        [HttpPost]
        [Authorize]
        public string GetMarkdown(string postid)
        {
            var g = _dbhelper.GetPostFromId(postid);
            return g.PostContent;
        }        
        

        // POST api/<controller>
        [HttpPost]
        [Authorize]
        public string CreatePost(string PostTitle, string PostMarkdown, string poststartdate)
        {
            WritaPost checkexists = _dbhelper.GetPostFromSlug(PostTitle.Replace(" ", "-").ToLower());
            if (checkexists == null)
            {

                WritaPost post = new WritaPost();
                post.PostAuthor = User.Identity.Name;
                post.Homepage = false;
                post.PostSlug = PostTitle.Replace(" ", "-").ToLower();
                post.PostTitle = PostTitle;
                post.PostType = WritaPostType.BLOGPOST;
                post.PostStatus = WritaPostStatus.DRAFT;
                post.PostContent = PostMarkdown;
                post.PostThumbnail = "/content/logo.jpg";
                post.PostCreated = DateTime.Now;
                post.PostLastEdited = DateTime.Now;
                DateTime dateValue;
                if (DateTime.TryParse(poststartdate, out dateValue))
                {
                    post.PostStartDate = dateValue ;
                }
                else
                {
                    post.PostStartDate = DateTime.Now;
                }

                _dbhelper.CreatePost(post);
                RebuildRoutes.Rebuild(true, _dbhelper);

                return "Success";
            }
            else
            {
                return "That post title has been used before, it must be unique.";
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult GetContent()
        {
            WritaContent w = new WritaContent();

            var md = new MarkdownSharp.Markdown();

            int icount = 0;
            foreach (var g in _dbhelper.GetPosts().ToList())
            {
                w.posts.Add(new WritaPostItems() { PostId = g.PostId, PostCreated =g.PostCreated, PostTitle = g.PostTitle });
                icount++;
            }
            
            return Json(w);
        }

        [HttpPost]
        [Authorize]
        public string UploadImage()
        {
            string s = "";
            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];
                file.SaveAs(Server.MapPath("~/Images/" + file.FileName));
            }
            
            return s ;
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Writa", null);
        }

        [HttpPost]
        public ActionResult Login(string password, string email)
        {
            var ux = _dbhelper.GetUserByEmail(new WritaUser() { EmailAddress = email });

            if (ux != null)
            {
                bool matches = BCrypt.Net.BCrypt.Verify(password, ux.UserPasswordEncrypted);

                if (matches)
                {
                    FormsAuthentication.SetAuthCookie(ux.Id, false);
                    //FormsAuthentication.RedirectFromLoginPage(u.Id, false); 
                    //return View("~/Views/Admin/Dashboard.cshtml", "~/Views/Admin/_Adminlayout.cshtml");
                    return RedirectToAction("Index");
                }
                else
                {
                    //ViewBag.LoginError = "<div class=\"ui-state-error\">Password did not match</div>";
                    return View("~/Views/Admin/Login.cshtml", "~/Views/Admin/_Adminlayout.cshtml");
                }

                //return RedirectToAction("Index", "Admin", null);
            }
            else
            {
                // ViewBag.LoginError = "<div class=\"ui-state-error\">Username/Password Not Found </div>";
                return View("~/Views/Admin/Login.cshtml", "~/Views/Admin/_Adminlayout.cshtml");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string password, string username, string email, string subscribetonews)
        {
            var newEncpassword = BCrypt.Net.BCrypt.HashPassword(password, 13);

            bool subscribe = false;
            if (subscribetonews == "on")
            {

                subscribe = true;

                try
                {
                    var client = new RestClient("http://writa.org");
                    var request = new RestRequest("api/notify/subscribe", Method.GET);
                    request.AddParameter("email", email);
                    request.AddParameter("name", username);
                    IRestResponse response = client.Execute(request);
                    var content = response.Content; // raw content as string
                }
                catch { }
            }

                var ux = _dbhelper.GetUserByEmail(new WritaUser () {  EmailAddress = email });
                if (ux == null)
                {
                    WritaUser newUser = new WritaUser()
                    {
                        EmailAddress = email,
                        DateRegistered = DateTime.Now,
                        DateLastLogin = DateTime.Now,
                        UserFullName = username,
                        UserPasswordEncrypted = newEncpassword,
                        UserIpAddress = Request.ServerVariables["REMOTE_ADDR"]

                    };

                    WritaUser u = _dbhelper.CreateUser(newUser);

                    //send registration email?
                    //var fullurl = this.Url.Action("ValidateAccount", "Account", null, this.Request.Url.Scheme);

                    //var template = System.IO.File.ReadAllText(Server.MapPath("~/App_Data/UserActivationTemplate.txt"));
                    //template = template.Replace("{activationurl}", fullurl + "?key=" + u.Id + "&eml=" + u.EmailAddress);
                    //NotificationEmailhelper.SendUserActivationEmail(u.Id, u.EmailAddress, template);
                    //

                    FormsAuthentication.RedirectFromLoginPage(u.Id, false);
                }
                else
                {
                    ViewBag.RegisterError = "An account already exists";
                }


            return View("Index");
        }

        [Authorize]
        public ActionResult Dashboard()
        {
            return View("~/Views/Admin/Dashboard.cshtml", "~/Views/Admin/_Adminlayout.cshtml", _dbhelper);
        }

        [Authorize]
        public ActionResult Editor(string id)
        {
            return View("~/Views/Admin/Editor.cshtml", "~/Views/Admin/_Adminlayout.cshtml");
        }

        [Authorize]
        public string GenerateRss() 
        {
            var fullurl = this.Url.RouteUrl("Home");
            string baseurl = fullurl;
            RssHelper r = new RssHelper(_dbhelper, _blogsettings);

            return r.GenerateRss(baseurl);
        }

        [Authorize]
        public PartialViewResult EditSettings(string id)
        {
            var post = _dbhelper.GetPostFromId(id);
            WritaPostSettingsUpdate w = new WritaPostSettingsUpdate();
            if (post.PostParent == null)
            {
                post.PostParent = "";
            }
            w.post = post;
            
            w._db = _dbhelper;
            return PartialView("~/Views/Admin/_UpdatePostSettings.cshtml", w);
        }

        [Authorize]
        public ActionResult Content()
        {
            return View("~/Views/Admin/Content.cshtml", "~/Views/Admin/_Adminlayout.cshtml");
        }

        [Authorize]
        public ActionResult PluginSettings()
        {
            return View("~/Views/Admin/PluginSettings.cshtml", "~/Views/Admin/_Adminlayout.cshtml", _dbhelper);
        }

        [Authorize]
        public ActionResult Settings()
        {
            AllSettings a = new AllSettings();
            a.writaSettings = _blogsettings.LoadSettings();
            a.globalSettings = _globalsettings.LoadSettings();
            return View("~/Views/Admin/Settings.cshtml", "~/Views/Admin/_Adminlayout.cshtml", a );
        }

        [Authorize]
        public ActionResult Media()
        {
            return View("~/Views/Admin/Media.cshtml", "~/Views/Admin/_Adminlayout.cshtml", _blogsettings.LoadSettings());
        }

        [Authorize]
        public PartialViewResult SelectImage(string classname)
        {
            return PartialView("~/Views/Admin/_SelectImage.cshtml", classname);
        }

        [Authorize]
        public ActionResult Editor2(string id)
        {
            ViewBag.New = true;
            ViewBag.PostId = id;
            if (id != null)
            {
                if (id.Length > 1)
                {
                    ViewBag.New = false;
                }
            }

            return View("~/Views/Admin/Editor2.cshtml", "~/Views/Admin/_Adminlayout.cshtml", id);
        }

        public ActionResult Index()
        {
            // test if admin user exists.

            if (_dbhelper.GetUsers().Count() == 0)
            {
                // display regstration/install screen
                return View("~/Views/Admin/Register.cshtml", "~/Views/Admin/_Adminlayout.cshtml");
            }
            else
            {
                // display login screen
                if (User.Identity.IsAuthenticated)
                {
                    return View("~/Views/Admin/Dashboard.cshtml", "~/Views/Admin/_Adminlayout.cshtml", _dbhelper);
                }
                else
                {
                    return View("~/Views/Admin/Login.cshtml", "~/Views/Admin/_Adminlayout.cshtml");
                }
            }

            //return View();
        }

        public ActionResult ResetPassword()
        {
            return View("~/Views/Admin/ResetPassword.cshtml", "~/Views/Admin/_Adminlayout.cshtml");
        }

        [HttpPost]
        public ActionResult ResetPassword(string email)
        {
            var user = _dbhelper.GetUserByEmail(new WritaUser() { EmailAddress = email });
            if (user != null)
            {
                var newpass = System.Web.Security.Membership.GeneratePassword(10, 2);
                var newEncpassword = BCrypt.Net.BCrypt.HashPassword(newpass, 13);
                string subject = "Your Writa password reset";
                string body = "Your new writa password is " + newpass;

                _email.SendEmail(subject, body, email, _globalsettings.LoadSettings().EmailFromAddress);
                ViewBag.Sent = true;
            }
            else
            {
                ViewBag.Error = true;
            }
            return View("~/Views/Admin/ResetPassword.cshtml", "~/Views/Admin/_Adminlayout.cshtml");
        }

        [Authorize]
        public PartialViewResult RestoreDb()
        {
            return PartialView("~/Views/Admin/_RestoreDb.cshtml");
        }

    }
}
