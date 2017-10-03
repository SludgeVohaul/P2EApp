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
                Logger.Log(Severity.Info, "Querying existing libraries.");
                var libraryIdentifiers = await Repository.GetLibraryIdentifiersAsync(Client);
                return libraryIdentifiers.FirstOrDefault(x => x.Name == libraryName);
            }
            catch (Exception ex)
            {
                LogException(ex, "Failed to query existing libraries:");
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
                Logger.Log(Severity.Info, "Querying existing movies.");
                return await Repository.GetMovieIdentifiersAsync(Client, libraryIdentifier.Id);
            }
            catch (Exception ex)
            {
                LogException(ex, "Failed to query existing movies:");
                return null;
            }
            finally
            {
                SemSlim.Release();
            }
        }
    }
}
