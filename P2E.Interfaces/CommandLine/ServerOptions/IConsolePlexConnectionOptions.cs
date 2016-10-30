namespace P2E.Interfaces.CommandLine.ServerOptions
{
    public interface IConsolePlexConnectionOptions
    {
        string PlexProtocol { get; set; }
        string PlexIpAddress { get; set; }
        int PlexPort { get; set; }


    }
}