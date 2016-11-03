using MediaBrowser.Model.Users;
using P2E.Interfaces.DataObjects;
using P2E.Interfaces.DataObjects.Emby;

namespace P2E.Interfaces.Services
{
    public interface IEmbyConnectionService
    {
        AuthenticationResult Login(IEmbyClient embyClient, IUserCredentials userCredentials);
        void Logout(IEmbyClient embyClient);
    }
}