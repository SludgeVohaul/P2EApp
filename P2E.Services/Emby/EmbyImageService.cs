using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Entities;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Emby.Library;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Repositories.Emby;
using P2E.Interfaces.Services.Emby;

namespace P2E.Services.Emby
{
    public class EmbyImageService : EmbyBaseService, IEmbyImageService
    {
        private static readonly SemaphoreSlim SemSlim = new SemaphoreSlim(1, 1);

        public EmbyImageService(IAppLogger logger, IEmbyClient client, IEmbyRepository embyRepository)
            : base(logger, client, embyRepository)
        {
        }

        public async Task<bool> TryAddImageToMovieAsync(ImageType imageType,
                                                        Uri imageUrl,
                                                        IMovieIdentifier movieIdentifier)
        {
            await SemSlim.WaitAsync();
            if (imageUrl == null)
            {
                Logger.Log(Severity.Warn, $"No {imageType} image available.");
                return true;
            }

            try
            {
                Logger.Log(Severity.Info, $"Adding {imageType} image.");
                await Repository.AddImageToMovieAsync(Client, imageType, imageUrl, movieIdentifier.Id);
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, $"Failed to add {imageType} image:");
                return false;
            }
            finally
            {
                SemSlim.Release();
            }
        }

        public async Task<bool> TryDeleteImageFromMovieAsync(ImageType imageType,
                                                             int imageIndex,
                                                             IMovieIdentifier movieIdentifier)
        {
            await SemSlim.WaitAsync();
            try
            {
                Logger.Log(Severity.Info, $"Deleting {imageType} image at index {imageIndex}.");
                await Repository.DeleteImageFromMovieAsync(Client, imageType, imageIndex, movieIdentifier.Id);
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, $"Failed to delete {imageType} image at index {imageIndex}:");
                return false;
            }
            finally
            {
                SemSlim.Release();
            }
        }

        /// <returns>The index of the last image, or -1 if there are no images of imageType, or null otherwise.</returns>
        public async Task<int?> GetMaxImageIndex(ImageType imageType, IMovieIdentifier movieIdentifier)
        {
            await SemSlim.WaitAsync();
            try
            {
                Logger.Log(Severity.Info, $"Querying the index of last {imageType} image.");
                var imageInfos = await Repository.GetImageInfosAsync(Client, movieIdentifier.Id);

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
            finally
            {
                SemSlim.Release();
            }
        }

        /// <remarks>
        /// There is no way to tell the server to display the added image. One has to
        /// assume that the added image is the one with the highest index. See
        /// https://emby.media/community/index.php?/topic/50676-how-to-re-index-a-new-remoteimage-of-an-item/
        /// But unfortunatellyreindexing does not work reliably, see
        /// https://emby.media/community/index.php?/topic/50794-apiclient-server-3230-cannot-reindex-the-backdrop-image-of-a-movie/
        /// </remarks>
        public async Task<bool> ReindexImageOfMovieAsync(ImageType imageType,
                                                         int currentIndex,
                                                         int newIndex,
                                                         IMovieIdentifier movieIdentifier)
        {
            await SemSlim.WaitAsync();
            try
            {
                Logger.Log(Severity.Info, $"Reindexing {imageType} image from index {currentIndex} to index {newIndex}.");
                await Repository.ReindexImageOfMovieAsync(Client, imageType, currentIndex, newIndex, movieIdentifier.Id);
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, $"Failed to reindex {imageType} image:");
                return false;
            }
            finally
            {
                SemSlim.Release();
            }
        }
    }
}