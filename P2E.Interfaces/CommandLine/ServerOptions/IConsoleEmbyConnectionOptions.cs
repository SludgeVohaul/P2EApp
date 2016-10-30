namespace P2E.Interfaces.CommandLine.ServerOptions
{
    public interface IConsoleEmbyConnectionOptions
    {
        string EmbyProtocol { get; set; }
        string EmbyIpAddress { get; set; }
        int EmbyPort { get; set; }
    }
}