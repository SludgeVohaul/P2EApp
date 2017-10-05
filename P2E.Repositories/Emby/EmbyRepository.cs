using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Emby.ApiClient.Model;
using MediaBrowser.Model.Collections;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Querying;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.Repositories.Emby;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.System;
using P2E.DataObjects.Emby.Library;
using P2E.Interfaces.DataObjects.Emby.Library;

namespace P2E.Repositories.Emby
{
    public class EmbyRepository : IEmbyRepository
    {
        public async Task<IReadOnlyCollection<ILibraryIdentifier>> GetLibraryIdentifiersAsync(IEmbyClient client)
        {
            var itemsResult = await client.GetUserViews(client.CurrentUserId);

            return itemsResult.Items
                .Select(x => new LibraryIdentifier
                {
                    Name = x.Name,
                    Id = x.Id
                })
                .ToArray();
        }

        public async Task<IReadOnlyCollection<IMovieIdentifier>> GetMovieIdentifiersAsync(IEmbyClient client, string libraryId)
        {
            var query = new ItemQuery
            {
                UserId = client.CurrentUserId,
                ParentId = libraryId,
                Filters = new[] { ItemFilter.IsNotFolder },
                IncludeItemTypes = new[] { "Movie" },
                Fields = new[] { ItemFields.Path },
                Recursive = true
            };
            var itemsResult = await client.GetItemsAsync(query);

            return itemsResult.Items
                .Select(x => new MovieIdentifier
                {
                    Filename = Path.GetFileName(x.Path),
                    Id = x.Id
                })
                .ToArray();
        }

        public async Task<IReadOnlyCollection<ICollectionIdentifier>> GetCollectionIdentifiersAsync(IEmbyClient client)
        {
            var query = new ItemQuery
            {
                UserId = client.CurrentUserId,
                // FYI: Collections are folders.
                Filters = new[] { ItemFilter.IsFolder },
                IncludeItemTypes = new[] { "Boxset" },
                Fields = new[] { ItemFields.Path },
                Recursive = true
            };
            var itemsResult = await client.GetItemsAsync(query);

            return itemsResult.Items
                .Select(x => new CollectionIdentifier
                {
                    Path = x.Path,
                    Id = x.Id,
                    Name = x.Name
                })
                .ToArray();
        }

        /// <remarks>Please read
        /// https://emby.media/community/index.php?/topic/50514-apiclient-how-to-check-whether-an-arbitrary-string-matches-an-existing-boxset/
        /// </remarks>
        public async Task<ICollectionIdentifier> CreateCollectionAsync(IEmbyClient client, string collectionName)
        {
            var args = new Dictionary<string, string>
            {
                {"IsLocked", "false"},
                {"Name", collectionName},
                {"ParentId", ""},
                {"Ids", ""}
            };
            var url = client.GetApiUrl("Collections");
            var collectionCreationResult = await client.SendAsync<CollectionCreationResult>(url, "POST", args);

            var baseItemDto = await client.GetItemAsync(collectionCreationResult.Id, client.CurrentUserId);

            return new CollectionIdentifier
            {
                Path = baseItemDto.Path,
                Id = collectionCreationResult.Id,
                Name = baseItemDto.Name
            };
        }

        /// <remarks>See
        /// https://github.com/MediaBrowser/Emby/blob/master/MediaBrowser.Api/Movies/CollectionService.cs
        /// for the POST URL.</remarks>
        public async Task AddMovieToCollectionAsync(IEmbyClient client, string movieId, string collectionId)
        {
            var args = new Dictionary<string, string>
            {
                {"Ids", movieId}
            };
            var url = client.GetApiUrl($"Collections/{collectionId}/Items");
            await client.SendAsync<EmptyRequestResult>(url, "POST", args);

        }

        public async Task AddImageToMovieAsync(IEmbyClient client, ImageType imageType, Uri imageUrl, string movieId)
        {
            var args = new Dictionary<string, string>
            {
                {"Type", imageType.ToString()},
                {"ImageUrl", imageUrl.AbsoluteUri}
            };

            var url = client.GetApiUrl($"Items/{movieId}/RemoteImages/Download");
            await client.SendAsync<EmptyRequestResult>(url, "POST", args);
        }


        /// <remarks>
        /// There seems to be some issue somewhere, which prevents a image from being reindexed.
        /// See
        /// https://emby.media/community/index.php?/topic/50794-apiclient-server-3230-cannot-reindex-the-backdrop-image-of-a-movie/
        /// </remarks>
        public async Task ReindexImageOfMovieAsync(IEmbyClient client, ImageType imageType, int index, int newIndex, string movieId)
        {
            var args = new Dictionary<string, string>
            {
                { "newIndex", newIndex.ToString()}
            };

            var url = client.GetApiUrl($"Items/{movieId}/Images/{imageType}/{index}/Index");
            await client.SendAsync<EmptyRequestResult>(url, "POST", args);
        }

        public async Task<IReadOnlyCollection<ImageInfo>> GetImageInfosAsync(IEmbyClient client, string movieId)
        {
            var url = client.GetApiUrl($"Items/{movieId}/Images");
            return await client.SendAsync<List<ImageInfo>>(url, "GET");
        }

        public async Task DeleteImageFromMovieAsync(IEmbyClient client, ImageType imageType, int index, string movieId)
        {
            var url = client.GetApiUrl($"Items/{movieId}/Images/{imageType}/{index}");
            await client.SendAsync<EmptyRequestResult>(url, "DELETE");
        }

        public async Task<PublicSystemInfo> GetPublicSystemInfoAsync(IEmbyClient client)
        {
            var url = client.GetApiUrl("System/Info/Public");
            return await client.SendAsync<PublicSystemInfo>(url, "GET");
        }

        public async Task UpdateMetadataAsync(IEmbyClient client, IEmbyMovieMetadata movieMetadata, string movieId)
        {
            var baseItemDto = await client.GetItemAsync(movieId, client.CurrentUserId);

            baseItemDto.Name = movieMetadata.Name;
            baseItemDto.ForcedSortName = movieMetadata.ForcedSortName;

            await client.UpdateItemAsync(movieId, baseItemDto);
        }

        public async Task SetMovieAsWatchedAsync(IEmbyClient client, DateTime lastPlayedDate, string movieId)
        {
            var args = new Dictionary<string, string>
            {
                { "DatePlayed", lastPlayedDate.ToLocalTime().ToString("yyyyMMddHHmmss")}
            };

            var url = client.GetApiUrl($"Users/{client.CurrentUserId}/PlayedItems/{movieId}");
            await client.SendAsync<EmptyRequestResult>(url, "POST", args);
        }

        public async Task SetMovieAsUnwatchedAsync(IEmbyClient client, string movieId)
        {
            var url = client.GetApiUrl($"Users/{client.CurrentUserId}/PlayedItems/{movieId}");
            await client.SendAsync<EmptyRequestResult>(url, "DELETE");
        }
    }
}
