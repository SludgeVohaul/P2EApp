using System;
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
            var embyService = _serviceFactory.CreateService<IEmbyService, IEmbyClient>(_client);

            //var collectionIdentifiers = await embyService.GetCollectionIdentifiersAsync(_client, plexMovieMetaDataItem.Collections);

            await Task.Delay(1000);

            //var failedMovieTitles = updateResults.Where(x => x.IsUpdated == false).Select(x => x.Title).ToArray();
            //await LogItemsAsync(Severity.Warn, "Update failed for the following titles", failedMovieTitles);
            OnItemProcessed();
            return false;
        }


        private void OnItemProcessed()
        {
            ItemProcessed?.Invoke(this, new EventArgs());
        }
    }
}