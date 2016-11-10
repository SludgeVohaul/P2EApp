using P2E.Interfaces.AppLogic;
using P2E.Interfaces.CommandLine.ServerOptions;
using P2E.Interfaces.DataObjects.Emby;
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
        private IEmbyConnectionService _embyConnectionService;

        private IEmbyClient _embyClient;

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
            var authResult = _embyConnectionService.Login(_embyClient, embyUserCredentials);
            if (authResult == null) return;

            try
            {
                // TODO - handle RemoteLoggedOut?
                //_embyClient.RemoteLoggedOut += EmbyClient_RemoteLoggedOut;
                
                _itemSearchService.TryExecute(_embyClient);
            }
            finally
            {
                _embyConnectionService.Logout(_embyClient);
            }
        }

        private void Initialize()
        {
            _userCredentialsService = _serviceFactory.CreateUserCredentialsService();
            _itemSearchService = _serviceFactory.CreateItemSearchService();
            _embyConnectionService = _serviceFactory.CreateEmbyConnectionService();

            var connectionInformationEmby1 = _connectionInformationFactory.CreateConnectionInformation<IConsoleEmbyInstance1ConnectionOptions>();
            var connectionInformationPlex1 = _connectionInformationFactory.CreateConnectionInformation<IConsolePlexInstance1ConnectionOptions>();

            _embyClient = _clientFactory.CreateEmbyClient(connectionInformationEmby1);
        }
    }
}
