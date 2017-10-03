using P2E.Interfaces.DataObjects.Emby.Library;

namespace P2E.DataObjects.Emby.Library
{
    public class CollectionIdentifier : ItemIdentifier, ICollectionIdentifier
    {
        public string Path { get; set; }
        public string Filename => System.IO.Path.GetFileName(Path)?.Replace(" [boxset]", "");
        public string Name { get; set; }
    }
}