using System.Collections.Generic;

namespace P2E.Interfaces.DataObjects.Plex.Library
{
    public interface IPlexMovieMetadata
    {
        List<string> Collections { get; set; }
        string OriginalTitle { get; set; }
        string Title { get; set; }
        string TitleSort { get; set; }
        string ViewCount { get; set; }
    }
}