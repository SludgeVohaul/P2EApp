using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using P2E.Interfaces.AppLogic.Emby;
using P2E.Interfaces.CommandLine;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Emby.Library;
using P2E.Interfaces.DataObjects.Plex.Library;
using P2E.Interfaces.Factories;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Services.Emby;
using P2E.Interfaces.Services.SpinWheel;

namespace P2E.AppLogic.Emby
{
    public class EmbyImportLogic : IEmbyImportLogic
    {
        private readonly IAppLogger _logger;
        private readonly IEmbyClient _client;
        private readonly ILogicFactory _logicFactory;
        private readonly IServiceFactory _serviceFactory;
        private readonly IConsoleLibraryOptions _consoleLibraryOptions;

        public EmbyImportLogic(IAppLogger logger,
            IEmbyClient client,
            ILogicFactory logicFactory,
            IServiceFactory serviceFactory,
            IConsoleLibraryOptions consoleLibraryOptions)
        {
            _logger = logger;
            _client = client;
            _logicFactory = logicFactory;
            _serviceFactory = serviceFactory;
            _consoleLibraryOptions = consoleLibraryOptions;
        }

        public async Task<bool> RunAsync(IReadOnlyCollection<IPlexMovieMetadata> plexMovieMetadataItems)
        {
            var spinWheelService = _serviceFactory.CreateService<ISpinWheelService>();
            var embyService = _serviceFactory.CreateService<IEmbyService, IEmbyClient>(_client);

            var libraryIdentifier = await GetLibraryIdentifierAsync(embyService, spinWheelService, _consoleLibraryOptions.EmbyLibraryName);
            if (libraryIdentifier == null)
            {
                _logger.Error($"Cannot find Emby library '{_consoleLibraryOptions.EmbyLibraryName}'.");
                return false;
            }

            var movieIdentifiers = await GetMovieIdentifiersAsync(embyService, spinWheelService, libraryIdentifier);
            if (movieIdentifiers == null || movieIdentifiers.Any() == false)
            {
                _logger.Error($"No movie files found in Emby library '{libraryIdentifier.Name}'.");
                return false;
            }

            var embyFiles = movieIdentifiers.Select(x => x.Filename).ToArray();
            var plexFiles = plexMovieMetadataItems.SelectMany(x => x.Filenames).ToArray();
            var filesInBothServers = embyFiles.Intersect(plexFiles).ToArray();
            var filesNotInBothServers = embyFiles.Except(plexFiles).Union(plexFiles.Except(embyFiles)).ToArray();

            await LogItemsAsync(Severity.Warn, "Following files do not exist in both servers:", filesNotInBothServers);

            // TODO - plexMovieMetadataItems could be null - handle this.
            var didUpdateMovies = await UpdateMoviesAsync(spinWheelService,
                                                        plexMovieMetadataItems.Where(x => x.Filenames.Any(y => filesInBothServers.Contains(y))).ToArray(),
                                                        movieIdentifiers.Where(x => filesInBothServers.Contains(x.Filename)).ToArray());

            // TODO - output some summary at the end?
            //var failedMovieTitles = updateResults.Where(x => x.IsUpdated == false).Select(x => x.Title).ToArray();
            //await LogItemsAsync(Severity.Warn, "Update failed for the following titles", failedMovieTitles);

            return didUpdateMovies.All(x => x);
        }

        private static async Task<ILibraryIdentifier> GetLibraryIdentifierAsync(IEmbyService embyService,
                                                                                ISpinWheelService spinWheelService,
                                                                                string libraryName)
        {
            var cts = new CancellationTokenSource();
            try
            {
                await spinWheelService.StartSpinWheelAsync(cts.Token);
                return await embyService.GetLibraryIdentifierAsync(libraryName);
            }
            finally
            {
                spinWheelService.StopSpinWheel(cts);
                cts.Dispose();
            }
        }

        private static async Task<IReadOnlyCollection<IMovieIdentifier>> GetMovieIdentifiersAsync(IEmbyService embyService,
                                                                                                        ISpinWheelService spinWheelService,
                                                                                                        ILibraryIdentifier libraryIdentifier)
        {
            var cts = new CancellationTokenSource();
            try
            {
                await spinWheelService.StartSpinWheelAsync(cts.Token);
                return await embyService.GetMovieIdentifiersAsync(libraryIdentifier);
            }
            finally
            {
                spinWheelService.StopSpinWheel(cts);
                cts.Dispose();
            }
        }

        private async Task<IReadOnlyCollection<bool>> UpdateMoviesAsync(ISpinWheelService spinWheelService,
                                                                        IReadOnlyCollection<IPlexMovieMetadata> plexMovieMetadataItems,
                                                                        IReadOnlyCollection<IMovieIdentifier> embyMovieIdentifiers)
        {
            var cts = new CancellationTokenSource();
            IEmbyImportMovieLogic embyImportMovieLogic = null;
            try
            {
                embyImportMovieLogic = _logicFactory.CreateEmbyImportMovieLogic(_client);
                embyImportMovieLogic.ItemProcessed += spinWheelService.OnItemProcessed;

                await spinWheelService.StartSpinWheelAsync(cts.Token);

                var updateTasks = plexMovieMetadataItems
                    .Select(plexMovieMetaDataItem =>
                    {
                        var embyMovieIdentifier = embyMovieIdentifiers.First(x => plexMovieMetaDataItem.Filenames.Contains(x.Filename));
                        return embyImportMovieLogic.RunAsync(plexMovieMetaDataItem, embyMovieIdentifier);
                    })
                    .ToArray();

                return await Task.WhenAll(updateTasks);
            }
            finally
            {
                if (embyImportMovieLogic != null) embyImportMovieLogic.ItemProcessed -= spinWheelService.OnItemProcessed;
                spinWheelService.StopSpinWheel(cts);
                cts.Dispose();
            }
        }

        private async Task LogItemsAsync(Severity severity, string headerMessage, IReadOnlyCollection<string> items)
        {

            await Task.Run(() =>
            {
                if (items.Any() == false) return;

                _logger.Log(severity, headerMessage);
                items
                .ToList()
                .ForEach(x => _logger.Log(severity, x));
            });
        }
    }
}