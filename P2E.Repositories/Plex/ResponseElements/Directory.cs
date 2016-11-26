using RestSharp.Deserializers;

namespace P2E.Repositories.Plex.ResponseElements
{
    public class Directory
    {
        [DeserializeAs(Name = "title")]
        public string Title { get; set; }
        [DeserializeAs(Name = "key")]
        public string Key { get; set; }
    }
}
