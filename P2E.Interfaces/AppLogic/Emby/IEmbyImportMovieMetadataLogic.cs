using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Emby.Library;

namespace P2E.Interfaces.AppLogic.Emby
{
    public interface IEmbyImportMovieMetadataLogic : ILogic
    {
        Task<bool> RunAsync(IEmbyMovieMetadata movieMetadata, IMovieIdentifier movieIdentifier);
    }
}