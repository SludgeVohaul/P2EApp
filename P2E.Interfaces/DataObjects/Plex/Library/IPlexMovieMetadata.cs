using System;
using System.Collections.Generic;

namespace P2E.Interfaces.DataObjects.Plex.Library
{
    public interface IPlexMovieMetadata
    {
        List<string> Collections { get; }
        List<string> Filenames { get; }

        string Title { get; }
        string TitleSort { get; }

        int? ViewCount { get; }
        long? LastViewedAt { get; }

        Uri ThumbUri { get; }
        Uri ArtUri { get; }
    }
}