using P2E.Interfaces.Services;

namespace P2E.Interfaces.Factories
{
    public interface IServiceFactory
    {
        IEmbyConnectionService CreateEmbyConnectionService();
        IItemSearchService CreateItemSearchService();
        IUserCredentialsService CreateUserCredentialsService();
    }
}