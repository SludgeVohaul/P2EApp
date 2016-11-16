namespace P2E.Interfaces.CommandLine
{
    public interface IConsoleSyncOptions
    {
        bool HasMovieCollections { get; }

        bool HasMovieTitle { get; }
        bool HasMovieOriginalTitle { get; }
        bool HasMovieTitleSort { get; }

        bool HasMovieViewCount { get; }
    }
}