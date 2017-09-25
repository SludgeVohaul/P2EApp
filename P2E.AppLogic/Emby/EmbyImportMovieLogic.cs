using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Entities;
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

        public async Task<bool> RunAsync(IPlexMovieMetadata plexMovieMetadata, IMovieIdentifier embyMovieIdentifier)
        {
            var retval = true;

            await SemSlim.WaitAsync();
            try
            {
                var embyService = _serviceFactory.CreateService<IEmbyService, IEmbyClient>(_client);

                _logger.Log(Severity.Info, $"Processing '{plexMovieMetadata.Title}'");

                // Get (and create if necessary) all collections the movie belongs to.
                var collectionIdentifiers = await GetCollectionsForMovieAsync(embyService, plexMovieMetadata.Collections);
                if (collectionIdentifiers == null)
                {
                    var msg = $"Failed to update Emby collections for '{plexMovieMetadata.Title}'. Movie will not be added into any collection.";
                    _logger.Log(Severity.Warn, msg);
                    retval = false;
                }

                // Add the movie to all collections.
                if (collectionIdentifiers != null)
                {
                    if (await AddMovieToCollections(embyService, collectionIdentifiers, embyMovieIdentifier) == false)
                    {
                        _logger.Log(Severity.Warn, $"Failed to add '{plexMovieMetadata.Title}' to one or more collections.");
                        retval = false;
                    }
                }

                // Add images to movie.
                if (await AddImagesToMovieWithDelete(embyService, plexMovieMetadata, embyMovieIdentifier) == false)
                {
                    _logger.Log(Severity.Warn, $"One or more images could not be properly added to '{plexMovieMetadata.Title}'.");
                    retval = false;
                }

                // Add images to movie.
                //if (await AddImagesToMovieWithReindex(embyService, plexMovieMetadata, embyMovieIdentifier) == false)
                //{
                //    _logger.Log(Severity.Warn, $"One or more images could not be properly added to '{plexMovieMetadata.Title}'.");
                //    retval = false;
                //}

                return retval;
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
            var embyCollections = await service.GetCollectionIdentifiersAsync();
            if (embyCollections == null) return null;
            var existingMovieCollections = new List<ICollectionIdentifier>(embyCollections.Where(x => plexCollections.Contains(x.PathBasename)));

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

        private async Task<bool> AddImagesToMovieWithDelete(IEmbyService service, IPlexMovieMetadata plexMovieMetadata, IMovieIdentifier embyMovieIdentifier)
        {
            // TODO - change this C# 7 tuple features in VS2017
            var plexEmbyImagePairs = new[]
            {
                new Tuple<Uri, ImageType>(plexMovieMetadata.ThumbUri, ImageType.Primary),
                new Tuple<Uri, ImageType>(plexMovieMetadata.ArtUri, ImageType.Backdrop)
            };

            var retval = true;

            foreach (var plexEmbyImagePair in plexEmbyImagePairs)
            {
                // Get the index of the last image.
                var maxImageIndex = await service.GetMaxImageIndex(plexEmbyImagePair.Item2, embyMovieIdentifier);
                if (maxImageIndex == null)
                {
                    _logger.Log(Severity.Warn, $"The added {plexEmbyImagePair.Item2} image might not be diplayed for '{plexMovieMetadata.Title}'.");
                    retval = false;
                    continue;
                }

                // Delete all existing images of a type from the movie from last to first.
                var itemCount = maxImageIndex.Value + 1;
                foreach (var lastIndex in Enumerable.Range(0, itemCount).OrderByDescending(x => x))
                {
                    if (await service.TryDeleteImageFromMovieAsync(plexEmbyImagePair.Item2, lastIndex, embyMovieIdentifier)) continue;

                    _logger.Log(Severity.Warn, $"The added {plexEmbyImagePair.Item2} image might not be diplayed for '{plexMovieMetadata.Title}'.");
                    retval = false;
                }

                // Add image.
                if (await service.TryAddImageToMovieAsync(plexEmbyImagePair.Item2, plexEmbyImagePair.Item1, embyMovieIdentifier) == false)
                {
                    _logger.Log(Severity.Warn, $"{plexEmbyImagePair.Item2} image at {plexEmbyImagePair.Item1} was not imported.");
                    retval = false;
                }
            }

            return retval;
        }

        //private async Task<bool> AddImagesToMovieWithReindex(IEmbyService service, IPlexMovieMetadata plexMovieMetadata, IMovieIdentifier embyMovieIdentifier)
        //{
        //    // TODO - change this C# 7 tuple features in VS2017
        //    var plexEmbyImagePairs = new []
        //    {
        //        new Tuple<Uri, ImageType>(plexMovieMetadata.ThumbUri, ImageType.Primary),
        //        new Tuple<Uri, ImageType>(plexMovieMetadata.ArtUri, ImageType.Backdrop)
        //    };

        //    var retval = true;

        //    foreach (var plexEmbyImagePair in plexEmbyImagePairs)
        //    {
        //        // Add image.
        //        if (await service.TryAddImageToMovieAsync(plexEmbyImagePair.Item2, plexEmbyImagePair.Item1, embyMovieIdentifier) == false)
        //        {
        //            _logger.Log(Severity.Warn, $"{plexEmbyImagePair.Item2} image at {plexEmbyImagePair.Item1} was not imported.");
        //            retval = false;
        //            continue;
        //        }

        //        // Get the index of the last image (of the just added type).
        //        var maxImageIndex = await service.GetMaxImageIndex(plexEmbyImagePair.Item2, embyMovieIdentifier);
        //        if (maxImageIndex == null)
        //        {
        //            _logger.Log(Severity.Warn, $"The added {plexEmbyImagePair.Item2} image might not be diplayed.");
        //            retval = false;
        //            continue;
        //        }

        //        // Do not reindex if there is only one (or none) image.
        //        if (maxImageIndex.Value < 1) continue;

        //        if (await service.ReindexImageOfMovieAsync(plexEmbyImagePair.Item2, maxImageIndex.Value, 0, embyMovieIdentifier) == false)
        //        {
        //            _logger.Log(Severity.Warn, $"The added {plexEmbyImagePair.Item2} will not be diplayed.");
        //            retval = false;
        //        }
        //    }

        //    return retval;
        //}

        private async Task<bool> AddMovieToCollections(IEmbyService service,
                                                       IReadOnlyCollection<ICollectionIdentifier> collectionIdentifiers,
                                                       IMovieIdentifier embyMovieIdentifier)
        {
            var addMovieToCollectionsResults = await Task.WhenAll(collectionIdentifiers.Select(x => service.TryAddMovieToCollectionAsync(embyMovieIdentifier, x)));
            return addMovieToCollectionsResults.All(x => x);
        }

        private void OnItemProcessed()
        {
            ItemProcessed?.Invoke(this, new EventArgs());
        }
    }
}