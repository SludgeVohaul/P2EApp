using System.Threading;
using System.Threading.Tasks;
using Emby.ApiInteraction;
using Emby.ApiInteraction.Cryptography;
using Emby.ApiInteraction.Net;
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

        public string ServerType => "Emby";

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

        /// <remarks>
        /// This is a copy of the Emby.ApiInteraction.ApiClient.DeleteAsync() method, since the original is private, for whatever reasons.
        /// The also private method SendAsync is replaced with IAsyncHttpClient.SendAsync which "seems" to work...
        /// TODO - Once the ApiClient.DeleteAsync() method is public this method should be deleted.
        /// </remarks>
        public async Task<T> DeleteAsync<T>(string url, CancellationToken cancellationToken = default(CancellationToken)) where T : class
        {
            url = AddDataFormat(url);

            using (var stream = await HttpClient.SendAsync(new HttpRequest
            {
                Url = url,
                CancellationToken = cancellationToken,
                RequestHeaders = HttpHeaders,
                Method = "DELETE"

            }).ConfigureAwait(false))
            {
                return DeserializeFromStream<T>(stream);
            }
        }
    }
}