namespace P2E.Interfaces.CommandLine.LibraryOptions
{
    public interface IConsoleLibraryOptions<T> where T : IConsoleLibraryOptions<T>
    {
        string LibraryName { get; }
    }
}