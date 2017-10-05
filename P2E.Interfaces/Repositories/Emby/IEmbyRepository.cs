using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.System;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Emby.Library;

namespace P2E.Interfaces.Repositories.Emby
{
    public interface IEmbyRepository : IRepository
    {
        Task<IReadOnlyCollection<ILibraryIdentifier>> GetLibraryIdentifiersAsync(IEmbyClient client);
        Task<IReadOnlyCollection<IMovieIdentifier>> GetMovieIdentifiersAsync(IEmbyClient client, string libraryId);

        Task<IReadOnlyCollection<ICollectionIdentifier>> GetCollectionIdentifiersAsync(IEmbyClient client);
        Task<ICollectionIdentifier> CreateCollectionAsync(IEmbyClient client, string collectionName);
        Task AddMovieToCollectionAsync(IEmbyClient client, string movieId, string collectionId);
        Task<PublicSystemInfo> GetPublicSystemInfoAsync(IEmbyClient client);

        Task AddImageToMovieAsync(IEmbyClient client, ImageType imageType, Uri imageUrl, string movieId);
        Task ReindexImageOfMovieAsync(IEmbyClient client, ImageType imageType, int index, int newIndex, string movieId);
        Task<IReadOnlyCollection<ImageInfo>> GetImageInfosAsync(IEmbyClient client, string movieId);
        Task DeleteImageFromMovieAsync(IEmbyClient client, ImageType imageType, int index, string movieId);

        Task UpdateMetadataAsync(IEmbyClient client, IEmbyMovieMetadata movieMetadata, string movieId);
        Task SetMovieAsWatchedAsync(IEmbyClient client, DateTime lastPlayedDate, string movieId);
        Task SetMovieAsUnwatchedAsync(IEmbyClient client, string movieId);
    }
}