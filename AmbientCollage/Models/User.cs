using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace AmbientCollage.Models
{
    [Serializable]
    public class User
    {
        public User()
        {

        }

        public User(string userName, string passwordHash, string email)
        {
            UserName = userName;
            PasswordHash = passwordHash;
            Email = email;
            Favorites = new List<Experience>();
        }

        public Guid id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public List<Experience> Favorites { get; set; }
    }
}