using System.Collections.Generic;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.DataObjects.Emby.Library;

namespace P2E.Interfaces.Repositories.Emby
{
    public interface IEmbyRepository : IRepository
    {
        Task<IReadOnlyCollection<ILibraryIdentifier>> GetLibraryIdentifiersAsync(IEmbyClient client);
        Task<IReadOnlyCollection<IFilenameIdentifier>> GetFilenameIdentifiersAsync(IEmbyClient client, ILibraryIdentifier libraryIdentifier);

        Task<IReadOnlyCollection<ICollectionIdentifier>> GetCollectionsAsync(IEmbyClient client);

        //Task<IList<IMovieIdentifier>> GetMovieIdsAsync(IEmbyClient client, string libraryName);
        //Task GetMovieItemsAsync(string filename, IEmbyClient client);
        //Task GetStuffAsync(IEmbyClient client);
    }
}