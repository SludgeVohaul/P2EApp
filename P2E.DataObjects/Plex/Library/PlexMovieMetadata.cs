using System;
using System.Collections.Generic;
using P2E.Interfaces.DataObjects.Plex.Library;

namespace P2E.DataObjects.Plex.Library
{
    public class PlexMovieMetadata : IPlexMovieMetadata
    {
        public List<string> Collections { get; set; }
        public List<string> Filenames { get; set; }

        public string Title { get; set; }
        public string TitleSort { get; set; }

        public int? ViewCount { get; set; }
        public long? LastViewedAt { get; set; }

        public Uri ThumbUri { get; set; }
        public Uri ArtUri { get; set; }
    }
}