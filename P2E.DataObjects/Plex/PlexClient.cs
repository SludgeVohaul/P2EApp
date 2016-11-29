using System.Threading.Tasks;
using P2E.Interfaces.DataObjects;
using P2E.Interfaces.DataObjects.Plex;
using RestSharp;

namespace P2E.DataObjects.Plex
{
    public class PlexClient : RestClient, IPlexClient
    {
        public IConnectionInformation ConnectionInformation { get; }
        public string AccessToken { get; private set; }

        public PlexClient(IConnectionInformation connectionInformation)
            : base(connectionInformation.ServerUrl)
        {
            ConnectionInformation = connectionInformation;
        }

        public async Task LoginAsync(string loginname, string password)
        {
            AccessToken = "";
            // Do nothing here, as plex auth is not supported.
            await Task.Run(() => { });
        }
        public async Task LogoutAsync()
        {
            AccessToken = null;
            // Do nothing here, as plex auth is not supported.
            await Task.Run(() => { });
        }

    }
}
