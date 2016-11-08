using P2E.Interfaces.DataObjects;
using P2E.Interfaces.DataObjects.Emby;

namespace P2E.Interfaces.Factories
{
    public interface IEmbyClientFactory
    {
        IEmbyClient CreateEmbyClient(IConnectionInformation connectionInformation);
    }
}