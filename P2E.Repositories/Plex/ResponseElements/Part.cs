using RestSharp.Deserializers;

namespace P2E.Repositories.Plex.ResponseElements
{
    public class Part
    {
        [DeserializeAs(Name = "container")]
        public string Container { get; set; }
        [DeserializeAs(Name = "file")]
        public string FileName { get; set; }
    }
}