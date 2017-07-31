using System.Threading.Tasks;
using P2E.Interfaces.DataObjects;
using P2E.Interfaces.DataObjects.Plex;
using P2E.Interfaces.Services;
using RestSharp;

namespace P2E.DataObjects.Plex
{
    public class PlexClient : RestClient, IPlexClient
    {
        private IConnectionInformation _connectionInformation;

        public string ServerType => "Plex";
        public string AccessToken { get; private set; }

        public PlexClient(IConnectionInformation connectionInformation)
            : base(connectionInformation.ServerUrl)
        {
            _connectionInformation = connectionInformation;
        }

        public void SetLoginData(IUserCredentialsService userCredentialsService)
        {
        }

        public async Task LoginAsync()
        {
            AccessToken = null;
            // Do nothing here, as plex auth is not supported.
            await Task.Run(() => { });
        }
        public async Task LogoutAsync()
        {
            AccessToken = null;
            // Do nothing here, as plex auth is not supported.
            await Task. Run(() => { });
        }
    }
}
