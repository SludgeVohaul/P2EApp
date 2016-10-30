namespace P2E.Interfaces.Services
{
    public interface IUserCredentialsService
    {
        string Loginname { get; }
        string Password { get; }
        bool HasUserCredentials { get; }
        void GetUserCredentials();
    }
}