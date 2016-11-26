using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using P2E.Interfaces.Repositories.Plex;
using MediaBrowser.Model.Logging;
using P2E.Interfaces.DataObjects.Plex;
using P2E.Repositories.Plex.ResponseElements;
using RestSharp;

namespace P2E.Repositories.Plex
{
    public class PlexRepository : IPlexRepository
    {
        private readonly ILogger _logger;

        public PlexRepository(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<string> GetLibraryUrlAsync(IPlexClient client, string libraryName)
        {
            var request = new RestRequest("library/sections/all", Method.GET);
            var response = await client.ExecuteTaskAsync<MediaContainer>(request);

            var libraryId = response.Data.Directories
                .Where(x => Equals(x.Title, libraryName))
                .Select(x => x.Key)
                .FirstOrDefault();

            return string.IsNullOrEmpty(libraryId)
                ? null
                : $"library/sections/{libraryId}/all";
        }

        // TODO remove
        //private Task<string> DoSleepAsync()
        //{
        //    Thread.Sleep(4000);
        //    return Task.FromResult("25");
        //}


    }
        
}
