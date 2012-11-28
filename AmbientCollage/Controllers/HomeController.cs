using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AmbientCollage.Models;
using AmbientCollage.Abstractions;

namespace AmbientCollage.Controllers
{
    public class HomeController : UserAwareController
    {
        DataAccessLayer dal = new DataAccessLayer(new MongoDataAccessLayer(new SimpleSecurity()));
        //DataAccessLayer dal = new DataAccessLayer(new FakeDataAccessLayer(new SimpleSecurity()));

        //
        // GET: /Home/
        public ActionResult Welcome()
        {
            if (CurrentUser != null)
                return View(CurrentUser);
            else
                return View("../Login");
        }

        [HttpPost]
        public ViewResult ReturningUser(User user)
        {
            User setUser = dal.PerformLogin(user.Email, user.PasswordHash);

            if (setUser != null)
            {
                // login success!
                CurrentUser = setUser;
                return View("Welcome", CurrentUser);
            }
            else
            {
                return View("../Login");
            }
        }

        public ViewResult NewUser(User user)
        {
            dal.CreateNewUser(user.UserName, user.Email, user.PasswordHash);
            CurrentUser = dal.PerformLogin(user.Email, user.PasswordHash);
            return View("Welcome", CurrentUser);
        }

        [HttpPost]
        public void CreateNewExperience(Experience experience)
        {
            experience.Creator = CurrentUser;
            dal.AddExperience(experience);
            //return View("../Home/Welcome", experience.Creator);
        }

        [HttpGet]
        public PartialViewResult LoadUserExperiences(Guid userId, bool onlyMine)
        {
            List<Experience> allExp = dal.FindExperiences("").Where(x => onlyMine && x.Creator.id == userId || !onlyMine).ToList();
            return PartialView("../Shared/ExperienceList", allExp);
        }

        [HttpPost]
        public void DeleteUserExperience(Guid targetId)
        {
            //Guid targetId = new Guid();
            dal.DeleteExperience(targetId);
        }
    }
}
