using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using P2E.Interfaces.AppLogic.Emby;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Emby.Library;
using P2E.Interfaces.DataObjects.Plex.Library;
using P2E.Interfaces.Factories;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Services.Emby;

namespace P2E.AppLogic.Emby
{
    public class EmbyImportMovieLogic : IEmbyImportMovieLogic
    {
        public event EventHandler ItemProcessed;

        // TODO - Is it a good idea to execute all updates asynchronously?
        // Do it sequentially first...
        private static readonly SemaphoreSlim SemSlim = new SemaphoreSlim(1, 1);

        private readonly IAppLogger _logger;
        private readonly IServiceFactory _serviceFactory;
        private readonly IEmbyClient _client;

        public EmbyImportMovieLogic(IAppLogger logger,
            IServiceFactory serviceFactory,
            IEmbyClient client)
        {
            _logger = logger;
            _serviceFactory = serviceFactory;
            _client = client;
        }

        public async Task<bool> RunAsync(IPlexMovieMetadata plexMovieMetaDataItem, IFilenameIdentifier embyFilenameIdentifier)
        {
            await SemSlim.WaitAsync();
            try
            {
                var embyService = _serviceFactory.CreateService<IEmbyService, IEmbyClient>(_client);

                _logger.Log(Severity.Info, $"Processing '{plexMovieMetaDataItem.Title}'");

                // Get (and create if necessary) all collections the movie belongs to.
                var collectionIdentifiers = await GetCollectionsForMovieAsync(embyService, plexMovieMetaDataItem.Collections);
                if (collectionIdentifiers == null)
                {
                    var msg = $"Failed to update Emby collections for '{plexMovieMetaDataItem.Title}'. Import aborted.";
                    _logger.Log(Severity.Error, msg);
                    return false;
                }

                // Add the movie to all collections.
                if ((await Task.WhenAll(collectionIdentifiers.Select(x => embyService.TryAddMovieToCollectionAsync(embyFilenameIdentifier, x)))).Any(x => x == false))
                {
                    var msg = $"Failed to add '{plexMovieMetaDataItem.Title}' to provided collections. Import aborted.";
                    _logger.Log(Severity.Error, msg);
                    return false;
                }






                //var failedMovieTitles = updateResults.Where(x => x.IsUpdated == false).Select(x => x.Title).ToArray();
                //await LogItemsAsync(Severity.Warn, "Update failed for the following titles", failedMovieTitles);

                return false;
            }
            finally
            {
                OnItemProcessed();
                SemSlim.Release();
            }
        }

        private async Task<IReadOnlyCollection<ICollectionIdentifier>> GetCollectionsForMovieAsync(IEmbyService service, IReadOnlyCollection<string> plexCollections)
        {
            // Plex movie is not included in any collections.
            if (plexCollections.Count == 0) return new ICollectionIdentifier[] {};

            // Already present Emby collections needed by the movie.
            var existingMovieCollections = (await service.GetCollectionIdentifiersAsync())
                .Where(x => plexCollections.Contains(x.PathBasename))
                .ToList();

            // All collections needed by the movie are already present.
            if (plexCollections.Count == existingMovieCollections.Count) return existingMovieCollections;

            var missingMovieCollections = plexCollections
                .Except(existingMovieCollections.Select(x => x.PathBasename))
                .ToArray();

            _logger.Log(Severity.Info, "Creating missing collections:");
            var createdCollections = await Task.WhenAll(missingMovieCollections.Select(service.CreateCollectionAsync));
            if (createdCollections.Any(x => x == null)) return null;

            existingMovieCollections.AddRange(createdCollections);
            return existingMovieCollections;
        }

        private void OnItemProcessed()
        {
            ItemProcessed?.Invoke(this, new EventArgs());
        }
    }
}