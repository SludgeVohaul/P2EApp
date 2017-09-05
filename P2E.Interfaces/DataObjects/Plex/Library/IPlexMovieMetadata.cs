using System;
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

        Uri ThumbUri { get; }
        Uri ArtUri { get; }
    }
}