using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using P2E.Interfaces.AppLogic;
using P2E.Interfaces.CommandLine.ServerOptions;
using P2E.Interfaces.DataObjects;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Plex;
using P2E.Interfaces.DataObjects.Plex.Library;
using P2E.Interfaces.Factories;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Services;
using P2E.Interfaces.Services.SpinWheel;

namespace P2E.AppLogic
{
    public class MainLogic : IMainLogic
    {
        private readonly IAppLogger _logger;
        private readonly ILogicFactory _logicFactory;
        private readonly IClientFactory _clientFactory;
        private readonly IConnectionInformationFactory _connectionInformationFactory;
        private readonly IServiceFactory _serviceFactory;

        public MainLogic(IAppLogger logger,
            ILogicFactory logicFactory,
            IClientFactory clientFactory,
            IConnectionInformationFactory connectionInformationFactory,
            IServiceFactory serviceFactory)
        {
            _logger = logger;
            _logicFactory = logicFactory;
            _clientFactory = clientFactory;
            _connectionInformationFactory = connectionInformationFactory;
            _serviceFactory = serviceFactory;
        }

        public async Task<bool> RunAsync()
        {
            //Initialize();

            var plexExportLogic = _logicFactory.CreateLogic<IPlexExportLogic>();
            var embyImportLogic = _logicFactory.CreateLogic<IEmbyImportLogic>();

            var connectionInformationEmby1 = _connectionInformationFactory
                .CreateConnectionInformation<IConsoleEmbyInstance1ConnectionOptions>();
            var connectionInformationPlex1 = _connectionInformationFactory
                .CreateConnectionInformation<IConsolePlexInstance1ConnectionOptions>();

            var embyClient = _clientFactory.CreateClient<IEmbyClient>(connectionInformationEmby1);
            var plexClient = _clientFactory.CreateClient<IPlexClient>(connectionInformationPlex1);

            var userCredentialsService = _serviceFactory.CreateService<IUserCredentialsService>();

            var clients = new List<IClient> { embyClient, plexClient };

            bool retval;
            try
            {
                // FYI: As async access to the console is not possible, login data is collected ahead of the async login tasks.
                // FYI: Each client gets an instance of the credentials service, but it's up to the client to use it or not.
                clients.ForEach(x => x.SetLoginData(userCredentialsService));

                var didLoginAll = await LoginAllClientsAsync(clients);
                if (didLoginAll == false) return false;

                // TODO - handle RemoteLoggedOut?
                //_embyClient.RemoteLoggedOut += EmbyClient_RemoteLoggedOut;

                //await _embyService.DoItAsync(_embyClient);

                var didExportFromPlex = await ExportFromPlex(plexClient, plexExportLogic);
                if (didExportFromPlex == false)
                {
                    _logger.Warn("No items to process - exiting.");
                    return false;
                }

                var didImportToEmby = await ImportToEmby(embyClient, embyImportLogic, plexExportLogic.MovieMetadataItems);
                if (didImportToEmby == false)
                {
                    _logger.Warn("Some items could not be updated.");
                    return false;
                }

                _logger.Info("Update successful.");
            }
            finally
            {
                var didLogoutAll = await LogoutAllClientsAsync(clients);
                retval = didLogoutAll;
                _logger.Info("Logic done.");
            }

            return retval;
        }

        private async Task<bool> ExportFromPlex(IPlexClient plexClient, IPlexExportLogic plexExportLogic)
        {
            var spinWheelService = _serviceFactory.CreateService<ISpinWheelService>();

            using (var cts = new CancellationTokenSource())
            {
                await spinWheelService.StartSpinWheelAsync(cts.Token);
                var didExportFromPlex = await plexExportLogic.RunAsync(plexClient);
                spinWheelService.StopSpinWheel(cts);

                return didExportFromPlex;
            }
        }

        private async Task<bool> ImportToEmby(IEmbyClient embyClient, IEmbyImportLogic embyImportLogic, IList<IPlexMovieMetadata> plexMovieMetadata)
        {
            var spinWheelService = _serviceFactory.CreateService<ISpinWheelService>();

            using (var cts = new CancellationTokenSource())
            {
                await spinWheelService.StartSpinWheelAsync(cts.Token);
                var didImportToEmby = await embyImportLogic.RunAsync(embyClient, plexMovieMetadata);
                spinWheelService.StopSpinWheel(cts);

                return didImportToEmby;
            }
        }

        private async Task<bool> LoginAllClientsAsync(IEnumerable<IClient> clients)
        {
            var spinWheelService = _serviceFactory.CreateService<ISpinWheelService>();

            using (var cts = new CancellationTokenSource())
            {
                await spinWheelService.StartSpinWheelAsync(cts.Token);
                var loginAllClientsTask = await Task.WhenAll(clients.Select(LoginClientAsync));
                spinWheelService.StopSpinWheel(cts);

                return loginAllClientsTask.All(x => x);
            }
        }

        private async Task<bool> LoginClientAsync(IClient client)
        {
            try
            {
                //sawait Task.Delay(4000);
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
            var spinWheelService = _serviceFactory.CreateService<ISpinWheelService>();

            using (var cts = new CancellationTokenSource())
            {
                await spinWheelService.StartSpinWheelAsync(cts.Token);
                var logoutAllClientsTask = await Task.WhenAll(clients.Select(LogoutClientAsync));
                spinWheelService.StopSpinWheel(cts);

                return logoutAllClientsTask.All(x => x);
            }
        }

        private async Task<bool> LogoutClientAsync(IClient client)
        {
            try
            {
                //await Task.Delay(4000);
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
