using System;
using System.Threading;
using System.Threading.Tasks;
using P2E.Interfaces.AppLogic.Emby;
using P2E.Interfaces.DataObjects.Emby.Library;
using P2E.Interfaces.DataObjects.Plex.Library;
using P2E.Interfaces.Factories;
using P2E.Interfaces.Logging;

namespace P2E.AppLogic.Emby
{
    public class EmbyImportMovieLogic : IEmbyImportMovieLogic
    {
        public event EventHandler ItemProcessed;

        // TODO - Is it a good idea to execute all updates asynchronously?
        // Do it sequentially first...
        private static readonly SemaphoreSlim SemSlim = new SemaphoreSlim(1, 1);

        private readonly IAppLogger _logger;
        private readonly ILogicFactory _logicFactory;

        public EmbyImportMovieLogic(IAppLogger logger, ILogicFactory logicFactory)
        {
            _logger = logger;
            _logicFactory = logicFactory;
        }

        public async Task<bool> RunAsync(IPlexMovieMetadata plexMovieMetadata, IMovieIdentifier embyMovieIdentifier)
        {
            var retval = true;

            await SemSlim.WaitAsync();
            try
            {
                _logger.Log(Severity.Info, $"Processing '{plexMovieMetadata.Title}'");

                // Add movie to all collections (create if necessary).
                var embyImportMovieCollectionsLogic = _logicFactory.CreateLogic<IEmbyImportMovieCollectionsLogic>();
                if (await embyImportMovieCollectionsLogic.RunAsync(plexMovieMetadata.Collections, embyMovieIdentifier) == false)
                {
                    var msg = $"Failed to update Emby collections for '{plexMovieMetadata.Title}'.";
                    _logger.Log(Severity.Warn, msg);
                    retval = false;
                }

                // Add images to movie.
                var embyImportMovieImagesLogic = _logicFactory.CreateLogic<IEmbyImportMovieImagesLogic>();
                if (await embyImportMovieImagesLogic.RunAsync(plexMovieMetadata, embyMovieIdentifier) == false)
                {
                    _logger.Log(Severity.Warn, $"One or more images could not be properly added to '{plexMovieMetadata.Title}'.");
                    retval = false;
                }

                // Update movie metadata.
                var embyImportMovieMetadataLogic = _logicFactory.CreateLogic<IEmbyImportMovieMetadataLogic>();
                if (await embyImportMovieMetadataLogic.RunAsync(plexMovieMetadata, embyMovieIdentifier) == false)
                {
                    _logger.Log(Severity.Warn, $"Metadata could not be updated on '{plexMovieMetadata.Title}'.");
                    retval = false;
                }

                return retval;
            }
            finally
            {
                OnItemProcessed();
                SemSlim.Release();
            }
        }

        private void OnItemProcessed()
        {
            ItemProcessed?.Invoke(this, new EventArgs());
        }
    }
}