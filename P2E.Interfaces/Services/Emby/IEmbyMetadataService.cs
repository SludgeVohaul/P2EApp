using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Emby.Library;

namespace P2E.Interfaces.Services.Emby
{
    public interface IEmbyMetadataService : IService
    {
        Task<bool> UpdateMetadataAsync(IEmbyMovieMetadata movieMetadata, IMovieIdentifier movieIdentifier);
    }
}