using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaBrowser.Model.Entities;
using P2E.Interfaces.AppLogic.Emby;
using P2E.Interfaces.DataObjects.Emby.Library;
using P2E.Interfaces.DataObjects.Plex.Library;
using P2E.Interfaces.Factories;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Services.Emby;

namespace P2E.AppLogic.Emby
{
    public class EmbyImportMovieImagesLogic : IEmbyImportMovieImagesLogic
    {
        private readonly IAppLogger _logger;
        private readonly IServiceFactory _serviceFactory;

        public EmbyImportMovieImagesLogic(IAppLogger logger,
                                          IServiceFactory serviceFactory)
        {
            _logger = logger;
            _serviceFactory = serviceFactory;
        }

        public async Task<bool> RunAsync(IPlexMovieMetadata plexMovieMetadata, IMovieIdentifier embyMovieIdentifier)
        {
            var importableImages = new[]
            {
                new ImportableImage {SourceUri = plexMovieMetadata.ThumbUri, ImageType = ImageType.Primary},
                new ImportableImage {SourceUri = plexMovieMetadata.ArtUri, ImageType = ImageType.Backdrop},

            };
            return await AddImagesToMovieWithDelete(importableImages, embyMovieIdentifier);
        }

        private async Task<bool> AddImagesToMovieWithDelete(IReadOnlyCollection<ImportableImage> importableImages, IMovieIdentifier embyMovieIdentifier)
        {
            var service = _serviceFactory.CreateService<IEmbyImageService>();

            var retval = true;

            foreach (var importableImage in importableImages)
            {
                // Get the index of the last image.
                var maxImageIndex = await service.GetMaxImageIndex(importableImage.ImageType, embyMovieIdentifier);
                if (maxImageIndex == null)
                {
                    _logger.Log(Severity.Warn, $"The added {importableImage.ImageType} image might not be diplayed for '{embyMovieIdentifier.Filename}'.");
                    retval = false;
                    continue;
                }

                // Delete all existing images of a type from the movie starting from last image.
                var itemCount = maxImageIndex.Value + 1;
                foreach (var lastIndex in Enumerable.Range(0, itemCount).OrderByDescending(x => x))
                {
                    if (await service.TryDeleteImageFromMovieAsync(importableImage.ImageType, lastIndex, embyMovieIdentifier)) continue;

                    _logger.Log(Severity.Warn, $"The added {importableImage.ImageType} image might not be diplayed for '{embyMovieIdentifier.Filename}'.");
                    retval = false;
                }

                // Add image.
                if (await service.TryAddImageToMovieAsync(importableImage.ImageType, importableImage.SourceUri, embyMovieIdentifier) == false)
                {
                    _logger.Log(Severity.Warn, $"{importableImage.ImageType} image from {importableImage.SourceUri} was not imported.");
                    retval = false;
                }
            }

            return retval;
        }

        /// <remarks>
        /// Unfortunatelly reindexing does not work reliably, see
        /// https://emby.media/community/index.php?/topic/50794-apiclient-server-3230-cannot-reindex-the-backdrop-image-of-a-movie/
        /// </remarks>
        private async Task<bool> AddImagesToMovieWithReindex(IReadOnlyCollection<ImportableImage> importableImages, IMovieIdentifier embyMovieIdentifier)
        {
            var service = _serviceFactory.CreateService<IEmbyImageService>();

            var retval = true;

            foreach (var importableImage in importableImages)
            {
                // Add image.
                if (await service.TryAddImageToMovieAsync(importableImage.ImageType, importableImage.SourceUri, embyMovieIdentifier) == false)
                {
                    _logger.Log(Severity.Warn, $"{importableImage.ImageType} image from {importableImage.SourceUri} was not imported.");
                    retval = false;
                    continue;
                }

                // Get the index of the last image (of the just added type).
                var maxImageIndex = await service.GetMaxImageIndex(importableImage.ImageType, embyMovieIdentifier);
                if (maxImageIndex == null)
                {
                    _logger.Log(Severity.Warn, $"The added {importableImage.ImageType} image might not be diplayed.");
                    retval = false;
                    continue;
                }

                // Do not reindex if there is only one (or none) image.
                if (maxImageIndex.Value < 1) continue;

                if (await service.ReindexImageOfMovieAsync(importableImage.ImageType, maxImageIndex.Value, 0, embyMovieIdentifier) == false)
                {
                    _logger.Log(Severity.Warn, $"The added {importableImage.ImageType} will not be diplayed.");
                    retval = false;
                }
            }

            return retval;
        }

        private class ImportableImage
        {
            public Uri SourceUri { get; set; }
            public ImageType ImageType { get; set; }
        }
    }
}