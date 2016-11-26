using System.Threading.Tasks;
using MediaBrowser.Model.Logging;
using P2E.Interfaces.AppLogic;
using P2E.Interfaces.CommandLine;
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
        private readonly ILogger _logger;
        private readonly IClientFactory _clientFactory;
        private readonly IConnectionInformationFactory _connectionInformationFactory;
        private readonly IServiceFactory _serviceFactory;
        private readonly IConsoleLibraryOptions _consoleLibraryOptions;

        private IUserCredentialsService _userCredentialsService;
        private IConnectionService _connectionService;
        private IEmbyService _embyService;
        private IPlexService _plexService;

        private IEmbyClient _embyClient;
        private IPlexClient _plexClient;

        public Logic(ILogger logger,
            IClientFactory clientFactory,
            IConnectionInformationFactory connectionInformationFactory,
            IServiceFactory serviceFactory,
            IConsoleLibraryOptions consoleLibraryOptions)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _connectionInformationFactory = connectionInformationFactory;
            _serviceFactory = serviceFactory;
            _consoleLibraryOptions = consoleLibraryOptions;
        }

        public async Task RunAsync()
        {
            Initialize();

            var embyUserCredentials = _userCredentialsService.PromptForUserCredentials(_embyClient.ConnectionInformation);
            //var plexUserCredentials = _userCredentialsService.PromptForUserCredentials(_plexClient.ConnectionInformation);

            await _connectionService.LoginAsync(_embyClient, embyUserCredentials);
            await _connectionService.LoginAsync(_plexClient);
            _logger.Debug("bin nach beiden loginasync");

            try
            {
                // TODO - handle RemoteLoggedOut?
                //_embyClient.RemoteLoggedOut += EmbyClient_RemoteLoggedOut;

                //_embyService.TryExecute(_embyClient);

                var plexLibraryUrl = await _plexService.GetLibraryUrlAsync(_plexClient, _consoleLibraryOptions.PlexLibraryName);

                if (plexLibraryUrl == null)
                {
                    _logger.Error($"Plex movie library '{_consoleLibraryOptions.PlexLibraryName}' not found!");
                    return;
                }
                _logger.Info("Logic done.");
            }
            finally
            {
               await  _connectionService.LogoutAsync(_embyClient);
               await _connectionService.LogoutAsync(_plexClient);
            }
        }

        private void Initialize()
        {
            _userCredentialsService = _serviceFactory.CreateService<IUserCredentialsService>();
            _connectionService = _serviceFactory.CreateService<IConnectionService>();
            _embyService = _serviceFactory.CreateService<IEmbyService>();
            _plexService = _serviceFactory.CreateService<IPlexService>();

            var connectionInformationEmby1 =
                _connectionInformationFactory.CreateConnectionInformation<IConsoleEmbyInstance1ConnectionOptions>();
            var connectionInformationPlex1 =
                _connectionInformationFactory.CreateConnectionInformation<IConsolePlexInstance1ConnectionOptions>();

            _embyClient = _clientFactory.CreateClient<IEmbyClient>(connectionInformationEmby1);
            _plexClient = _clientFactory.CreateClient<IPlexClient>(connectionInformationPlex1);
        }
    }
}
