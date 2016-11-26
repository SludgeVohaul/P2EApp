using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Plex;

namespace P2E.Interfaces.Services.Plex
{
    public interface IPlexService : IService
    {
        Task<string> GetLibraryUrlAsync(IPlexClient client, string libraryName);
    }
}
