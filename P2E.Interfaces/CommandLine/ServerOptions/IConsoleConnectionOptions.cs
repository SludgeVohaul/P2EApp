namespace P2E.Interfaces.CommandLine.ServerOptions
{
    public interface IConsoleConnectionOptions<T> where T : IConsoleConnectionOptions<T>
    {
        string Protocol { get; }
        string IpAddress { get; }
        int Port { get; }
    }
}