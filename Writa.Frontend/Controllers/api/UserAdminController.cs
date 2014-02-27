using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Writa.Models;
using BCrypt;
namespace Writa.Frontend.Controllers.api
{
    public class AddUserRequest
    {
        public string UserFullName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }

    public class UpdateUserRequest
    {
        public string Id { get; set; }
        public string UserFullName { get; set; }
        public string EmailAddress { get; set; }
        public AccountType UserType { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string Id { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }

    [RoutePrefix("api")]
    public class UserAdminController : ApiController
    {

        private IDataHelper _dbhelper;
        public UserAdminController(IDataHelper dbhelper)
        {
            _dbhelper = dbhelper;

        }

        // GET api/<controller>
        [Route("updateuser")]
        [HttpPost]
        public string UpdateUser(UpdateUserRequest a)
        {
            var user = _dbhelper.GetUserById(new WritaUser() { Id = a.Id  });
            user.EmailAddress = a.EmailAddress;
            user.UserFullName = a.UserFullName;
            user.UserType = a.UserType;
            _dbhelper.UpdateUser(user);
            return "Updated";
        }

        // GET api/<controller>
        [Route("changepassword")]
        [HttpPost]
        public string UpdateUser(ChangePasswordRequest a)
        {
            var user = _dbhelper.GetUserById(new WritaUser() { Id = a.Id });

                var newEncpassword = BCrypt.Net.BCrypt.HashPassword(a.NewPassword, 13);
                _dbhelper.UpdateUser(user);
                return "Updated";

        }

        // GET api/<controller>
        [Route("adduser")]
        [HttpPost]
        public string AddUser(AddUserRequest a)
        {
            var newEncpassword = BCrypt.Net.BCrypt.HashPassword(a.Password, 13);
            WritaUser newUser = new WritaUser()
            {
                EmailAddress = a.EmailAddress,
                DateRegistered = DateTime.Now,
                DateLastLogin = DateTime.Now,
                UserFullName = a.UserFullName,
                UserPasswordEncrypted = newEncpassword,
                UserIpAddress = ""

            };

            WritaUser u = _dbhelper.CreateUser(newUser);

            return "User "+ a.EmailAddress + " was created";
        }
    }
}