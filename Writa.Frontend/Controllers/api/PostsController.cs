using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Writa.Models;
using Writa.Models.Validation;
using Writa.Frontend.Models;
using FluentValidation.Results;
namespace Writa.Frontend.Controllers.api
{
    public class PostUpdate {
        public string postid { get;set ;}
        public string posttitle { get;set ;}
        public string postmarkdown { get;set ;}
    }

    public class TagUpdate
    {
        public string postid { get;set ;}
        public string tag { get;set ;}
        public bool delete { get; set; }
    }

    public class DeletePost
    {
        public string postid { get; set; }
    }


    [RoutePrefix("api/posts")]
    public class PostsController : ApiController
    {
        public IDataHelper _dbhelper;

        public PostsController(IDataHelper d)
        {
            _dbhelper = d;
        }

        [Route("get")]
        [Authorize]
        public WritaContent Get()
        {
            WritaContent w = new WritaContent();
            var md = new MarkdownSharp.Markdown();
            int icount = 0;
            foreach (var g in _dbhelper.GetPosts().ToList())
            {
                w.posts.Add(new WritaPostItems() { PostId = g.PostId, PostCreated = g.PostCreated, PostTitle = g.PostTitle });
                if (icount == 0) {
                    w.SelectedPost = w.posts[0];
                }
                icount++;
            }
            return w;
        }

        // GET api/<controller>/5
        [Authorize]
        public WritaPostItems Get(string id)
        {
            var g = _dbhelper.GetPostFromId(id);
            return new WritaPostItems() { 
                PostId = g.PostId, PostCreated = g.PostCreated, PostTitle = g.PostTitle, 
                CurrentPostMarkdown = g.PostContent 
            };
        }

        // POST api/<controller>
        [Route("update")]
        [HttpPost]
        [Authorize]
        public string Update(PostUpdate u)
        {
            var post = _dbhelper.GetPostFromId(u.postid);
            post.PostTitle = u.posttitle;
            post.PostContent = u.postmarkdown;
            _dbhelper.UpdatePost(post);
            return u.posttitle;
        }

        [Route("tag")]
        [HttpPost]
        [Authorize]
        public string tag(TagUpdate t)
        {
            var post = _dbhelper.GetPostFromId(t.postid);
            if (t.delete)
            {
                
                post.PostTags.Remove(t.tag.ToLower());
                _dbhelper.UpdatePost(post);
                return "Deleted";
            }
            else
            {
                if (!post.PostTags.Contains(t.tag))
                {
                    post.PostTags.Add(t.tag.ToLower());
                }
                _dbhelper.UpdatePost(post);
                return "Tagged";
            }
        }

        [Route("deletepost")]
        [HttpPost]
        [Authorize]
        public string Deletepost(DeletePost post)
        {
            if (post != null)
            {
                if (post.postid != null)
                {
                    _dbhelper.DeletePost(post.postid);
                    RebuildRoutes.Rebuild(true, _dbhelper);
                    return "deleted " + post.postid;
                }
                else
                {
                    return "postid null";
                }
            }
            else
            {
                return "post null";
            }
        }

        [Route("updatesettings")]
        [HttpPost]
        [Authorize]
        public string Updatesettings(WritaPost u)
        {
            ValidationResult results =  ValidationHelper.ValidateWritaPost(u, _dbhelper);

            if (results.IsValid)
            {
                WritaPost x = _dbhelper.GetPostFromId(u.PostId);
                x.PostSlug = StringTools.ReplaceBadInUrl(u.PostSlug);
                x.PostTitle = u.PostTitle;
                x.PostRedirect = u.PostRedirect;
                x.PostType = u.PostType;
                x.PostStatus = u.PostStatus;
                x.PostCreated = u.PostCreated;
                x.PostSummary = u.PostSummary;
                x.PostThumbnail = u.PostThumbnail;
                x.PostParent = u.PostParent;
                _dbhelper.UpdatePost(x);
                RebuildRoutes.Rebuild(true, _dbhelper);
                return "Success - post updated";
            }
            else
            {

                var s = "Error";
                foreach (ValidationFailure f in results.Errors)
                {
                    s += f.ErrorMessage;
                }
                return s;

            }

            
        }
    }
}