using CommandLine;

namespace P2E.Interfaces.CommandLine
{
    // TODO - needs refactoring 
    public interface IConsoleOptions
    {
        string GetUsage();

        IParserState LastParserState { get; set; }

        string Plex1Protocol { get; set; }
        string Plex1IpAddress { get; set; }
        int Plex1Port { get; set; }

        string Emby1Protocol { get; set; }
        string Emby1IpAddress { get; set; }
        int Emby1Port { get; set; }

        string EmbyLibraryName { get; set; }
        string PlexLibraryName { get; set; }
    }
}