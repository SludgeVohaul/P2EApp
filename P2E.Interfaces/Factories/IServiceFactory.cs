using P2E.Interfaces.Services;

namespace P2E.Interfaces.Factories
{
    public interface IServiceFactory
    {
        T CreateService<T>() where T : IService;
    }
}