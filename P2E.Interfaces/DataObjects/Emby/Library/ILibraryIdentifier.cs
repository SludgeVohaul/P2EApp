namespace P2E.Interfaces.DataObjects.Emby.Library
{
    public interface ILibraryIdentifier : IItemIdentifier
    {
        string Name { get; }
    }
}