using P2E.Interfaces.DataObjects.Emby.Library;

namespace P2E.DataObjects.Emby.Library
{
    public class FilenameIdentifier : ItemIdentifier, IFilenameIdentifier
    {
        public string Filename { get; set; }
    }
}