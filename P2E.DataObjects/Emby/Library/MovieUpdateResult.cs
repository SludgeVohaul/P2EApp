using P2E.Interfaces.DataObjects.Emby.Library;

namespace P2E.DataObjects.Emby.Library
{
    public class MovieUpdateResult : IMovieUpdateResult
    {
        public string Filename { get; set; }
        public string Title { get; set; }
        public bool IsUpdated { get; set; }
    }
}