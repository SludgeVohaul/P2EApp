using CommandLine;
using CommandLine.Text;
using P2E.Interfaces.CommandLine;

namespace P2E.CommandLine
{
    public class ConsoleOptions : IConsoleOptions
    {
        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        [ParserState]
        public IParserState LastParserState { get; set; }

        //[Option('a', "plexprotocol", Required = false, HelpText = "Plex protocol")]
        public string PlexProtocol { get; set; } = "http";

        [Option('b', "plexip", Required = true, HelpText = "Plex ip address")]
        public string PlexIpAddress { get; set; }

        [Option('c', "plexport", Required = false, DefaultValue = 32400, HelpText = "Plex port")]
        public int PlexPort { get; set; }

        //[Option('x', "embyprotocol", Required = false, HelpText = "Emby protocol")]
        public string EmbyProtocol { get; set; } = "http";

        [Option('y', "embyip", Required = true, HelpText = "Emby ip address")]
        public string EmbyIpAddress { get; set; }

        [Option('z', "embyport", Required = false, DefaultValue = 8096, HelpText = "Emby port")]
        public int EmbyPort { get; set; }
    }
}