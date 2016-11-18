using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp.Deserializers;

namespace P2E.DataObjects.Plex.ResponseElements
{
    public class Video
    {
        public List<Collection> Collections { get; set; }

        [DeserializeAs(Name = "originalTitle")]
        public string OriginalTitle { get; set; }
        [DeserializeAs(Name = "title")]
        public string Title { get; set; }
        [DeserializeAs(Name = "titleSort")]
        public string TitleSort { get; set; }
        [DeserializeAs(Name = "viewCount")]
        public string ViewCount { get; set; }
    }
}
