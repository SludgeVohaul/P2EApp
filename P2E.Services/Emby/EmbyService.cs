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

        public async Task<IReadOnlyCollection<IFilenameIdentifier>> GetFilenameIdentifiersAsync(
            ILibraryIdentifier libraryIdentifier)
        {
            try
            {
                return await _repository.GetFilenameIdentifiersAsync(_client, libraryIdentifier);
            }
            catch (Exception ex)
            {
                LogException(ex, "Failed to get filename identifiers:");
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
                _logger.Log(Severity.Debug, $"New collection ID: {collectionIdentifier.Id} Pathname: {collectionIdentifier.PathBasename}");

                return collectionIdentifier;
            }
            catch (Exception ex)
            {
                LogException(ex, $"Failed to create collection '{collectionName}':");
                return null;
            }
        }

        public async Task<bool> TryAddMovieToCollectionAsync(IFilenameIdentifier filenameIdentifier,
                                                             ICollectionIdentifier collectionIdentifier)
        {
            try
            {
                _logger.Log(Severity.Info, $"Adding movie to collection '{collectionIdentifier.PathBasename}'.");
                await _repository.AddMovieToCollectionAsync(_client, filenameIdentifier.Id, collectionIdentifier.Id);
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, $"Failed to add movie to collection '{collectionIdentifier.PathBasename}':");
                return false;
            }
        }

        public async Task<bool> TryAddImageToMovieAsync(IFilenameIdentifier filenameIdentifier,
                                                        ImageType imageType, Uri imageUrl)
        {
            try
            {
                if (imageUrl == null)
                {
                    _logger.Log(Severity.Warn, $"No {imageType} image available.");
                    return true;
                }

                _logger.Log(Severity.Info, $"Adding {imageType} image.");
                await _repository.AddImageToMovieAsync(_client, filenameIdentifier.Id, imageType, imageUrl);
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, $"Failed to add {imageType} image:");
                return false;
            }
        }

        public async Task<bool> TryDeleteImagesFromMovieAsync(IFilenameIdentifier filenameIdentifier, ImageType imageType)
        {
            try
            {
                _logger.Log(Severity.Info, $"Deleting all {imageType} images.");
                await _repository.DeleteImagesFromMovieAsync(_client, filenameIdentifier.Id, imageType);

                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, $"Failed to delete {imageType} images:");
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


        //public async Task<IMovieUpdateResult> UpdateItemAsync(IPlexMovieMetadata plexMovieMetadata, IFilenameIdentifier filenameIdentifier)
        //{



        //    //await Task.Delay(1000);
        //    _logger.Info($"Processing {plexMovieMetadata.Title}");
        //    return new MovieUpdateResult
        //    {
        //        Filename = filenameIdentifier.Filename,
        //        Title = plexMovieMetadata.Title,
        //        IsUpdated = false
        //    };
        //}

        //public async Task<bool> UpdateItemAsync(IPlexMovieMetadata plexMovieMetadata, string embyLibraryName)
        //{
        //    //var movieIds = await _repository.GetMovieIdsAsync(_client, embyLibraryName);




        //    await Task.Delay(1000);
        //    _logger.Info($"Processing {plexMovieMetadata.Title}");
        //    return true;

        //}
    }
}
