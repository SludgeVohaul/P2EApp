using P2E.Interfaces.DataObjects.Emby.Library;

namespace P2E.DataObjects.Emby.Library
{
    public class MovieIdentifier : ItemIdentifier, IMovieIdentifier
    {
        public string Filename { get; set; }
    }
}