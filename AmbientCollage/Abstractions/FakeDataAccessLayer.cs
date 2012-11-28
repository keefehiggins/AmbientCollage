using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using AmbientCollage.Models;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace AmbientCollage.Abstractions
{
    public class FakeDataAccessLayer : IDataAccessLayer
    {
        private ISecurity securityLayer;
        private List<AudioLink> audioLinks;
        private List<ImageLink> imageLinks;
        private List<Experience> experienceList;

        private User fake1 = new User()
        {
            Email = "Edmund@AlchemicalSoftware.com",
            Favorites = new List<Experience>(),
            id = Guid.Empty,
            PasswordHash = "password",
            UserName = "Edmund"
        };

        private User fake2 = new User()
        {
            Email = "lutemaster@gmail.com",
            Favorites = new List<Experience>(),
            id = Guid.Empty,
            PasswordHash = "password",
            UserName = "FrankieAvocado"
        };

        public FakeDataAccessLayer(ISecurity security)
        {
            securityLayer = security;
            audioLinks = new List<AudioLink>();
            imageLinks = new List<ImageLink>();
            experienceList = new List<Experience>();
        }

        public User GetCurrentUser()
        {
            throw new NotImplementedException();
        }

        public User GetUserByID(Guid userId)
        {
            return fake1;
        }

        public User GetUserByName(string userName)
        {
            return fake1;
        }

        public User GetUserByEmail(string email)
        {
            return fake1;
        }

        public User PerformLogin(string email, string password)
        {
            return fake1;
        }

        public void CreateNewUser(string userName, string email, string password)
        {

        }

        public AudioLink AddAudioLink(string link, User foundBy, string description)
        {
            AudioLink toInsert = new AudioLink(link, description, foundBy.UserName);
            audioLinks.Add(toInsert);
            return toInsert;
        }

        public IEnumerable<AudioLink> FindAudioLinks(string searchString)
        {
            List<AudioLink> returnMe = null;

            returnMe = (from l in audioLinks
                        orderby l.id
                        where l.Description.Contains(searchString)
                        select l).ToList();

            return returnMe;
        }

        public AudioLink GetAudioLinkById(Guid audioLinkId)
        {
            return audioLinks.FirstOrDefault(x => x.id == audioLinkId);
        }

        public ImageLink AddImageLink(string link, User foundBy, string description)
        {
            ImageLink toInsert = new ImageLink(link, description, foundBy.UserName);
            imageLinks.Add(toInsert);
            return toInsert;
        }

        public IEnumerable<ImageLink> FindImageLinks(string searchString)
        {
            List<ImageLink> returnMe = null;

            returnMe = (from l in imageLinks
                        orderby l.id
                        where l.Description.Contains(searchString)
                        select l).ToList();

            return returnMe;
        }

        public ImageLink GetImageLinkById(Guid imageLinkId)
        {
            return imageLinks.FirstOrDefault(x => x.id == imageLinkId);
        }

        public void AddExperience(List<AudioLink> audioLinks, ImageLink imageLink, User builtBy, string description, bool share)
        {
            Experience newExp = new Experience(audioLinks, imageLink, builtBy, description, share);
            experienceList.Add(newExp);
        }

        public void AddExperience(Experience experience)
        {
            experienceList.Add(experience);
        }

        public void DeleteExperience(Guid experienceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Experience> FindExperiences(string searchString)
        {
            List<Experience> returnMe = null;

            returnMe = (from e in experienceList
                        orderby e.id
                        //where e.Description.Contains(searchString)
                        select e).ToList();

            return returnMe;
        }

        public Experience GetExperienceById(Guid experienceId)
        {
            return experienceList.FirstOrDefault(x => x.id == experienceId);
        }
    }
}