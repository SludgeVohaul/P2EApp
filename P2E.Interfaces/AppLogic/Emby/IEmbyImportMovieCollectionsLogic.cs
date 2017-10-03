using System.Collections.Generic;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Emby.Library;

namespace P2E.Interfaces.AppLogic.Emby
{
    public interface IEmbyImportMovieCollectionsLogic : ILogic
    {
        Task<bool> RunAsync(IReadOnlyCollection<string> plexMovieCollections, IMovieIdentifier embyMovieIdentifier);
    }
}