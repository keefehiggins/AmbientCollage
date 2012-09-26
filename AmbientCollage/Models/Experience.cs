using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmbientCollage.Models
{
    [Serializable]
    public class Experience
    {
        public Experience(Dictionary<AudioLinkType, AudioLink> audio, ImageLink visual, User createdBy, string description)
        {
            Creator = createdBy;
            Foreground = audio[AudioLinkType.Music];
            Background = audio[AudioLinkType.Background];
            Visuals = visual;
            Description = description;
        }

        public Guid id { get; set; }
        public User Creator { get; set; }
        public AudioLink Foreground { get; set; }
        public AudioLink Background { get; set; }
        public ImageLink Visuals { get; set; }
        public string Description { get; set; }
    }
}