using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Writa.Models;
namespace Writa.Frontend.Models
{
    public class UserManagerViewModel
    {
        public List<WritaUser> users = new List<WritaUser>();
        public WritaUser currentuser { get; set; }
        public List<WritaPost> posts = new List<WritaPost>();
    }
}