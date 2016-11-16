using P2E.Interfaces.Services;

namespace P2E.Interfaces.Factories
{
    public interface IServiceFactory
    {
        IConnectionService CreateConnectionService();
        IItemSearchService CreateItemSearchService();
        IUserCredentialsService CreateUserCredentialsService();
    }
}