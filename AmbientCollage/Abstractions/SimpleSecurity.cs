using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace AmbientCollage.Abstractions
{
    public class SimpleSecurity : ISecurity
    {

        public string EncryptPassword(string password)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(password, "sha1");
        }

    }
}