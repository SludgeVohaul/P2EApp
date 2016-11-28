using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using P2E.Interfaces.Repositories.Plex;
using P2E.DataObjects.Plex.Library;
using P2E.Interfaces.DataObjects.Plex;
using P2E.Interfaces.DataObjects.Plex.Library;
using P2E.Repositories.Plex.ResponseElements;
using RestSharp;

namespace P2E.Repositories.Plex
{
    public class PlexRepository : IPlexRepository
    {
        public async Task<string> GetLibraryUrlAsync(IPlexClient client, string libraryName)
        {
            //return await DoSleepAsync();
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

        public async Task<List<IPlexMovieMetadata>> GetMovieMetadataAsync(IPlexClient client, string libraryUrl)
        {
            //await DoSleepAsync();
            // TODO remove
            //var bla = new DateTime(342424324, 1, 1, 1, 1, 1);
            var request = new RestRequest(libraryUrl, Method.GET);
            var response = await client.ExecuteTaskAsync<MediaContainer>(request);

            return response.Data.Videos
                .Select(x => new PlexMovieMetadata
                {
                    Title = x.Title,
                    OriginalTitle = x.OriginalTitle,
                    TitleSort = x.TitleSort,
                    ViewCount = x.ViewCount,
                    Collections = x.Collections.Select(c => c.Name).ToList()
                } as IPlexMovieMetadata)
                .ToList();
        }

        // TODO remove
        //private async Task<string> DoSleepAsync()
        //{
        //    await Task.Delay(4000);
        //    return "25";
        //}
    }
}
