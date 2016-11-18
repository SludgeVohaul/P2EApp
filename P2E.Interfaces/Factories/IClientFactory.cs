using P2E.Interfaces.DataObjects;

namespace P2E.Interfaces.Factories
{
    public interface IClientFactory
    {
        T CreateClient<T>(IConnectionInformation connectionInformation) where T : IClient;
    }
}