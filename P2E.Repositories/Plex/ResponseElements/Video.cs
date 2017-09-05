using System.Collections.Generic;
using RestSharp.Deserializers;

namespace P2E.Repositories.Plex.ResponseElements
{
    public class Video
    {
        public List<Collection> Collections { get; set; }
        public List<Media> Medias { get; set; }

        [DeserializeAs(Name = "originalTitle")]
        public string OriginalTitle { get; set; }
        [DeserializeAs(Name = "title")]
        public string Title { get; set; }
        [DeserializeAs(Name = "titleSort")]
        public string TitleSort { get; set; }
        [DeserializeAs(Name = "viewCount")]
        public string ViewCount { get; set; }
        [DeserializeAs(Name = "thumb")]
        public string Thumb { get; set; }
        [DeserializeAs(Name = "art")]
        public string Art { get; set; }
    }
}
