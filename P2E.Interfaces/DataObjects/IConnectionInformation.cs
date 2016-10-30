namespace P2E.Interfaces.DataObjects
{
    public interface IConnectionInformation
    {
        string Protocol { get; }
        string IpAddress { get; }
        int Port { get; }
        string ServerUrl { get; }
    }
}