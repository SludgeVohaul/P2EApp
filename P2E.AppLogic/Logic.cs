using P2E.Interfaces.AppLogic;
using P2E.Interfaces.CommandLine.ServerOptions;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Plex;
using P2E.Interfaces.Factories;
using P2E.Interfaces.Services;

namespace P2E.AppLogic
{
    public class Logic : ILogic
    {
        private readonly IClientFactory _clientFactory;
        private readonly IConnectionInformationFactory _connectionInformationFactory;
        private readonly IServiceFactory _serviceFactory;

        private IItemSearchService _itemSearchService;
        private IUserCredentialsService _userCredentialsService;
        private IConnectionService _connectionService;

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
                
                _itemSearchService.TryExecute(_embyClient);
            }
            finally
            {
                _connectionService.Logout(_embyClient);
                _connectionService.Logout(_plexClient);
            }
        }

        private void Initialize()
        {
            _userCredentialsService = _serviceFactory.CreateUserCredentialsService();
            _itemSearchService = _serviceFactory.CreateItemSearchService();
            _connectionService = _serviceFactory.CreateConnectionService();

            var connectionInformationEmby1 = _connectionInformationFactory.CreateConnectionInformation<IConsoleEmbyInstance1ConnectionOptions>();
            var connectionInformationPlex1 = _connectionInformationFactory.CreateConnectionInformation<IConsolePlexInstance1ConnectionOptions>();

            _embyClient = _clientFactory.CreateClient<IEmbyClient>(connectionInformationEmby1);
            _plexClient = _clientFactory.CreateClient<IPlexClient>(connectionInformationPlex1);
        }
    }
}
