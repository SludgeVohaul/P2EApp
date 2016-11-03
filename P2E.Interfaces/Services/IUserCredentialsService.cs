using P2E.Interfaces.DataObjects;

namespace P2E.Interfaces.Services
{
    public interface IUserCredentialsService
    {
        IUserCredentials GetUserCredentials(IConnectionInformation connectionInformation);
    }
}