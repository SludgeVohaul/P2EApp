using System.Collections.Generic;

namespace P2E.Repositories.Plex.ResponseElements
{
    public class MediaContainer
    {
        public List<Directory> Directories { get; set; }
        public List<Video> Videos { get; set; }
    }
}
