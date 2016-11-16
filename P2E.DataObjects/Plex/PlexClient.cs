using System.Threading.Tasks;
using MediaBrowser.Model.Logging;
using P2E.Interfaces.DataObjects;
using P2E.Interfaces.DataObjects.Plex;
using RestSharp;

namespace P2E.DataObjects.Plex
{
    public class PlexClient : IPlexClient
    {
        private RestClient _client;

        public IConnectionInformation ConnectionInformation { get; }

        public PlexClient(ILogger logger, IConnectionInformation connectionInformation)
        {
            ConnectionInformation = connectionInformation;
            _client = new RestClient(connectionInformation.ServerUrl);
        }

        public async Task LoginAsync(string loginname, string password)
        {
            // Do nothing here, as plex auth is not supported.
            await Task.FromResult(0);
        }
        public async Task LogoutAsync()
        {
            // Do nothing here, as plex auth is not supported.
            await Task.FromResult(0);
        }

    }
}
