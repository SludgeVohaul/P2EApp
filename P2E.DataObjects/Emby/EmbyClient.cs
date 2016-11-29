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
        public IConnectionInformation ConnectionInformation { get; }

        public EmbyClient(ILogger logger, IDevice device, ICryptographyProvider cryptographyProvider, IConnectionInformation connectionInformation, IApplicationInformation applicationInformation)
            : base(logger, connectionInformation.ServerUrl, applicationInformation.Name, device, applicationInformation.Version, cryptographyProvider)
        {
            ConnectionInformation = connectionInformation;
        }

        public async Task LoginAsync(string loginname, string password)
        {
            await AuthenticateUserAsync(loginname, password);
        }

        public async Task LogoutAsync()
        {
            await Logout();
        }
    }
}