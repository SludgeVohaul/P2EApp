using P2E.Interfaces.DataObjects.Emby.Library;

namespace P2E.DataObjects.Emby.Library
{
    public class LibraryIdentifier : ItemIdentifier, ILibraryIdentifier
    {
        public string Name { get; set; }
    }
}