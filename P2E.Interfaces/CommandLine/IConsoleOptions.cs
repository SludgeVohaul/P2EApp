using CommandLine;

namespace P2E.Interfaces.CommandLine
{
    // TODO - needs refactoring 
    public interface IConsoleOptions
    {
        string GetUsage();

        IParserState LastParserState { get; }

        bool DryRun { get; }

        string Plex1Protocol { get; }
        string Plex1IpAddress { get; }
        int Plex1Port { get; set; }

        string Emby1Protocol { get; }
        string Emby1IpAddress { get; }
        int Emby1Port { get; }
    }
}