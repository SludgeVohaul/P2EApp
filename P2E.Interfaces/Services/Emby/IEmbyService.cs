using System.Collections.Generic;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Emby.Library;

namespace P2E.Interfaces.Services.Emby
{
    public interface IEmbyService : IService
    {
        Task<ILibraryIdentifier> GetLibraryIdentifierAsync(string libraryName);
        Task<IReadOnlyCollection<IMovieIdentifier>> GetMovieIdentifiersAsync(ILibraryIdentifier libraryIdentifier);
    }
}
