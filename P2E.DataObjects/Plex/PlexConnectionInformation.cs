using P2E.Interfaces.CommandLine.ServerOptions;
using P2E.Interfaces.DataObjects;

namespace P2E.DataObjects.Plex
{
    public class PlexConnectionInformation : IConnectionInformation
    {
        public string Protocol { get; }
        public string IpAddress { get; }
        public int Port { get; }
        public string ServerUrl { get; }

        public PlexConnectionInformation(IConsolePlexConnectionOptions consolePlexConnectionOptions)
        {
            Protocol = consolePlexConnectionOptions.PlexProtocol;
            IpAddress = consolePlexConnectionOptions.PlexIpAddress;
            Port = consolePlexConnectionOptions.PlexPort;
            ServerUrl = $"{consolePlexConnectionOptions.PlexProtocol}://{consolePlexConnectionOptions.PlexIpAddress}:{consolePlexConnectionOptions.PlexPort}";
        }
    }
}