using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Events;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Users;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.Services;

namespace P2E.Repositories
{
    public abstract class EmbyBaseRepository
    {
        private readonly object _lockObject = new object();

        private readonly IEmbyConnectionService _embyConnectionService;
        private readonly IUserCredentialsService _userCredentialsService;

        private AuthenticationResult _authResult;
 
        protected readonly IEmbyClient EmbyClient;
        protected readonly ILogger Logger;

        
        protected EmbyBaseRepository(ILogger logger, IUserCredentialsService userCredentialsService, IEmbyConnectionService embyConnectionService, IEmbyClient embyClient)
        {
            Logger = logger;
            _userCredentialsService = userCredentialsService;

            _embyConnectionService = embyConnectionService;
            EmbyClient = embyClient;

            EmbyClient.RemoteLoggedOut += EmbyClient_RemoteLoggedOut;
        }

        public bool TryConnect()
        {
            lock (_lockObject)
            {
                if (_authResult != null)
                {
                    Logger.Error("Client is already authenticated!");
                    return false;
                }
            }
            
            _authResult = _embyConnectionService.Login(EmbyClient, _userCredentialsService);

            lock (_lockObject)
            {
                if (_authResult != null) return true;

                Logger.Error("Failed to authenticate!");
                return false;
            }
        }


        public void Disconnect()
        {
            lock (_lockObject)
            {
                if (_authResult == null) return;
            }
            
            _embyConnectionService.Logout(EmbyClient);

            lock (_lockObject)
            {
                _authResult = null;
            }
        }

        private void EmbyClient_RemoteLoggedOut(object sender, GenericEventArgs<RemoteLogoutReason> e)
        {
            var remoteLogoutReason = e.Argument;
            switch (remoteLogoutReason)
            {
                case RemoteLogoutReason.GeneralAccesError:
                    Logger.Debug("General Access Error");
                    break;
                case RemoteLogoutReason.ParentalControlRestriction:
                    Logger.Debug("Parental Control");
                    break;
                default:
                    Logger.Debug("Something else");
                    break;
            }

            lock (_lockObject)
            {
                _authResult = null;
            }
        }
    }
}