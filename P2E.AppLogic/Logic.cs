using P2E.Interfaces.AppLogic;
using P2E.Interfaces.CommandLine.ServerOptions;
using P2E.Interfaces.DataObjects;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.Factories;
using P2E.Interfaces.Services;

namespace P2E.AppLogic
{
    public class Logic : ILogic
    {
        private readonly IEmbyClientFactory _embyClientFactory;
        private readonly IConnectionInformationFactory _connectionInformationFactory;
        private readonly IServiceFactory _serviceFactory;

        private IItemSearchService _itemSearchService;
        private IUserCredentialsService _userCredentialsService;
        private IEmbyConnectionService _embyConnectionService;

        private IConnectionInformation _connectionInformationEmby1;
        private IConnectionInformation _connectionInformationPlex1;

        private IUserCredentials _embyUserCredentials;
        private IEmbyClient _embyClient;

        public Logic(IEmbyClientFactory embyClientFactory,
            IConnectionInformationFactory connectionInformationFactory,
            IServiceFactory serviceFactory)
        {
            _embyClientFactory = embyClientFactory;
            _connectionInformationFactory = connectionInformationFactory;
            _serviceFactory = serviceFactory;
        }

        public void Run()
        {
            Initialize();

            var authResult = _embyConnectionService.Login(_embyClient, _embyUserCredentials);
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

            _connectionInformationEmby1 = _connectionInformationFactory.CreateConnectionInformation<IConsoleEmbyInstance1ConnectionOptions>();
            _connectionInformationPlex1 = _connectionInformationFactory.CreateConnectionInformation<IConsolePlexInstance1ConnectionOptions>();

            _embyClient = _embyClientFactory.CreateEmbyClient(_connectionInformationEmby1);
            _embyUserCredentials = _userCredentialsService.PromptForUserCredentials(_connectionInformationEmby1);
        }
    }
}
