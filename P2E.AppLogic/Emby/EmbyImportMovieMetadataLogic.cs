using System.Threading.Tasks;
using P2E.Interfaces.AppLogic.Emby;
using P2E.Interfaces.DataObjects.Emby.Library;
using P2E.Interfaces.DataObjects.Plex.Library;
using P2E.Interfaces.Factories;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Services.Emby;

namespace P2E.AppLogic.Emby
{
    public class EmbyImportMovieMetadataLogic : IEmbyImportMovieMetadataLogic
    {
        private readonly IAppLogger _logger;
        private readonly IServiceFactory _serviceFactory;

        public EmbyImportMovieMetadataLogic(IAppLogger logger,
            IServiceFactory serviceFactory)
        {
            _logger = logger;
            _serviceFactory = serviceFactory;
        }

        public async Task<bool> RunAsync(IPlexMovieMetadata plexMovieMetadata, IMovieIdentifier movieIdentifier)
        {
            var retval = true;

            var metadataService = _serviceFactory.CreateService<IEmbyMetadataService>();


            // Update Watched/Unwatched.
            if (await metadataService.UpdateWatchedStatusAsync(plexMovieMetadata, movieIdentifier) == false)
            {
                _logger.Log(Severity.Warn, $"Failed to update Watched/Unwatched for '{movieIdentifier.Filename}'.");
                retval = false;
            }

            // Update other metadata.
            if (await metadataService.UpdateMetadataAsync(plexMovieMetadata, movieIdentifier) == false)
            {
                _logger.Log(Severity.Warn, $"Failed to update metadata for '{movieIdentifier.Filename}'.");
                retval = false;
            }

            return retval;
        }
    }
}