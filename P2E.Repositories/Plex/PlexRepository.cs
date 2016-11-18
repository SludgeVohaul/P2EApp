using P2E.Interfaces.DataObjects.Plex;
using P2E.Interfaces.Repositories.Plex;
using MediaBrowser.Model.Logging;

namespace P2E.Repositories.Plex
{
    public class PlexRepository : IPlexRepository
    {
        private readonly ILogger _logger;

        public IPlexClient Client { get; set; }

        public PlexRepository(ILogger logger)
        {
            _logger = logger;
        }
    }
        
}
