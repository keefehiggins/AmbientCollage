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
    public class MongoDataAccessLayer : IDataAccessLayer
    {
        private MongoServer server;
        private MongoDatabase db;
        private ISecurity securityLayer;

        [Serializable]
        public class ShortExperience
        {
            public ShortExperience(Experience toDehydrate)
            {
                Creator = toDehydrate.Creator.id;
                Foreground = toDehydrate.Foreground.id;
                Background = toDehydrate.Background.id;
                Visuals = toDehydrate.Visuals.id;
                Description = toDehydrate.Description;
                id = toDehydrate.id;
            }

            public Guid id { get; set; }
            public Guid Creator { get; set; }
            public Guid Foreground { get; set; }
            public Guid Background { get; set; }
            public Guid Visuals { get; set; }
            public string Description { get; set; }
        }

        private Experience rehydrateExperience(ShortExperience dehydrated)
        {
            User user = GetUserByID(dehydrated.Creator);
            AudioLink foreground = GetAudioLinkById(dehydrated.Foreground);
            AudioLink background = GetAudioLinkById(dehydrated.Background);
            ImageLink visuals = GetImageLinkById(dehydrated.Visuals);

            Dictionary<AudioLinkType, AudioLink> fullAudio= new Dictionary<AudioLinkType,AudioLink>();
            fullAudio.Add(AudioLinkType.Music, foreground);
            fullAudio.Add(AudioLinkType.Background, background);

            Experience rehydrated = new Experience(fullAudio, visuals, user, dehydrated.Description);
            rehydrated.id = dehydrated.id;
            return rehydrated;
        }

        public MongoDataAccessLayer(ISecurity security)
        {
            server = MongoServer.Create("mongodb://localhost:27017");
            db = server.GetDatabase("AmbientCollage");
            securityLayer = security;
        }

        public User GetCurrentUser()
        {
            throw new NotImplementedException();
        }

        public User GetUserByID(Guid userId)
        {
            User returnMe = null;
            MongoCollection<User> users = db.GetCollection<User>("Users");
            var query = Query.EQ("id", userId);
            returnMe = users.Find(query).FirstOrDefault();
            return returnMe;
        }

        public User GetUserByName(string userName)
        {
            User returnMe = null;
            MongoCollection<User> users = db.GetCollection<User>("Users");
            var query = Query.EQ("UserName", userName);
            returnMe = users.Find(query).FirstOrDefault();
            return returnMe;
        }

        public User GetUserByEmail(string email)
        {
            User returnMe = null;
            MongoCollection<User> users = db.GetCollection<User>("Users");
            var query = Query.EQ("Email", email);
            returnMe = users.Find(query).FirstOrDefault();
            return returnMe;
        }

        public User PerformLogin(string email, string password)
        {
            User toLogin = GetUserByEmail(email);

            if (toLogin.PasswordHash == securityLayer.EncryptPassword(password))
            {
                return toLogin;
            }
            else
            {
                return null;
            }

            throw new NotImplementedException();
        }

        public void CreateNewUser(string userName, string email, string password)
        {
            User toSave = new User(userName, securityLayer.EncryptPassword(password), email);
            MongoCollection<User> users = db.GetCollection<User>("Users");
            users.Insert(toSave);
        }

        public void AddAudioLink(string link, User foundBy, string description, AudioLinkType audioType)
        {
            MongoCollection<AudioLink> links = db.GetCollection<AudioLink>("AudioLinks");
            AudioLink toInsert = new AudioLink(link, description, foundBy.UserName, audioType);
            links.Insert(toInsert);
        }

        public IEnumerable<AudioLink> FindAudioLinks(string searchString, AudioLinkType audioType)
        {
            List<AudioLink> returnMe = null;
            MongoCollection<AudioLink> links = db.GetCollection<AudioLink>("AudioLinks");

            returnMe = (from l in links.AsQueryable<AudioLink>()
                        orderby l.id
                        where l.Description.Contains(searchString) && ( l.AudioType == audioType || audioType == AudioLinkType.Any)
                        select l).ToList();

            return returnMe;
        }

        public AudioLink GetAudioLinkById(Guid audioLinkId)
        {
            AudioLink returnMe = null;
            MongoCollection<AudioLink> users = db.GetCollection<AudioLink>("AudioLinks");
            var query = Query.EQ("id", audioLinkId);
            returnMe = users.Find(query).FirstOrDefault();
            return returnMe;
        }

        public void AddImageLink(string link, User foundBy, string description)
        {
            MongoCollection<ImageLink> links = db.GetCollection<ImageLink>("ImageLinks");
            ImageLink toInsert = new ImageLink(link, description, foundBy.UserName);
            links.Insert(toInsert);
        }

        public IEnumerable<ImageLink> FindImageLinks(string searchString)
        {
            List<ImageLink> returnMe = null;
            MongoCollection<ImageLink> links = db.GetCollection<ImageLink>("ImageLinks");

            returnMe = (from l in links.AsQueryable<ImageLink>()
                        orderby l.id
                        where l.Description.Contains(searchString)
                        select l).ToList();

            return returnMe;
        }

        public ImageLink GetImageLinkById(Guid imageLinkId)
        {
            ImageLink returnMe = null;
            MongoCollection<ImageLink> links = db.GetCollection<ImageLink>("ImageLinks");
            var query = Query.EQ("id", imageLinkId);
            returnMe = links.Find(query).FirstOrDefault();
            return returnMe;
        }

        public void AddExperience(Dictionary<AudioLinkType, AudioLink> audioLinks, ImageLink imageLink, User builtBy, string description)
        {
            MongoCollection<ShortExperience> links = db.GetCollection<ShortExperience>("Experiences");
            ShortExperience dehydratedExperience = new ShortExperience(new Experience(audioLinks, imageLink, builtBy, description));
            links.Insert(dehydratedExperience);
        }

        public void AddExperience(Experience experience)
        {
            MongoCollection<ShortExperience> links = db.GetCollection<ShortExperience>("Experiences");
            ShortExperience dehydratedExperience = new ShortExperience(experience);
            links.Insert(dehydratedExperience);
        }

        public IEnumerable<Experience> FindExperiences(string searchString)
        {
            List<Experience> returnMe = null;
            MongoCollection<ShortExperience> experiences = db.GetCollection<ShortExperience>("Experiences");

            returnMe = (from e in experiences.AsQueryable<ShortExperience>()
                        orderby e.id
                        where e.Description.Contains(searchString)
                        select rehydrateExperience(e)).ToList();

            return returnMe;
        }

        public Experience GetExperienceById(Guid experienceId)
        {
            Experience returnMe = null;
            MongoCollection<ShortExperience> users = db.GetCollection<ShortExperience>("Experiences");
            var query = Query.EQ("id", experienceId);
            returnMe = rehydrateExperience(users.Find(query).FirstOrDefault());
            return returnMe;
        }
    }
}