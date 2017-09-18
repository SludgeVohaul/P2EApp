using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaBrowser.Model.Entities;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Emby.Library;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Services.Emby;
using P2E.Interfaces.Repositories.Emby;

namespace P2E.Services.Emby
{
    public class EmbyService : IEmbyService
    {
        private readonly IEmbyClient _client;
        private readonly IAppLogger _logger;
        private readonly IEmbyRepository _repository;

        public EmbyService(IAppLogger logger, IEmbyClient client, IEmbyRepository embyRepository)
        {
            _client = client;
            _logger = logger;
            _repository = embyRepository;
        }

        public async Task<ILibraryIdentifier> GetLibraryIdentifierAsync(string libraryName)
        {
            try
            {
                var libraryIdentifiers = await _repository.GetLibraryIdentifiersAsync(_client);
                return libraryIdentifiers.FirstOrDefault(x => x.Name == libraryName);
            }
            catch (Exception ex)
            {
                LogException(ex, "Failed to get library identifier:");
                return null;
            }
        }

        public async Task<IReadOnlyCollection<IMovieIdentifier>> GetMovieIdentifiersAsync(
            ILibraryIdentifier libraryIdentifier)
        {
            try
            {
                return await _repository.GetMovieIdentifiersAsync(_client, libraryIdentifier.Id);
            }
            catch (Exception ex)
            {
                LogException(ex, "Failed to get movie identifiers:");
                return null;
            }
        }

        public async Task<IReadOnlyCollection<ICollectionIdentifier>> GetCollectionIdentifiersAsync()
        {
            try
            {
                return (await _repository.GetCollectionIdentifiersAsync(_client)).ToArray();
            }
            catch (Exception ex)
            {
                LogException(ex, "Failed to get collection identifiers:");
                return null;
            }
        }

        public async Task<ICollectionIdentifier> CreateCollectionAsync(string collectionName)
        {
            try
            {
                _logger.Log(Severity.Info, $"Creating new collection '{collectionName}'.");
                var collectionIdentifier = await _repository.CreateCollectionAsync(_client, collectionName);
                _logger.Log(Severity.Debug,
                    $"New collection ID: {collectionIdentifier.Id} Pathname: {collectionIdentifier.PathBasename}");

                return collectionIdentifier;
            }
            catch (Exception ex)
            {
                LogException(ex, $"Failed to create collection '{collectionName}':");
                return null;
            }
        }

        public async Task<bool> TryAddMovieToCollectionAsync(IMovieIdentifier movieIdentifier,
                                                             ICollectionIdentifier collectionIdentifier)
        {
            try
            {
                _logger.Log(Severity.Info, $"Adding movie to collection '{collectionIdentifier.PathBasename}'.");
                await _repository.AddMovieToCollectionAsync(_client, movieIdentifier.Id, collectionIdentifier.Id);
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, $"Failed to add movie to collection '{collectionIdentifier.PathBasename}':");
                return false;
            }
        }

        /// <remarks>
        /// There is no way to tell the server to display the added image. One has to
        /// assume that the added image is the one with the highest index. See
        /// https://emby.media/community/index.php?/topic/50676-how-to-re-index-a-new-remoteimage-of-an-item/
        /// Therefore adding, finding and re-indexing is all done in this method as tearing it apart
        /// would increase chances that the last image is not the just added one anymore.
        /// </remarks>
        public async Task<bool> TryAddImageToMovieAsync(ImageType imageType,
                                                        Uri imageUrl,
                                                        IMovieIdentifier movieIdentifier)
        {
            if (imageUrl == null)
            {
                _logger.Log(Severity.Warn, $"No {imageType} image available.");
                return true;
            }

            try
            {
                _logger.Log(Severity.Info, $"Adding {imageType} image.");
                await _repository.AddImageToMovieAsync(_client, imageType, imageUrl, movieIdentifier.Id);
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, $"Failed to add {imageType} image:");
                return false;
            }
        }

        public async Task<bool> TryDeleteImageFromMovieAsync(ImageType imageType,
                                                             int imageIndex,
                                                             IMovieIdentifier movieIdentifier)
        {
            try
            {
                _logger.Log(Severity.Info, $"Deleting {imageType} image.");
                await _repository.DeleteImageFromMovieAsync(_client, imageType, imageIndex, movieIdentifier.Id);
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, $"Failed to delete {imageType} image at index {imageIndex}:");
                return false;
            }
        }

        public async Task<int?> GetMaxImageIndex(ImageType imageType, IMovieIdentifier movieIdentifier)
        {
            try
            {
                _logger.Log(Severity.Info, $"Querying the index of last {imageType} image.");
                var imageInfos = await _repository.GetImageInfosAsync(_client, movieIdentifier.Id);

                return imageInfos
                    .Where(x => x.ImageType == imageType)
                    .Select(x => x.ImageIndex ?? 0)
                    .Max();
            }
            catch (Exception ex)
            {
                LogException(ex, $"Failed to query the index of last {imageType} image:");
                return null;
            }
        }

        public async Task<bool> ReindexImageOfMovieAsync(ImageType imageType,
                                                         int currentIndex,
                                                         int newIndex,
                                                         IMovieIdentifier movieIdentifier)
        {
            try
            {
                _logger.Log(Severity.Info, $"Reindexing {imageType} image from index {currentIndex} to index {newIndex}.");
                await _repository.ReindexImageOfMovieAsync(_client, imageType, currentIndex, newIndex, movieIdentifier.Id);
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, $"Failed to reindex {imageType} image:");
                return false;
            }
        }

        private void LogException(Exception ex, string message)
        {
            _logger.Log(Severity.Error, message);
            _logger.Log(Severity.ErrorException, ex.Message);
            while (ex.InnerException != null)
            {
                _logger.Log(Severity.ErrorException, ex.InnerException.Message);
                ex = ex.InnerException;
            }
        }
    }
}
