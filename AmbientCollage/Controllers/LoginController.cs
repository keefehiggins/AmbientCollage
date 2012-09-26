using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AmbientCollage.Abstractions;

namespace AmbientCollage.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        public ActionResult Index()
        {
            return View("Greet");
        }

        public ActionResult Create(string userName, string email, string password)
        {
            DataAccessLayer dal = new DataAccessLayer(new MongoDataAccessLayer(new SimpleSecurity()));

            dal.CreateNewUser(userName, email, password);

            return View();
        }

    }
}
