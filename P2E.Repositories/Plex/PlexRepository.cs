using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using P2E.Interfaces.Repositories.Plex;
using P2E.DataObjects.Plex.Library;
using P2E.Interfaces.DataObjects.Plex;
using P2E.Interfaces.DataObjects.Plex.Library;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Services.Plex.Exceptions;
using P2E.Repositories.Plex.ResponseElements;
using RestSharp;

namespace P2E.Repositories.Plex
{
    public class PlexRepository : IPlexRepository
    {
        private readonly IAppLogger _logger;

        public PlexRepository(IAppLogger logger)
        {
            _logger = logger;
        }

        public async Task<string> GetLibraryIdAsync(IPlexClient client, string libraryName)
        {
            var mediaContainer = await GetResponseData<MediaContainer>(client, "/library/sections/all");
            return mediaContainer.Directories
                .Where(x => Equals(x.Title, libraryName))
                .Select(x => x.Key)
                .FirstOrDefault();
        }

        public async Task<List<IPlexMovieMetadata>> GetMovieLibraryMetadataAsync(IPlexClient client, string libraryId)
        {
            // TODO - Supplying an invalid subset (all, on deck, newest, ...) results in HTTP 200 and an empty list.
            // TODO - Similar with sections => e.g. sectionsssss
            // TODO - Maybe this should be handled here with a warning or something.
            var mediaContainer = await GetResponseData<MediaContainer>(client, $"/library/sections/{libraryId}/all");

            return mediaContainer.Videos
                .Select(x => new PlexMovieMetadata
                {
                    Title = x.Title,
                    TitleSort = x.TitleSort,

                    ViewCount = x.ViewCount,
                    LastViewedAt = x.LastViewedAt,

                    ThumbUri = CreateAbsoluteUri(client.BaseUrl, x.Thumb),
                    ArtUri = CreateAbsoluteUri(client.BaseUrl, x.Art),

                    Collections = x.Collections
                        .Select(c => c.Name).ToList(),

                    Filenames = x.Medias
                        .SelectMany(m => m.Parts)
                        .Select(p => Path.GetFileName(p.FileName))
                        .ToList()
                } as IPlexMovieMetadata)
                .ToList();
        }

        private async Task<T> GetResponseData<T>(IRestClient client, string absolutePath)
        {
            var request = new RestRequest(absolutePath, Method.GET);

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var response = await client.ExecuteTaskAsync<T>(request);
            watch.Stop();

            if (response.ErrorException != null)
            {
                var msg = "Failed to retrieve response.";
                throw new ResponseFailureException(msg,
                                                   request.Method,
                                                   new Uri(client.BaseUrl, request.Resource),
                                                   response.ErrorException);
            }

            _logger.Log(Severity.Debug,
                        $"Received {(int)response.StatusCode} status code after {watch.ElapsedMilliseconds} ms from {response.Request.Method}: {response.ResponseUri}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var msg = $"Received HTTP status code is not '{HttpStatusCode.OK}' ({(int) HttpStatusCode.OK}).";
                throw new UnexpectedHttpStatusCodeException(msg,
                                                            response.StatusCode,
                                                            watch.ElapsedMilliseconds,
                                                            response.Request.Method, response.ResponseUri);
            }

            return response.Data;
        }

        private Uri CreateAbsoluteUri(Uri baseUrl, string absolutePath)
        {
            try
            {
                return string.IsNullOrEmpty(absolutePath)
                    ? null
                    : new Uri(baseUrl, absolutePath);
            }
            catch (Exception ex)
            {
                _logger.Log(Severity.Error, $"Failed to create a valid URI from '{baseUrl}' and '{absolutePath}':");
                _logger.Log(Severity.ErrorException, ex.Message);
                return null;
            }
        }
    }
}
