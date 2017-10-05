using P2E.Interfaces.DataObjects.Emby.Library;

namespace P2E.DataObjects.Emby.Library
{
    public class EmbyMovieMetadata : IEmbyMovieMetadata
    {
        public string Name { get; set; }
        public string ForcedSortName { get; set; }
    }
}