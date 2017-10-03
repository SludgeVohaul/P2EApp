using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Emby.ApiClient.Model;
using MediaBrowser.Model.Dto;

namespace P2E.Interfaces.DataObjects.Emby
{
    public interface IEmbyClient : IApiClient, IClient
    {
        Task<T> SendAsync<T>(string url,
                             string requestMethod,
                             Dictionary<string, string> args = null,
                             CancellationToken cancellationToken = default(CancellationToken)) where T : class;

        Task UpdateItemAsync(string id,
                             BaseItemDto baseItemDto,
                             CancellationToken cancellationToken = default(CancellationToken));
    }
}