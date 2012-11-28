using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AmbientCollage.Abstractions;
using AmbientCollage.Models;

namespace AmbientCollage.Controllers
{
    public class LoginController : UserAwareController
    {

        DataAccessLayer dal = new DataAccessLayer(new MongoDataAccessLayer(new SimpleSecurity()));
        //
        // GET: /Login/

        public ActionResult Index()
        {
            return View("Greet");
        }

        [HttpGet]
        public string DoesUserExist(string email)
        {
            User find = dal.GetUserByEmail(email);

            if (find != null)
                return find.UserName;
            else
                return null;
        }
    }
}
