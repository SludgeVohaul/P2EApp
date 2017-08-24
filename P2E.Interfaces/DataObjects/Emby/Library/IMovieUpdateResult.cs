namespace P2E.Interfaces.DataObjects.Emby.Library
{
    public interface IMovieUpdateResult
    {
        string Filename { get; }
        string Title { get; }
        bool IsUpdated { get; }
    }
}