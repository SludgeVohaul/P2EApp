using System.Collections.Generic;

namespace P2E.Interfaces.DataObjects.Plex.Library
{
    public interface IPlexMovieMetadata
    {
        List<string> Collections { get; }
        List<string> Filenames { get; }

        string OriginalTitle { get; }
        string Title { get; }
        string TitleSort { get; }
        string ViewCount { get; }

        string Thumb { get; }
        string Art { get; }
    }
}