using MediaBrowser.Model.Users;
using P2E.Interfaces.DataObjects.Emby;

namespace P2E.Interfaces.Services
{
    public interface IEmbyConnectionService
    {
        AuthenticationResult Login(IEmbyClient embyClient, IUserCredentialsService userCredentialsService);
        void Logout(IEmbyClient embyClient);
    }
}