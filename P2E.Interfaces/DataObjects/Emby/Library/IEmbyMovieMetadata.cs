namespace P2E.Interfaces.DataObjects.Emby.Library
{
    public interface IEmbyMovieMetadata
    {
        string OriginalTitle { get; }
        string Name { get; }
        string ForcedSortName { get; }
        string ViewCount { get; }
    }
}