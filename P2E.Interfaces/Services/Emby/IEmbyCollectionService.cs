using System.Collections.Generic;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Emby.Library;
using OperationgSystem = MediaBrowser.Model.System.OperatingSystem;

namespace P2E.Interfaces.Services.Emby
{
    public interface IEmbyCollectionService : IService
    {
        Task<IReadOnlyCollection<ICollectionIdentifier>> GetCollectionIdentifiersAsync();
        Task<ICollectionIdentifier> CreateCollectionAsync(string collectionName);
        Task<bool> TryAddMovieToCollectionAsync(IMovieIdentifier movieIdentifier, ICollectionIdentifier collectionIdentifier);
        Task<OperationgSystem?> GetServerOperatingSystemAsync();
    }
}