using P2E.Interfaces.DataObjects;
using P2E.Interfaces.Repositories;

namespace P2E.Interfaces.Factories
{
    public interface IRepositoryFactory
    {
        T CreateRepository<T>(IClient embyClient);
    }
}