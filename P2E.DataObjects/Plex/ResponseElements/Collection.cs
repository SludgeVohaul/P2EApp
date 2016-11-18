using RestSharp.Deserializers;

namespace P2E.DataObjects.Plex.ResponseElements
{
    public class Collection
    {
        [DeserializeAs(Name = "tag")]
        public string Tag { get; set; }
    }
}
