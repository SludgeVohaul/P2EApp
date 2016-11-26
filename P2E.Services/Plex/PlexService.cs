using System.Threading.Tasks;
using MediaBrowser.Model.Logging;
using P2E.Interfaces.DataObjects.Plex;
using P2E.Interfaces.Services.Plex;
using P2E.Interfaces.Repositories.Plex;

namespace P2E.Services.Plex
{
    public class PlexService : IPlexService
    {
        private readonly ILogger _logger;
        private readonly IPlexRepository _repository;

        public PlexService(ILogger logger, IPlexRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<string> GetLibraryUrlAsync(IPlexClient client, string libraryName)
        {
            using (var spinWheel = new SpinWheel(_logger))
            {
                var ignoredTask = spinWheel.SpinAsync();
                return await _repository.GetLibraryUrlAsync(client, libraryName);
            }
        }
    }
}
