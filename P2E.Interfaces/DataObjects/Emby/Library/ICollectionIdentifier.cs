namespace P2E.Interfaces.DataObjects.Emby.Library
{
    public interface ICollectionIdentifier : IItemIdentifier
    {
        string Filename { get; }
        string Path { get; }
        string Name { get; }
    }
}