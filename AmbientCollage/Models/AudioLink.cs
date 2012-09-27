using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmbientCollage.Models
{
    public enum AudioLinkType { Music, Background, Any };

    [Serializable]
    public class AudioLink
    {
        public AudioLink()
        {

        }

        public AudioLink(string url, string description, string user, AudioLinkType type)
        {
            LinkUrl = url;
            Description = description;
            FoundByUsername = user;
            AudioType = type;
        }

        public Guid id { get; set; }
        public string LinkUrl { get; set; }
        public string Description { get; set; }
        public string FoundByUsername {get; set;}
        public AudioLinkType AudioType { get; set; }
    }
}