using System.Collections.Generic;
using System.Threading.Tasks;
using MediaBrowser.Model.Entities;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Emby.Library;

namespace P2E.Interfaces.Repositories.Emby
{
    public interface IEmbyRepository : IRepository
    {
        Task<IReadOnlyCollection<ILibraryIdentifier>> GetLibraryIdentifiersAsync(IEmbyClient client);
        Task<IReadOnlyCollection<IFilenameIdentifier>> GetFilenameIdentifiersAsync(IEmbyClient client, ILibraryIdentifier libraryIdentifier);

        Task<IReadOnlyCollection<ICollectionIdentifier>> GetCollectionIdentifiersAsync(IEmbyClient client);
        Task<ICollectionIdentifier> CreateCollectionAsync(IEmbyClient client, string pathBasename);

        Task AddMovieToCollectionAsync(IEmbyClient client, string movieId, string collectionId);
        Task AddImageToMovie(IEmbyClient client, string movieId, ImageType imageType, string imageUrl);
        Task DeleteImagesFromMovie(IEmbyClient client, string movieId, ImageType imageType);


        //Task<IList<IMovieIdentifier>> GetMovieIdsAsync(IEmbyClient client, string libraryName);
        //Task GetMovieItemsAsync(string filename, IEmbyClient client);
        //Task GetStuffAsync(IEmbyClient client);
    }
}