using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Logging;
using P2E.Interfaces.AppLogic;
using P2E.Interfaces.CommandLine;
using P2E.Interfaces.CommandLine.ServerOptions;
using P2E.Interfaces.DataObjects;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Plex;
using P2E.Interfaces.DataObjects.Plex.Library;
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
            IUserCredentials plexUserCredentials = null;

            var spinWheel = new SpinWheel(_logger);

            try
            {
                if (await LoginClients(spinWheel, _embyClient, embyUserCredentials, _plexClient, plexUserCredentials) == false) return;

                // TODO - handle RemoteLoggedOut?
                //_embyClient.RemoteLoggedOut += EmbyClient_RemoteLoggedOut;

                //_embyService.TryExecute(_embyClient);

                var movieMetadataItems = await GetPlexMetadata(spinWheel, _plexClient, _consoleLibraryOptions.PlexLibraryName);
                if (movieMetadataItems == null) return;

                movieMetadataItems.ForEach(x => _logger.Debug($"{x.OriginalTitle}"));

                _logger.Info("Logic done.");
            }
            finally
            {
                await LogoutClients(spinWheel, _embyClient, _plexClient);
            }
        }

        private void Initialize()
        {
            _userCredentialsService = _serviceFactory.CreateService<IUserCredentialsService>();
            _connectionService = _serviceFactory.CreateService<IConnectionService>();
            _embyService = _serviceFactory.CreateService<IEmbyService>();
            _plexService = _serviceFactory.CreateService<IPlexService>();

            var connectionInformationEmby1 = _connectionInformationFactory
                .CreateConnectionInformation<IConsoleEmbyInstance1ConnectionOptions>();
            var connectionInformationPlex1 = _connectionInformationFactory
                .CreateConnectionInformation<IConsolePlexInstance1ConnectionOptions>();

            _embyClient = _clientFactory.CreateClient<IEmbyClient>(connectionInformationEmby1);
            _plexClient = _clientFactory.CreateClient<IPlexClient>(connectionInformationPlex1);
        }

        private async Task<List<IPlexMovieMetadata>> GetPlexMetadata(SpinWheel spinWheel, IPlexClient plexClient,
            string plexLibraryName)
        {
            using (var cts = new CancellationTokenSource())
            {
                var spinTask = spinWheel.SpinAsync(cts.Token);
                try
                {
                    string libraryUrl;
                    try
                    {
                        libraryUrl = await _plexService.GetLibraryUrlAsync(plexClient, plexLibraryName);
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorException($"Failed to retrieve Plex library \"{plexLibraryName}\":", ex, ex.Message);
                        return null;
                    }

                    if (libraryUrl == null)
                    {
                        _logger.Error($"Library {plexLibraryName} not found!");
                        return null;
                    }

                    List<IPlexMovieMetadata> movieMetadataList;
                    try
                    {
                        movieMetadataList = await _plexService.GetMovieMetadataAsync(plexClient, libraryUrl);
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorException($"Failed to retrieve metadata for Plex library \"{plexLibraryName}\":", ex, ex.Message);
                        return null;
                    }

                    return movieMetadataList;
                }
                finally
                {
                    cts.Cancel();
                    await spinTask;
                    _logger.Debug($"Duration: {spinWheel.SpinDuration}ms");
                }
            }
        }

        private async Task<bool> LoginClients(SpinWheel spinWheel, IClient embyClient,
            IUserCredentials embyUserCredentials,
            IClient plexClient, IUserCredentials plexUserCredentials)
        {
            using (var cts = new CancellationTokenSource())
            {
                var spinTask = spinWheel.SpinAsync(cts.Token);
                try
                {
                    try
                    {
                        await _connectionService.LoginAsync(embyClient, embyUserCredentials);
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorException("Emby login failed:", ex, ex.Message);
                        return false;
                    }

                    try
                    {
                        await _connectionService.LoginAsync(plexClient, plexUserCredentials);
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorException("Plex login failed:", ex, ex.Message);
                        return false;
                    }

                    return true;
                }
                finally
                {
                    cts.Cancel();
                    await spinTask;
                    _logger.Debug($"Duration: {spinWheel.SpinDuration}ms");
                }
            }
        }

        private async Task LogoutClients(SpinWheel spinWheel, IClient embyClient, IClient plexClient)
        {
            using (var cts = new CancellationTokenSource())
            {
                var spinTask = spinWheel.SpinAsync(cts.Token);
                try
                {
                    if (_embyClient.AccessToken != null)
                    {
                        await _connectionService.LogoutAsync(embyClient);
                        _logger.Info("Disconnected from Emby.");
                    }

                    if (_plexClient.AccessToken != null)
                    {
                        await _connectionService.LogoutAsync(plexClient);
                        _logger.Info("Disconnected from Plex.");
                    }
                }
                finally
                {
                    cts.Cancel();
                    await spinTask;
                    _logger.Debug($"Duration: {spinWheel.SpinDuration}ms");
                }
            }
        }
    }
}
