using System.Threading.Tasks;
using Emby.ApiInteraction;
using Emby.ApiInteraction.Cryptography;
using MediaBrowser.Model.ApiClient;
using P2E.Interfaces.DataObjects;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Services;

namespace P2E.DataObjects.Emby
{
    public class EmbyClient : ApiClient, IEmbyClient
    {
        private IUserCredentials _userCredentials;
        private readonly IConnectionInformation _connectionInformation;

        public string ServerType { get; } = "Emby";

        public EmbyClient(IAppLogger logger, IDevice device, ICryptographyProvider cryptographyProvider,
            IConnectionInformation connectionInformation, IApplicationInformation applicationInformation)
            : base(
                logger, connectionInformation.ServerUrl, applicationInformation.Name, device,
                applicationInformation.Version, cryptographyProvider)
        {
            _connectionInformation = connectionInformation;
        }

        public void SetLoginData(IUserCredentialsService userCredentialsService)
        {
            _userCredentials = userCredentialsService?.PromptForUserCredentials(_connectionInformation, ServerType);
        }

        public async Task LoginAsync()
        {
            var authResult = await AuthenticateUserAsync(_userCredentials?.Loginname, _userCredentials?.Password);
            SetAuthenticationInfo(authResult.AccessToken, authResult.User.Id);
        }

        public async Task LogoutAsync()
        {
            if (AccessToken != null)
            {
                await Logout();
            }
            else
            {
                await Task.Run(() => { });
            }
        }
    }
}