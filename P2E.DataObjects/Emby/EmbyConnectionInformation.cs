using P2E.Interfaces.CommandLine.ServerOptions;
using P2E.Interfaces.DataObjects;

namespace P2E.DataObjects.Emby
{
    public class EmbyConnectionInformation : IConnectionInformation
    {
        public string Protocol { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string ServerUrl { get; }

        public EmbyConnectionInformation(IConsoleEmbyConnectionOptions consoleEmbyConnectionOptions)
        {
            Protocol = consoleEmbyConnectionOptions.EmbyProtocol;
            IpAddress = consoleEmbyConnectionOptions.EmbyIpAddress;
            Port = consoleEmbyConnectionOptions.EmbyPort;
            ServerUrl = $"{consoleEmbyConnectionOptions.EmbyProtocol}://{consoleEmbyConnectionOptions.EmbyIpAddress}:{consoleEmbyConnectionOptions.EmbyPort}";
        }
    }
}