namespace P2E.Interfaces.DataObjects.Emby.Library
{
    public interface IFilenameIdentifier : IItemIdentifier
    {
        string Filename { get; }
    }
}