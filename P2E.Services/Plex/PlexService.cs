using System.Collections.Generic;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects.Plex;
using P2E.Interfaces.DataObjects.Plex.Library;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Services.Plex;
using P2E.Interfaces.Repositories.Plex;
using P2E.Interfaces.Services.Plex.Exceptions;

namespace P2E.Services.Plex
{
    public class PlexService : IPlexService
    {
        private readonly IAppLogger _logger;
        private readonly IPlexClient _client;
        private readonly IPlexRepository _repository;

        public PlexService(IAppLogger logger, IPlexClient client, IPlexRepository repository)
        {
            _logger = logger;
            _client = client;
            _repository = repository;
        }

        public async Task<List<IPlexMovieMetadata>> GetMovieMetadataAsync(string libraryName)
        {
            try
            {
                var libraryId = await _repository.GetLibraryIdAsync(_client, libraryName);
                if (libraryId == null)
                {
                    _logger.Error($"Library '{libraryName}' not found!");
                    return null;
                }

                return await _repository.GetMovieLibraryMetadataAsync(_client, libraryId);
            }
            catch (ResponseFailureException ex)
            {
                _logger.ErrorException($"{ex.Message}\nRequest: {ex.RequestMethod}: {ex.RequestUri}", ex, ex.InnerException?.Message);
                return null;
            }
            catch (UnexpectedHttpStatusCodeException ex)
            {
                var msg = $"Received '{ex.StatusCode}' ({(int)ex.StatusCode}) status code after {ex.ResponseDuration} ms from {ex.RequestMethod}: {ex.ResponseUri}";
                _logger.ErrorException($"{msg}\n{ex.Message}", ex);
                return null;
            }
        }
    }
}
