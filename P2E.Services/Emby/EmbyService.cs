using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Emby.Library;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Services.Emby;
using P2E.Interfaces.Repositories.Emby;

namespace P2E.Services.Emby
{
    public class EmbyService : EmbyBaseService, IEmbyService
    {
        private static readonly SemaphoreSlim SemSlim = new SemaphoreSlim(1, 1);

        public EmbyService(IAppLogger logger, IEmbyClient client, IEmbyRepository embyRepository)
            : base(logger, client, embyRepository)
        {
        }

        public async Task<ILibraryIdentifier> GetLibraryIdentifierAsync(string libraryName)
        {
            await SemSlim.WaitAsync();
            try
            {
                var libraryIdentifiers = await Repository.GetLibraryIdentifiersAsync(Client);
                return libraryIdentifiers.FirstOrDefault(x => x.Name == libraryName);
            }
            catch (Exception ex)
            {
                LogException(ex, "Failed to get library identifier:");
                return null;
            }
            finally
            {
                SemSlim.Release();
            }
        }

        public async Task<IReadOnlyCollection<IMovieIdentifier>> GetMovieIdentifiersAsync(
            ILibraryIdentifier libraryIdentifier)
        {
            await SemSlim.WaitAsync();
            try
            {
                return await Repository.GetMovieIdentifiersAsync(Client, libraryIdentifier.Id);
            }
            catch (Exception ex)
            {
                LogException(ex, "Failed to get movie identifiers:");
                return null;
            }
            finally
            {
                SemSlim.Release();
            }
        }

        public async Task<IReadOnlyCollection<ICollectionIdentifier>> GetCollectionIdentifiersAsync()
        {
            await SemSlim.WaitAsync();
            try
            {
                return (await Repository.GetCollectionIdentifiersAsync(Client)).ToArray();
            }
            catch (Exception ex)
            {
                LogException(ex, "Failed to get collection identifiers:");
                return null;
            }
            finally
            {
                SemSlim.Release();
            }
        }

        public async Task<ICollectionIdentifier> CreateCollectionAsync(string collectionName)
        {
            await SemSlim.WaitAsync();
            try
            {
                Logger.Log(Severity.Info, $"Creating new collection '{collectionName}'.");
                var collectionIdentifier = await Repository.CreateCollectionAsync(Client, collectionName);
                Logger.Log(Severity.Debug,
                    $"New collection ID: {collectionIdentifier.Id} Pathname: {collectionIdentifier.PathBasename}");

                return collectionIdentifier;
            }
            catch (Exception ex)
            {
                LogException(ex, $"Failed to create collection '{collectionName}':");
                return null;
            }
            finally
            {
                SemSlim.Release();
            }
        }

        public async Task<bool> TryAddMovieToCollectionAsync(IMovieIdentifier movieIdentifier,
                                                             ICollectionIdentifier collectionIdentifier)
        {
            await SemSlim.WaitAsync();
            try
            {
                Logger.Log(Severity.Info, $"Adding movie to collection '{collectionIdentifier.PathBasename}'.");
                await Repository.AddMovieToCollectionAsync(Client, movieIdentifier.Id, collectionIdentifier.Id);
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, $"Failed to add movie to collection '{collectionIdentifier.PathBasename}':");
                return false;
            }
            finally
            {
                SemSlim.Release();
            }
        }
    }
}
