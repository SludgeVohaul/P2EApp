using P2E.Interfaces.CommandLine.ServerOptions;

namespace P2E.Interfaces.DataObjects
{
    // TODO - is the constraint necessary?
    public interface IConnectionInformation<T> : IConnectionInformation where T : IConsoleConnectionOptions<T>
    {
    }

    public interface IConnectionInformation
    {
        string Protocol { get; }
        string IpAddress { get; }
        int Port { get; }
        string ServerUrl { get; }
    }
}