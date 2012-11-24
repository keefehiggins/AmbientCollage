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
                Sounds = toDehydrate.Sounds.Select(x => x.id).ToList();
                Visuals = toDehydrate.Visuals.id;
                Description = toDehydrate.Description;
                Share = toDehydrate.Share;
                id = toDehydrate.id;
            }

            public Guid id { get; set; }
            public Guid Creator { get; set; }
            public List<Guid> Sounds { get; set; }
            public Guid Visuals { get; set; }
            public string Description { get; set; }
            public bool Share { get; set; }
        }

        private Experience rehydrateExperience(ShortExperience dehydrated)
        {
            User user = GetUserByID(dehydrated.Creator);
            List<AudioLink> sounds = new List<AudioLink>();

            if (dehydrated.Sounds != null)
            {
                sounds = dehydrated.Sounds.Select(x => GetAudioLinkById(x)).ToList();
            }

            ImageLink visuals = GetImageLinkById(dehydrated.Visuals);

            Experience rehydrated = new Experience(sounds, visuals, user, dehydrated.Description, dehydrated.Share);
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
            var query = Query.EQ("_id", userId);
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

            if (toLogin != null && toLogin.PasswordHash == securityLayer.EncryptPassword(password))
            {
                return toLogin;
            }
            else
            {
                return null;
            }
        }

        public void CreateNewUser(string userName, string email, string password)
        {
            User toSave = new User(userName, securityLayer.EncryptPassword(password), email);
            MongoCollection<User> users = db.GetCollection<User>("Users");
            users.Insert(toSave);
        }

        public AudioLink AddAudioLink(string link, User foundBy, string description)
        {
            MongoCollection<AudioLink> links = db.GetCollection<AudioLink>("AudioLinks");
            AudioLink toInsert = new AudioLink(link, description, foundBy.UserName);
            links.Insert(toInsert);
            return toInsert;
        }

        public IEnumerable<AudioLink> FindAudioLinks(string searchString)
        {
            List<AudioLink> returnMe = null;
            MongoCollection<AudioLink> links = db.GetCollection<AudioLink>("AudioLinks");

            returnMe = (from l in links.AsQueryable<AudioLink>()
                        orderby l.id
                        where l.Description.Contains(searchString)
                        select l).ToList();

            return returnMe;
        }

        public AudioLink GetAudioLinkById(Guid audioLinkId)
        {
            AudioLink returnMe = null;
            MongoCollection<AudioLink> users = db.GetCollection<AudioLink>("AudioLinks");
            var query = Query.EQ("_id", audioLinkId);
            returnMe = users.Find(query).FirstOrDefault();
            return returnMe;
        }

        public ImageLink AddImageLink(string link, User foundBy, string description)
        {
            MongoCollection<ImageLink> links = db.GetCollection<ImageLink>("ImageLinks");
            ImageLink toInsert = new ImageLink(link, description, foundBy.UserName);
            links.Insert(toInsert);
            return toInsert;
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
            var query = Query.EQ("_id", imageLinkId);
            returnMe = links.Find(query).FirstOrDefault();
            return returnMe;
        }

        public void AddExperience(List<AudioLink> audioLinks, ImageLink imageLink, User builtBy, string description, bool share)
        {
            MongoCollection<ShortExperience> links = db.GetCollection<ShortExperience>("Experiences");
            ShortExperience dehydratedExperience = new ShortExperience(new Experience(audioLinks, imageLink, builtBy, description, share));
            links.Insert(dehydratedExperience);
        }

        public void AddExperience(Experience experience)
        {
            MongoCollection<ShortExperience> links = db.GetCollection<ShortExperience>("Experiences");

            List<AudioLink> savedSounds = new List<AudioLink>();
            foreach (AudioLink sound in experience.Sounds ?? new List<AudioLink>())
            {
                savedSounds.Add(AddAudioLink(sound.LinkUrl, experience.Creator, sound.Description));
            }
            experience.Sounds = savedSounds;
            experience.Visuals = AddImageLink(experience.Visuals.LinkUrl, experience.Creator, experience.Visuals.Description);

            ShortExperience dehydratedExperience = new ShortExperience(experience);
            links.Insert(dehydratedExperience);
        }

        public IEnumerable<Experience> FindExperiences(string searchString)
        {
            List<Experience> returnMe = null;
            MongoCollection<ShortExperience> experiences = db.GetCollection<ShortExperience>("Experiences");

            returnMe = (from e in experiences.AsQueryable<ShortExperience>()
                        orderby e.id
                        //where e.Description.Contains(searchString)
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