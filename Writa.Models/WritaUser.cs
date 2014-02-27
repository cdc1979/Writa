using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Writa.Models
{
    public class WritaUser
    {
        public string Id { get; set; }
        public DateTime? DateRegistered { get; set; }
        public DateTime? DateLastLogin { get; set; }
        public string UserPasswordEncrypted { get; set; }
        public string UserIpAddress { get; set; }
        public string UserFullName { get; set; }
        public string EmailAddress { get; set; }
        public AccountType UserType { get; set; }
        public bool UserVerified { get; set; }
        public bool CanAddPosts { get; set; }
        public bool CanEditPosts { get; set; }
    }

    public enum AccountType { ADMINISTRATOR, USER }
}
