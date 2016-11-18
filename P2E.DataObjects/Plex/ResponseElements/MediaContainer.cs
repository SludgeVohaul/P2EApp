using System.Collections.Generic;

namespace P2E.DataObjects.Plex.ResponseElements
{
    public class MediaContainer
    {
        public List<Directory> Directories { get; set; }
        public List<Video> Videos { get; set; }
    }
}
