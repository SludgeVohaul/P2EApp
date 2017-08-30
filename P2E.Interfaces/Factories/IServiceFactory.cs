using P2E.Interfaces.DataObjects;
using P2E.Interfaces.Services;

namespace P2E.Interfaces.Factories
{
    public interface IServiceFactory
    {
        T CreateService<T>() where T : IService;
        TService CreateService<TService, TClient>(TClient client) where TService : IService where TClient : IClient;
    }
}