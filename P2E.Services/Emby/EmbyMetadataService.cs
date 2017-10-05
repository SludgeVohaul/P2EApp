using System;
using System.Threading;
using System.Threading.Tasks;
using P2E.DataObjects.Emby.Library;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Emby.Library;
using P2E.Interfaces.DataObjects.Plex.Library;
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
        public async Task<bool> UpdateMetadataAsync(IPlexMovieMetadata plexMovieMetadata, IMovieIdentifier movieIdentifier)
        {
            await SemSlim.WaitAsync();
            try
            {
                var embyMovieMetadata = new EmbyMovieMetadata
                {
                    Name = plexMovieMetadata.Title,
                    ForcedSortName = plexMovieMetadata.TitleSort
                };
                Logger.Log(Severity.Info, "Updating metadata.");
                await Repository.UpdateMetadataAsync(Client, embyMovieMetadata, movieIdentifier.Id);
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

        public async Task<bool> UpdateWatchedStatusAsync(IPlexMovieMetadata plexMovieMetadata,
                                                         IMovieIdentifier movieIdentifier)
        {
            await SemSlim.WaitAsync();
            try
            {
                Logger.Log(Severity.Info, "Determining Watched/Unwatched status.");
                var playCount = plexMovieMetadata.ViewCount ?? 0;

                var lastPlayedDate = plexMovieMetadata.LastViewedAt == null
                    ? default(DateTime?)
                    : new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                        .AddSeconds(Convert.ToDouble(plexMovieMetadata.LastViewedAt));

                Logger.Log(Severity.Info, "Setting as 'Unwatched'.");
                await Repository.SetMovieAsUnwatchedAsync(Client, movieIdentifier.Id);

                if (playCount > 0)
                {
                    if (lastPlayedDate == null) throw new Exception("LastPlayedDate is null and PlayCount greater than 0.");

                    Logger.Log(Severity.Info, $"Setting as 'Watched' with playcount '{playCount}'.");
                    for (var i = 0; i < playCount; i++)
                    {
                        await Repository.SetMovieAsWatchedAsync(Client, lastPlayedDate.Value, movieIdentifier.Id);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, "Failed to update Watched/Unwatched:");
                return false;
            }
            finally
            {
                SemSlim.Release();
            }
        }
    }
}