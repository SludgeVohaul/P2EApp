using P2E.Interfaces.DataObjects;

namespace P2E.Interfaces.Services
{
    public interface IUserCredentialsService : IService
    {
        IUserCredentials PromptForUserCredentials(IConnectionInformation connectionInformation);
    }
}