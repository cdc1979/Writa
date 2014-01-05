using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Writa.Models
{
    public class WritaPost
    {
        public string PostId { get; set; }
        public string PostTitle { get; set;}
        public string PostSummary { get; set; }
        public string PostContent { get; set; } // current post content in markdown format
        public string PostSlug { get; set; }
        public string PostThumbnail { get; set; }
        public List<string> PostTags = new List<string>();
        public bool Homepage { get; set; }
        public DateTime PostCreated { get; set; }
        public string PostAuthor { get; set; }
        public DateTime PostLastEdited { get; set; }
        public DateTime PostStartDate { get; set; } // schedule post to start on
        public WritaPostStatus PostStatus { get; set; }
        public WritaPostType PostType { get; set; }
        public string PostParent { get; set; } // allows for a post to have a parent
    }

    public enum WritaPostStatus { PUBLISHED, DRAFT, DELETED } // status of this post
    public enum WritaPostType { HOMEPAGE,BLOGPOST,STATICPAGE } // page type

    public class WritaPostContent
    {
        public string PostContentId { get; set; }
        public string PostId { get; set; }
        public string PostMarkdown { get; set; }
        public string PostEditAuthor { get; set; }
        public DateTime? PostEditDate { get; set; }
    }

    public class NewPost
    {
        public string PostId { get; set; }
        public string PostAuthor { get; set; }
        public string PostTitle { get; set; }
        public string PostMarkdown { get; set; }
    }
}
