using P2E.Interfaces.AppLogic;
using P2E.Interfaces.CommandLine.ServerOptions;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Plex;
using P2E.Interfaces.Factories;
using P2E.Interfaces.Services;
using P2E.Interfaces.Services.Emby;
using P2E.Interfaces.Services.Plex;

namespace P2E.AppLogic
{
    public class Logic : ILogic
    {
        private readonly IClientFactory _clientFactory;
        private readonly IConnectionInformationFactory _connectionInformationFactory;
        private readonly IServiceFactory _serviceFactory;

        private IUserCredentialsService _userCredentialsService;
        private IConnectionService _connectionService;
        private IEmbyService _embyService;
        private IPlexService _plexService;

        private IEmbyClient _embyClient;
        private IPlexClient _plexClient;

        public Logic(IClientFactory clientFactory,
            IConnectionInformationFactory connectionInformationFactory,
            IServiceFactory serviceFactory)
        {
            _clientFactory = clientFactory;
            _connectionInformationFactory = connectionInformationFactory;
            _serviceFactory = serviceFactory;
        }

        public void Run()
        {
            Initialize();

            var embyUserCredentials = _userCredentialsService.PromptForUserCredentials(_embyClient.ConnectionInformation);
            //var plexUserCredentials = _userCredentialsService.PromptForUserCredentials(_plexClient.ConnectionInformation);

            if (_connectionService.TryLogin(_embyClient, embyUserCredentials) == false) return;
            if (_connectionService.TryLogin(_plexClient) == false) return;

            try
            {
                // TODO - handle RemoteLoggedOut?
                //_embyClient.RemoteLoggedOut += EmbyClient_RemoteLoggedOut;
                
                _embyService.TryExecute(_embyClient);
                _plexService.TryExecute(_plexClient);
            }
            finally
            {
                _connectionService.Logout(_embyClient);
                _connectionService.Logout(_plexClient);
            }
        }

        private void Initialize()
        {
            _userCredentialsService = _serviceFactory.CreateService<IUserCredentialsService>();
            _connectionService = _serviceFactory.CreateService<IConnectionService>();
            _embyService = _serviceFactory.CreateService<IEmbyService>();
            _plexService = _serviceFactory.CreateService<IPlexService>();

            var connectionInformationEmby1 = _connectionInformationFactory.CreateConnectionInformation<IConsoleEmbyInstance1ConnectionOptions>();
            var connectionInformationPlex1 = _connectionInformationFactory.CreateConnectionInformation<IConsolePlexInstance1ConnectionOptions>();

            _embyClient = _clientFactory.CreateClient<IEmbyClient>(connectionInformationEmby1);
            _plexClient = _clientFactory.CreateClient<IPlexClient>(connectionInformationPlex1);
        }
    }
}
