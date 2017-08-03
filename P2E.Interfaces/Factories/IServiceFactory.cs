using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Plex;
using P2E.Interfaces.Services;
using P2E.Interfaces.Services.Emby;
using P2E.Interfaces.Services.Plex;

namespace P2E.Interfaces.Factories
{
    public interface IServiceFactory
    {
        T CreateService<T>() where T : IService;
        IEmbyService CreateService(IEmbyClient client);
        IPlexService CreateService(IPlexClient client);
    }
}