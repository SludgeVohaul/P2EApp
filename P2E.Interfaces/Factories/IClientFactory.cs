using P2E.Interfaces.DataObjects;
using P2E.Interfaces.DataObjects.Emby;

namespace P2E.Interfaces.Factories
{
    public interface IClientFactory
    {
        IEmbyClient CreateEmbyClient(IConnectionInformation connectionInformation);
    }
}