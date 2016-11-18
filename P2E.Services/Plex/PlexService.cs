using P2E.Interfaces.DataObjects.Plex;
using P2E.Interfaces.Services.Plex;
using P2E.Interfaces.Repositories.Plex;

namespace P2E.Services.Plex
{
    public class PlexService : IPlexService
    {
        private IPlexRepository _repository;

        public PlexService(IPlexRepository plexRepository)
        {
            _repository = plexRepository;
        }

        public bool TryExecute(IPlexClient plexClient)
        {
            _repository.Client = plexClient;    

            return true;
        }
    }
}
