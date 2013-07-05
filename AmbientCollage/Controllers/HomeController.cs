using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AmbientCollage.Models;
using AmbientCollage.Abstractions;
using System.Net;
using System.IO;

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
                return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult ReturningUser(User user)
        {
            User setUser = dal.PerformLogin(user.Email, user.PasswordHash);

            if (setUser != null)
            {
                // login success!
                CurrentUser = setUser;
                return RedirectToAction("Welcome");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult NewUser(User user)
        {
            dal.CreateNewUser(user.UserName, user.Email, user.PasswordHash);
            CurrentUser = dal.PerformLogin(user.Email, user.PasswordHash);
            return View("Welcome", CurrentUser);
        }

        private string EnsureStringIsTrackID(string fromUser, string soundCloudClientID)
        {
            // = "098b86397e9b4932fd99618f0a0e99d2";
            string toReturn = "";

            int testInt = 0;
            if (int.TryParse(fromUser, out testInt))
            {
                toReturn = fromUser; // already good to go since it's already an integer
            }
            else if (fromUser.Contains("://soundcloud.com"))
            {
                string toResolve = string.Format("http://api.soundcloud.com/resolve.json?url={0}&client_id={1}", fromUser, soundCloudClientID);

                WebRequest request = WebRequest.Create(toResolve);
                request.Method = "GET";
                
                using(WebResponse response = request.GetResponse())
                {
                    using(Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        string downloadedData = reader.ReadToEnd();

                        var dynamicData = System.Web.Helpers.Json.Decode(downloadedData);

                        if (dynamicData.kind == "track")
                        {
                            toReturn = dynamicData.id.ToString();
                        }
                        else
                        {
                            toReturn = "NOT A TRACK";
                        }
                        
                    }
                }

            }

            return toReturn;
        }

        [HttpPost]
        public void CreateNewExperience(Experience experience)
        {
            string currentClientId = System.Configuration.ConfigurationManager.AppSettings["SoundCloudClientID"];

            try
            {
                experience.Creator = CurrentUser;
                foreach (var sound in experience.Sounds)
                {
                    sound.LinkUrl = EnsureStringIsTrackID(sound.LinkUrl, currentClientId);
                }

                dal.AddExperience(experience);
            }
            catch (Exception ex)
            {
                //todo:  do something with this exception
                Response.Redirect("Error");
            }
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
