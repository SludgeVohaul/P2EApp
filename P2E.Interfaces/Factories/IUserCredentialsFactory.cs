using P2E.Interfaces.DataObjects;

namespace P2E.Interfaces.Factories
{
    public interface IUserCredentialsFactory
    {
        IUserCredentials CreateUserCredentials();
    }
}