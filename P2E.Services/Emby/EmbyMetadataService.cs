using System;
using System.Threading;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Emby.Library;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Repositories.Emby;
using P2E.Interfaces.Services.Emby;

namespace P2E.Services.Emby
{
    public class EmbyMetadataService : EmbyBaseService, IEmbyMetadataService
    {
        private static readonly SemaphoreSlim SemSlim = new SemaphoreSlim(1, 1);

        public EmbyMetadataService(IAppLogger logger, IEmbyClient client, IEmbyRepository embyRepository)
            : base(logger, client, embyRepository)
        {
        }

        /// <remarks>
        /// FYI: This method is rather just a wrapper, as the only way to update any metadata of an Emby item is to
        /// send all of the item's metadata properties back to the server. So the heavy work is done in  EmbyRepository.
        /// </remarks>
        public async Task<bool> UpdateMetadataAsync(IEmbyMovieMetadata movieMetadata, IMovieIdentifier movieIdentifier)
        {
            await SemSlim.WaitAsync();
            try
            {
                Logger.Log(Severity.Info, "Updating metadata.");
                await Repository.UpdateMetadataAsync(Client, movieMetadata, movieIdentifier.Id);
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, "Failed to update metadata:");
                return false;
            }
            finally
            {
                SemSlim.Release();
            }
        }
    }
}