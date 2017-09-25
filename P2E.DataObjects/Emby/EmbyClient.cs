using System.Threading;
using System.Threading.Tasks;
using Emby.ApiClient;
using Emby.ApiClient.Cryptography;
using Emby.ApiClient.Model;
using Emby.ApiClient.Net;
using P2E.Interfaces.DataObjects;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Services;

namespace P2E.DataObjects.Emby
{
    public class EmbyClient : ApiClient, IEmbyClient
    {
        private static readonly SemaphoreSlim SemSlim = new SemaphoreSlim(1, 1);

        private IUserCredentials _userCredentials;
        private readonly IConnectionInformation _connectionInformation;

        public string ServerType => "Emby";

        public EmbyClient(IAppLogger logger, IDevice device, ICryptographyProvider cryptographyProvider,
            IConnectionInformation connectionInformation, IApplicationInformation applicationInformation)
            : base(logger, connectionInformation.ServerUrl, applicationInformation.Name, device,
                applicationInformation.Version, cryptographyProvider)
        {
            _connectionInformation = connectionInformation;
        }

        public void SetLoginData(IUserCredentialsService userCredentialsService)
        {
            _userCredentials = userCredentialsService?.PromptForUserCredentials(_connectionInformation, ServerType);
        }

        public async Task<T> SendAsync<T>(string url,
                                  string requestMethod,
                                  QueryStringDictionary args = null,
                                  CancellationToken cancellationToken = default(CancellationToken)) where T : class
        {
            // Client can send only one request at any time.
            await SemSlim.WaitAsync(cancellationToken);
            try
            {
                url = AddDataFormat(url);
                var requestContent = args?.GetQueryString();

                var httpRequest = new HttpRequest
                {
                    Url = url,
                    CancellationToken = cancellationToken,
                    RequestHeaders = HttpHeaders,
                    Method = requestMethod,
                    RequestContentType = "application/x-www-form-urlencoded",
                    RequestContent = requestContent
                };

                using (var stream = await HttpClient.SendAsync(httpRequest).ConfigureAwait(false))
                {
                    return DeserializeFromStream<T>(stream);
                }
            }
            finally
            {
                SemSlim.Release();
            }
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