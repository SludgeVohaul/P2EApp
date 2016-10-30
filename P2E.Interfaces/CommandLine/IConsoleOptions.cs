using CommandLine;
using P2E.Interfaces.CommandLine.ServerOptions;

namespace P2E.Interfaces.CommandLine
{
    public interface IConsoleOptions : IConsolePlexConnectionOptions, IConsoleEmbyConnectionOptions
    {
        string GetUsage();

        IParserState LastParserState { get; set; }
    }
}