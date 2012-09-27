using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AmbientCollage.Models;

namespace AmbientCollage.Abstractions
{
    public interface ISecurity
    {
        string EncryptPassword(string password);
    }

    public class Security: ISecurity
    {
        private ISecurity internalSecurity;

        public Security(ISecurity securityToUse)
        {
            internalSecurity = securityToUse;
        }

        public string EncryptPassword(string password)
        {
            return internalSecurity.EncryptPassword(password);
        }
    }
}