using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var libraryIdentifiers = await _repository.GetLibraryIdentifiersAsync(_client);
            return libraryIdentifiers.FirstOrDefault(x => x.Name == libraryName);
        }

        public async Task<IReadOnlyCollection<IFilenameIdentifier>> GetFilenameIdentifiersAsync(
            ILibraryIdentifier libraryIdentifier)
        {
            return await _repository.GetFilenameIdentifiersAsync(_client, libraryIdentifier);
        }

        public async Task<IReadOnlyCollection<ICollectionIdentifier>> GetCollectionIdentifiersAsync()
        {
            return (await _repository.GetCollectionIdentifiersAsync(_client)).ToArray();
        }

        public async Task<ICollectionIdentifier> CreateCollectionAsync(string collectionName)
        {
            try
            {
                var collectionIdentifier = await _repository.CreateCollectionAsync(_client, collectionName);
                _logger.Log(Severity.Info, $"New collection '{collectionIdentifier.PathBasename}'");
                _logger.Log(Severity.Debug, $"New collection ID: {collectionIdentifier.Id}");
                return collectionIdentifier;
            }
            catch (Exception ex)
            {
                _logger.Log(Severity.Error, $"Failed to create collection '{collectionName}':");
                _logger.Log(Severity.ErrorException, ex.Message);
                while (ex.InnerException != null)
                {
                    _logger.Log(Severity.ErrorException, ex.InnerException.Message);
                    ex = ex.InnerException;
                }
                return null;
            }
        }

        public async Task<bool> TryAddMovieToCollectionAsync(IFilenameIdentifier filenameIdentifier,
                                                             ICollectionIdentifier collectionIdentifier)
        {
            try
            {
                await _repository.AddMovieToCollectionAsync(_client, filenameIdentifier.Id, collectionIdentifier.Id);
                _logger.Log(Severity.Info, $"Adding movie to collection '{collectionIdentifier.PathBasename}'");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Log(Severity.Error, $"Failed to add movie to collection '{collectionIdentifier.PathBasename}':");
                _logger.Log(Severity.ErrorException, ex.Message);
                while (ex.InnerException != null)
                {
                    _logger.Log(Severity.ErrorException, ex.InnerException.Message);
                    ex = ex.InnerException;
                }
                return false;
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
