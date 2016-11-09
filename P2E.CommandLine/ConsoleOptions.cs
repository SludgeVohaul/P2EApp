using CommandLine;
using CommandLine.Text;
using P2E.Interfaces.CommandLine;
using P2E.Interfaces.CommandLine.LibraryOptions;
using P2E.Interfaces.CommandLine.ServerOptions;

namespace P2E.CommandLine
{
    public class ConsoleOptions : IConsoleOptions, IConsolePlexInstance1ConnectionOptions, IConsoleEmbyInstance1ConnectionOptions, IConsolePlexLibraryOptions, IConsoleEmbyLibraryOptions, IConsoleSyncOptions
    {
        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [Option('a', "plexprotocol", Required = false, DefaultValue = "http", HelpText = "Plex protocol")]
        public string Plex1Protocol { get; set; }
        [Option('b', "plexip", Required = true, HelpText = "Plex ip address")]
        public string Plex1IpAddress { get; set; }
        [Option('c', "plexport", Required = false, DefaultValue = 32400, HelpText = "Plex port")]
        public int Plex1Port { get; set; }

        [Option('x', "embyprotocol", Required = false, DefaultValue = "http", HelpText = "Emby protocol")]
        public string Emby1Protocol { get; set; }
        [Option('y', "embyip", Required = true, HelpText = "Emby ip address")]
        public string Emby1IpAddress { get; set; }
        [Option('z', "embyport", Required = false, DefaultValue = 8096, HelpText = "Emby port")]
        public int Emby1Port { get; set; }

        [Option('l', "plexlibrary", Required = true, HelpText = "Plex library name")]
        public string PlexLibraryName { get; set; }

        [Option('m', "embylibrary", Required = false, DefaultValue = "", HelpText = "Emby library name")]
        public string EmbyLibraryName { get; set; }

        [Option('p', "moviecollections", HelpText = "Sync the movie collections.")]
        public bool HasMovieCollections { get; set; }



        string IConsoleConnectionOptions<IConsolePlexInstance1ConnectionOptions>.Protocol => Plex1Protocol;
        string IConsoleConnectionOptions<IConsolePlexInstance1ConnectionOptions>.IpAddress => Plex1IpAddress;
        int IConsoleConnectionOptions<IConsolePlexInstance1ConnectionOptions>.Port => Plex1Port;

        string IConsoleConnectionOptions<IConsoleEmbyInstance1ConnectionOptions>.Protocol => Emby1Protocol;
        string IConsoleConnectionOptions<IConsoleEmbyInstance1ConnectionOptions>.IpAddress => Emby1IpAddress;
        int IConsoleConnectionOptions<IConsoleEmbyInstance1ConnectionOptions>.Port => Emby1Port;

        string IConsoleLibraryOptions<IConsoleEmbyLibraryOptions>.LibraryName => EmbyLibraryName;
        string IConsoleLibraryOptions<IConsolePlexLibraryOptions>.LibraryName => PlexLibraryName;
    }
}