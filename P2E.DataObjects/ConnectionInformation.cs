using P2E.Interfaces.CommandLine.ServerOptions;
using P2E.Interfaces.DataObjects;

namespace P2E.DataObjects
{
    public class ConnectionInformation<T> : IConnectionInformation<T> where T : IConsoleConnectionOptions<T>
    {
        private readonly T _consoleConnectionOptions;

        public string Protocol => _consoleConnectionOptions.Protocol;
        public string IpAddress => _consoleConnectionOptions.IpAddress;
        public int Port => _consoleConnectionOptions.Port;
        public string ServerUrl => $"{Protocol}://{IpAddress}:{Port}";

        public ConnectionInformation(T consoleConnectionOptions)
        {
            _consoleConnectionOptions = consoleConnectionOptions;
        }
    }
}