using P2E.Interfaces.DataObjects.Emby;

namespace P2E.Interfaces.Services
{
    public interface IEmbyConnectionService
    {
        bool TryLogin(IEmbyClient embyClient, IUserCredentialsService userCredentialsService);
        void Logout(IEmbyClient embyClient);
    }
}