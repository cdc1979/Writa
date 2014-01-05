using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Writa.Models;
using Writa.Frontend.Models;
namespace Writa.Frontend.Controllers.api
{
    public class CreatePostController : ApiController
    {
        public IDataHelper _datahelper;

        public CreatePostController(IDataHelper d)
        {
            _datahelper = d;

        }

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public string Post(string PostTitle, string PostMarkdown)
        {

            WritaPost post = new WritaPost();
            post.PostAuthor = User.Identity.Name;
            post.Homepage = false;
            post.PostSlug = PostTitle.Replace(" ", "-").Replace(".","").ToLower();
            post.PostTitle = PostTitle;
            post.PostContent = PostMarkdown;
            post.PostCreated = DateTime.Now;
            post.PostLastEdited = DateTime.Now;
            post.PostThumbnail = "/content/logo.jpg";
            
            _datahelper.CreatePost(post);
            RebuildRoutes.Rebuild(true, _datahelper);
            return "done";
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}