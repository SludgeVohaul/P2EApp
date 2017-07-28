using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using P2E.Interfaces.AppLogic;
using P2E.Interfaces.CommandLine;
using P2E.Interfaces.CommandLine.ServerOptions;
using P2E.Interfaces.DataObjects;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Plex;
using P2E.Interfaces.DataObjects.Plex.Library;
using P2E.Interfaces.Factories;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Services;
using P2E.Interfaces.Services.Emby;
using P2E.Interfaces.Services.Plex;
using P2E.Interfaces.Services.SpinWheel;

namespace P2E.AppLogic
{
    public class Logic : ILogic
    {
        private readonly IAppLogger _logger;
        private readonly IClientFactory _clientFactory;
        private readonly IConnectionInformationFactory _connectionInformationFactory;
        private readonly IServiceFactory _serviceFactory;
        private readonly IConsoleLibraryOptions _consoleLibraryOptions;

        private IUserCredentialsService _userCredentialsService;
        private ISpinWheelService _spinWheelService;
        private IEmbyService _embyService;
        private IPlexService _plexService;

        private IEmbyClient _embyClient;
        private IPlexClient _plexClient;

        public Logic(IAppLogger logger,
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

            var clients = new List<IClient> { _embyClient, _plexClient };

            try
            {
                // FYI: As async access to the console is not possible, login data is collected ahead of the async login tasks.
                // FYI: Each client gets an instance of the credentials service, but it's up to the client to use it or not.
                clients.ForEach(x => x.SetLoginData(_userCredentialsService));

                var loginAllTask = await LoginAllClientsAsync(clients);
                if (loginAllTask == false) return;

                // TODO - handle RemoteLoggedOut?
                //_embyClient.RemoteLoggedOut += EmbyClient_RemoteLoggedOut;

                //await _embyService.DoItAsync(_embyClient);

                var movieMetadataItems = await GetPlexMetadataAsync(_plexClient, _consoleLibraryOptions.PlexLibraryName);
                if (movieMetadataItems == null || movieMetadataItems.Any() == false)
                {
                    _logger.Warn("Found no items to process.");
                    return;
                }

                var updateAllTask = await UpdateAllEmbyMovieMetadataAsync(_embyClient, movieMetadataItems, _consoleLibraryOptions.EmbyLibraryName);
                _logger.Info(updateAllTask ? "Update successful." : "Some items could not be updated.");
            }
            finally
            {
                await LogoutAllClientsAsync(clients);
                _logger.Info("Logic done.");
            }
        }

        private void Initialize()
        {
            _userCredentialsService = _serviceFactory.CreateService<IUserCredentialsService>();
            _embyService = _serviceFactory.CreateService<IEmbyService>();
            _plexService = _serviceFactory.CreateService<IPlexService>();

            _spinWheelService = _serviceFactory.CreateService<ISpinWheelService>();

            var connectionInformationEmby1 = _connectionInformationFactory
                .CreateConnectionInformation<IConsoleEmbyInstance1ConnectionOptions>();
            var connectionInformationPlex1 = _connectionInformationFactory
                .CreateConnectionInformation<IConsolePlexInstance1ConnectionOptions>();

            _embyClient = _clientFactory.CreateClient<IEmbyClient>(connectionInformationEmby1);
            _plexClient = _clientFactory.CreateClient<IPlexClient>(connectionInformationPlex1);
        }

        private async Task<List<IPlexMovieMetadata>> GetPlexMetadataAsync(IPlexClient plexClient, string plexLibraryName)
        {
            using (var cts = new CancellationTokenSource())
            {
                await _spinWheelService.StartSpinWheelAsync(cts.Token);
                var movieMetadata = await _plexService.GetMovieMetadataAsync(plexClient, plexLibraryName);
                _spinWheelService.StopSpinWheel(cts);

                return movieMetadata;
            }
        }

        private async Task<bool> UpdateAllEmbyMovieMetadataAsync(IEmbyClient embyClient, IEnumerable<IPlexMovieMetadata> movieMetadataItems, string embyLibraryName)
        {
            var movieMetadataItemsArr = movieMetadataItems.ToArray();
            _embyService.ItemProcessed += _spinWheelService.OnItemProcessed;

            using (var cts = new CancellationTokenSource())
            {
                await _spinWheelService.StartSpinWheelAsync(movieMetadataItemsArr.Length, cts.Token);
                var updateTask = new Func<IPlexMovieMetadata, Task<bool>>(x => _embyService.UpdateItemAsync(embyClient, x, embyLibraryName));
                var updateTasksCompletedTask = await Task.WhenAll(movieMetadataItemsArr.Take(5).Select(updateTask));
                _spinWheelService.StopSpinWheel(cts);

                return updateTasksCompletedTask.All(x => x);
            }
        }

        private async Task<bool> LoginAllClientsAsync(IEnumerable<IClient> clients)
        {
            using (var cts = new CancellationTokenSource())
            {
                await _spinWheelService.StartSpinWheelAsync(cts.Token);
                var loginAllClientsTask = await Task.WhenAll(clients.Select(LoginClientAsync));
                _spinWheelService.StopSpinWheel(cts);

                return loginAllClientsTask.All(x => x);
            }
        }

        private async Task<bool> LoginClientAsync(IClient client)
        {
            try
            {
                await Task.Delay(4000);
                await client.LoginAsync();
                _logger.Info($"Logged into '{client.ServerType}'.");
                return true;
            }
            catch (Exception ex)
            {
                //var innerExceptions = new Func<Exception, IEnumerable<string>>(x =>
                //{
                //    var inmList = new List<string>();
                //    while (x.InnerException != null)
                //    {
                //        inmList.Add(x.Message);
                //        x = x.InnerException;
                //    }
                //    return inmList;
                //});

                //var asd = innerExceptions(ex);
                //_logger.ErrorException("lalala exception {0} {1} {2}", ex, "aaa", "bbb", "ccc");
                _logger.ErrorException($"{client.ServerType} login failed:", ex, ex.InnerException?.Message, ex.InnerException?.Message);
                //throw;
                return false;
            }
        }

        private async Task<bool> LogoutAllClientsAsync(IEnumerable<IClient> clients)
        {
            using (var cts = new CancellationTokenSource())
            {
                await _spinWheelService.StartSpinWheelAsync(cts.Token);
                var logoutAllClientsTask = await Task.WhenAll(clients.Select(LogoutClientAsync));
                _spinWheelService.StopSpinWheel(cts);

                return logoutAllClientsTask.All(x => x);
            }
        }

        private async Task<bool> LogoutClientAsync(IClient client)
        {
            try
            {
                await Task.Delay(4000);
                await client.LogoutAsync();
                _logger.Info($"Logged out from '{client.ServerType}'.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.ErrorException($"{client.ServerType} logout failed:", ex, ex.Message);
                return false;
            }
        }
    }
}
