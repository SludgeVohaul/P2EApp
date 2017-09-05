using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.ApiClient;

namespace P2E.Interfaces.DataObjects.Emby
{
    public interface IEmbyClient : IApiClient, IClient
    {
        Task<T> DeleteAsync<T>(string url, CancellationToken cancellationToken = default(CancellationToken)) where T : class;
    }
}