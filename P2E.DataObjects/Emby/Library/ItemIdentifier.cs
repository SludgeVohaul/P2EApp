using P2E.Interfaces.DataObjects.Emby.Library;

namespace P2E.DataObjects.Emby.Library
{
    public abstract class ItemIdentifier : IItemIdentifier
    {
        public string Id { get; set; }
    }
}