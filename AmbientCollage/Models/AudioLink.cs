﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmbientCollage.Models
{
    [Serializable]
    public class AudioLink
    {
        public AudioLink()
        {

        }

        public AudioLink(string url, string description, string user)
        {
            LinkUrl = url;
            Description = description;
            FoundByUsername = user;
        }

        public Guid id { get; set; }
        public string LinkUrl { get; set; }
        public string Description { get; set; }
        public string FoundByUsername { get; set; }
    }
}