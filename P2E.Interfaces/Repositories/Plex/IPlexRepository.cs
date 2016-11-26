using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Plex;

namespace P2E.Interfaces.Repositories.Plex
{
    public interface IPlexRepository : IRepository
    {
        Task<string> GetLibraryUrlAsync(IPlexClient client, string libraryName);
    }
}
