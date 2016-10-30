using MediaBrowser.Model.Logging;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.Services;

namespace P2E.Repositories
{
    public abstract class EmbyBaseRepository
    {
        private readonly IEmbyConnectionService _embyConnectionService;
        private readonly IUserCredentialsService _userCredentialsService;

        protected readonly IEmbyClient EmbyClient;
        protected readonly ILogger Logger;
        
        protected EmbyBaseRepository(ILogger logger, IUserCredentialsService userCredentialsService, IEmbyConnectionService embyConnectionService, IEmbyClient embyClient)
        {
            Logger = logger;
            _userCredentialsService = userCredentialsService;

            _embyConnectionService = embyConnectionService;
            EmbyClient = embyClient;
        }

        public bool TryConnect()
        {
            if (_embyConnectionService.TryLogin(EmbyClient, _userCredentialsService)) return true;

            Logger.Error("Failed to authenticate!");
            return false;
        }

        public void Disconnect()
        {
            // TODO - implement magic to disconnect only when client is still connected.
            _embyConnectionService.Logout(EmbyClient);
        }
    }
}