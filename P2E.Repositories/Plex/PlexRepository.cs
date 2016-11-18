using P2E.Interfaces.Repositories.Plex;
using MediaBrowser.Model.Logging;

namespace P2E.Repositories.Plex
{
    public class PlexRepository : IPlexRepository
    {
        private readonly ILogger _logger;

        public PlexRepository(ILogger logger)
        {
            _logger = logger;
        }
    }
        
}
