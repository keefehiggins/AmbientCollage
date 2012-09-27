using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AmbientCollage.Models;
using AmbientCollage.Abstractions;

namespace AmbientCollage.Controllers
{
    public class HomeController : Controller
    {
        DataAccessLayer dal = new DataAccessLayer(new MongoDataAccessLayer(new SimpleSecurity()));

        //
        // GET: /Home/
        public ActionResult Welcome()
        {
            User currentUser = (User)HttpContext.Session["CurrentUser"];

            if (currentUser != null)
                return View(currentUser);
            else
                return View("../Login");
        }

        [HttpPost]
        public ActionResult ReturningUser(User user)
        {
            User setUser = dal.PerformLogin(user.Email, user.PasswordHash);

            HttpContext.Session["CurrentUser"] = setUser;

            return View("../Home/Welcome", setUser);
        }

        public ActionResult NewUser(User user)
        {
            dal.CreateNewUser(user.UserName, user.Email, user.PasswordHash);
            User createdUser = dal.PerformLogin(user.Email, user.PasswordHash);
            HttpContext.Session["CurrentUser"] = createdUser;

            return View("../Home/Welcome", createdUser);
        }

        [HttpPost]
        public ActionResult CreateNewExperience(Experience experience)
        {
            experience.Creator = (User)HttpContext.Session["CurrentUser"];
            dal.AddExperience(experience);
            return View("../Home/Welcome", experience.Creator);
        }

        [HttpGet]
        public List<Experience> LoadUserExperiences(Guid userId)
        {
            List<Experience> allExp = dal.FindExperiences("").ToList();
            return allExp;
        }
    }
}
