using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Emby.Library;

namespace P2E.Interfaces.Repositories.Emby
{
    public interface IEmbyRepository : IRepository
    {
        Task<IReadOnlyCollection<ILibraryIdentifier>> GetLibraryIdentifiersAsync(IEmbyClient client);
        Task<IReadOnlyCollection<IMovieIdentifier>> GetMovieIdentifiersAsync(IEmbyClient client, string libraryId);

        Task<IReadOnlyCollection<ICollectionIdentifier>> GetCollectionIdentifiersAsync(IEmbyClient client);
        Task<ICollectionIdentifier> CreateCollectionAsync(IEmbyClient client, string pathBasename);

        Task AddMovieToCollectionAsync(IEmbyClient client, string movieId, string collectionId);
        Task AddImageToMovieAsync(IEmbyClient client, ImageType imageType, Uri imageUrl, string movieId);
        Task ReindexImageOfMovieAsync(IEmbyClient client, ImageType imageType, int index, int newIndex, string movieId);
        Task<IReadOnlyCollection<ImageInfo>> GetImageInfosAsync(IEmbyClient client, string movieId);
        Task DeleteImageFromMovieAsync(IEmbyClient client, ImageType imageType, int index, string movieId);

        Task UpdateMetadataAsync(IEmbyClient client, IEmbyMovieMetadata movieMetadata, string movieId);



        //Task<IList<IMovieIdentifier>> GetMovieIdsAsync(IEmbyClient client, string libraryName);
        //Task GetMovieItemsAsync(string filename, IEmbyClient client);
        //Task GetStuffAsync(IEmbyClient client);
    }
}