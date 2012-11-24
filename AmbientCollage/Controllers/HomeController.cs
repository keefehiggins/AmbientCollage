﻿using System;
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
        //DataAccessLayer dal = new DataAccessLayer(new FakeDataAccessLayer(new SimpleSecurity()));

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
        public ViewResult ReturningUser(User user)
        {
            User setUser = dal.PerformLogin(user.Email, user.PasswordHash);

            if (setUser != null)
            {
                // login success!
                HttpContext.Session["CurrentUser"] = setUser;
                return View("Welcome", setUser);
            }
            else
            {
                return View("../Login");
            }
        }

        public ViewResult NewUser(User user)
        {
            dal.CreateNewUser(user.UserName, user.Email, user.PasswordHash);
            User createdUser = dal.PerformLogin(user.Email, user.PasswordHash);
            HttpContext.Session["CurrentUser"] = createdUser;

            return View("Welcome", createdUser);
        }

        [HttpPost]
        public void CreateNewExperience(Experience experience)
        {
            User currentUser = (User)HttpContext.Session["CurrentUser"];
            experience.Creator = currentUser;
            dal.AddExperience(experience);
            //return View("../Home/Welcome", experience.Creator);
        }

        [HttpGet]
        public PartialViewResult LoadUserExperiences(Guid userId)
        {
            List<Experience> allExp = dal.FindExperiences("").ToList();
            return PartialView("../Shared/ExperienceList", allExp);
        }
    }
}
