namespace P2E.Interfaces.DataObjects.Emby.Library
{
    public interface ICollectionIdentifier : IItemIdentifier
    {
        string PathBasename { get; }
    }
}