using P2E.Interfaces.CommandLine.ServerOptions;
using P2E.Interfaces.DataObjects;

namespace P2E.DataObjects
{
    public class ConnectionInformation<T> : IConnectionInformation<T> where T : IConsoleConnectionOptions<T>
    {
        public string Protocol { get; }
        public string IpAddress { get; }
        public int Port { get; }
        public string ServerUrl { get; }

        public ConnectionInformation(T consoleConnectionOptions)
        {
            Protocol = consoleConnectionOptions.Protocol;
            IpAddress = consoleConnectionOptions.IpAddress;
            Port = consoleConnectionOptions.Port;
            ServerUrl = $"{consoleConnectionOptions.Protocol}://{consoleConnectionOptions.IpAddress}:{consoleConnectionOptions.Port}";
        }
    }
}