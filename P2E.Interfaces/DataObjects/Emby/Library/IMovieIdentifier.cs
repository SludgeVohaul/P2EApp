namespace P2E.Interfaces.DataObjects.Emby.Library
{
    public interface IMovieIdentifier : IItemIdentifier
    {
        string Filename { get; }
    }
}