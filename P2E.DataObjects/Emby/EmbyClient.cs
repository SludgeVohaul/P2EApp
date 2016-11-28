using System;
using System.Threading.Tasks;
using Emby.ApiInteraction;
using Emby.ApiInteraction.Cryptography;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Logging;
using P2E.Interfaces.DataObjects;
using P2E.Interfaces.DataObjects.Emby;

namespace P2E.DataObjects.Emby
{
    public class EmbyClient : ApiClient, IEmbyClient
    {
        private readonly ILogger _logger;
        public IConnectionInformation ConnectionInformation { get; }

        public EmbyClient(ILogger logger, IDevice device, ICryptographyProvider cryptographyProvider, IConnectionInformation connectionInformation, IApplicationInformation applicationInformation)
            : base(logger, connectionInformation.ServerUrl, applicationInformation.Name, device, applicationInformation.Version, cryptographyProvider)
        {
            _logger = logger;
            ConnectionInformation = connectionInformation;
        }

        public async Task<bool> TryLoginAsync(string loginname, string password)
        {
            try
            {
                // TODO -  handle null values
                var authenticationTask = AuthenticateUserAsync(loginname, password);

                await authenticationTask;
                return authenticationTask.IsFaulted == false;
            }
            catch (Exception ex)
            {
                // TODO - ex.Message is not being logged.
                _logger.ErrorException("Authentication failed", ex, ex.Message);
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            await Logout();
        }
    }
}