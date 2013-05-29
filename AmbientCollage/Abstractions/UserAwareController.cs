using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AmbientCollage.Models;

namespace AmbientCollage.Abstractions
{
    public class UserAwareController : Controller
    {

        public User CurrentUser
        {
            get
            {
                User currentUser = (User)HttpContext.Session["CurrentUser"];
                return currentUser;
            }
            set
            {
                HttpContext.Session["CurrentUser"] = value;
            }
        }

    }
}
