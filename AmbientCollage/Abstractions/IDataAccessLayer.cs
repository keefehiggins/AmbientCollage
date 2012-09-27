using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AmbientCollage.Models;

namespace AmbientCollage.Abstractions
{
    public interface IDataAccessLayer
    {
        User GetCurrentUser();
        User GetUserByID(Guid userId);
        User GetUserByName(string userName);
        User GetUserByEmail(string email);
        User PerformLogin(string email, string password);
        void CreateNewUser(string userName, string email, string password);
        
        void AddAudioLink(string link, User foundBy, string description, AudioLinkType audioType);
        IEnumerable<AudioLink> FindAudioLinks(string searchString, AudioLinkType audioType);
        AudioLink GetAudioLinkById(Guid audioLinkId);

        void AddImageLink(string link, User foundBy, string description);
        IEnumerable<ImageLink> FindImageLinks(string searchString);
        ImageLink GetImageLinkById(Guid imageLinkId);

        void AddExperience(Dictionary<AudioLinkType, AudioLink> audioLinks, ImageLink imageLink, User builtBy, string description);
        void AddExperience(Experience experience);
        IEnumerable<Experience> FindExperiences(string searchString);
        Experience GetExperienceById(Guid experienceId);
    }

    public class DataAccessLayer : IDataAccessLayer
    {
        private IDataAccessLayer internalDal;

        public DataAccessLayer(IDataAccessLayer dal)
        {
            internalDal = dal;
        }

        public User GetCurrentUser()
        {
            return internalDal.GetCurrentUser();
        }

        public User GetUserByID(Guid userId)
        {
            return internalDal.GetUserByID(userId);
        }

        public User GetUserByName(string userName)
        {
            return internalDal.GetUserByName(userName);
        }

        public User GetUserByEmail(string email)
        {
            return internalDal.GetUserByEmail(email);
        }

        public User PerformLogin(string email, string password)
        {
            return internalDal.PerformLogin(email, password);
        }

        public void CreateNewUser(string userName, string email, string password)
        {
            internalDal.CreateNewUser(userName, email, password);
        }

        public void AddAudioLink(string link, User foundBy, string description, AudioLinkType audioType)
        {
            internalDal.AddAudioLink(link, foundBy, description, audioType);
        }

        public IEnumerable<AudioLink> FindAudioLinks(string searchString, AudioLinkType audioType)
        {
            return internalDal.FindAudioLinks(searchString, audioType);
        }

        public AudioLink GetAudioLinkById(Guid audioLinkId)
        {
            return internalDal.GetAudioLinkById(audioLinkId);
        }

        public void AddImageLink(string link, User foundBy, string description)
        {
            internalDal.AddImageLink(link, foundBy, description);
        }

        public IEnumerable<ImageLink> FindImageLinks(string searchString)
        {
            return internalDal.FindImageLinks(searchString);
        }

        public ImageLink GetImageLinkById(Guid imageLinkId)
        {
            return internalDal.GetImageLinkById(imageLinkId);
        }

        public void AddExperience(Dictionary<AudioLinkType, AudioLink> audioLinks, ImageLink imageLink, User builtBy, string description)
        {
            internalDal.AddExperience(audioLinks, imageLink, builtBy, description);
        }

        public void AddExperience(Experience experience)
        {
            internalDal.AddExperience(experience);
        }

        public IEnumerable<Experience> FindExperiences(string searchString)
        {
            return internalDal.FindExperiences(searchString);
        }

        public Experience GetExperienceById(Guid experienceId)
        {
            return internalDal.GetExperienceById(experienceId);
        }
    }
    
}