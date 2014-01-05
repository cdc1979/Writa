using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Writa.Models.Settings;
namespace Writa.Models
{
    public interface IWritaPage
    {
        GlobalSettings Settings { get; set; }
        WritaSettings BlogSettings { get; set; }
        WritaPost Post { get; set; }
        WritaPost NextPost { get; set; }
        WritaPost PreviousPost { get; set; }
        List<WritaPost> RelatedPosts { get; set; }
        string Tag { get; set; }
        string PageTitle { get; set; }
        string PageDescription { get; set; }
    }
    public class WritaPage : IWritaPage
    {
        public GlobalSettings Settings { get; set; }
        public WritaSettings BlogSettings { get; set; }
        public WritaPost Post { get; set; }
        public WritaPost NextPost { get; set; }
        public WritaPost PreviousPost { get; set; }
        public List<WritaPost> RelatedPosts { get; set; }
        public string Tag { get; set; }
        public string PageTitle { get; set; }
        public string PageDescription { get; set; }
    }
}
