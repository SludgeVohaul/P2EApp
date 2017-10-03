using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Emby.Library;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Repositories.Emby;
using P2E.Interfaces.Services.Emby;
using OperatingSystem = MediaBrowser.Model.System.OperatingSystem;

namespace P2E.Services.Emby
{
    public class EmbyCollectionService : EmbyBaseService, IEmbyCollectionService
    {
        private static readonly SemaphoreSlim SemSlim = new SemaphoreSlim(1, 1);

        private OperatingSystem? _serverOperatingSystem;

        public EmbyCollectionService(IAppLogger logger, IEmbyClient client, IEmbyRepository embyRepository)
            : base(logger, client, embyRepository)
        {
        }

        public async Task<IReadOnlyCollection<ICollectionIdentifier>> GetCollectionIdentifiersAsync()
        {
            await SemSlim.WaitAsync();
            try
            {
                Logger.Log(Severity.Info, "Querying existing collections.");
                return (await Repository.GetCollectionIdentifiersAsync(Client)).ToArray();
            }
            catch (Exception ex)
            {
                LogException(ex, "Failed to query existing collections");
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
                var msg = $"New collection ID: {collectionIdentifier.Id} Filename: {collectionIdentifier.Filename}";
                Logger.Log(Severity.Debug, msg);

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
                Logger.Log(Severity.Info, $"Adding movie to collection '{collectionIdentifier.Name}'.");
                await Repository.AddMovieToCollectionAsync(Client, movieIdentifier.Id, collectionIdentifier.Id);
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, $"Failed to add movie to collection '{collectionIdentifier.Name}':");
                return false;
            }
            finally
            {
                SemSlim.Release();
            }
        }

        public async Task<OperatingSystem?> GetServerOperatingSystemAsync()
        {
            await SemSlim.WaitAsync();
            try
            {
                // TODO - This saves expensive HTTP requests once the OS has been determined,
                // TODO - but remains null in case of errors -> error message for each movie.
                if (_serverOperatingSystem != null) return _serverOperatingSystem.Value;

                Logger.Log(Severity.Info, "Querying the server's operating system.");
                var publicSystemInfo = await Client.GetPublicSystemInfoAsync();

                OperatingSystem osEnum;
                if (Enum.TryParse(publicSystemInfo.OperatingSystem, out osEnum))
                {
                    _serverOperatingSystem = osEnum;
                }
                return _serverOperatingSystem;
            }
            catch (Exception ex)
            {
                LogException(ex, "Failed to query the server's operating system:");
                return null;
            }

            finally
            {
                SemSlim.Release();
            }
        }
    }
}