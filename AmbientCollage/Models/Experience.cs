using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmbientCollage.Models
{
    [Serializable]
    public class Experience
    {
        public Experience()
        {

        }

        public Experience(List<AudioLink> audio, ImageLink visual, User createdBy, string description, bool share)
        {
            Creator = createdBy;
            Sounds = audio ?? new List<AudioLink>();
            Visuals = visual;
            Description = description;
            Share = share;
        }

        public Guid id { get; set; }
        public User Creator { get; set; }
        public List<AudioLink> Sounds { get; set; }
        public ImageLink Visuals { get; set; }
        public string Description { get; set; }
        public bool Share { get; set; }
    }
}