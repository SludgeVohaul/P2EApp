using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.Repositories;
using P2E.Interfaces.Services;

namespace P2E.Services
{
    public class ItemSearchService : IItemSearchService
    {
        private readonly IEmbyRepository _repository;

        public IEmbyClient EmbyClient { get; set; }

        public ItemSearchService(IEmbyRepository repository)
        {
            _repository = repository;
        }

        public bool TryExecute()
        {
            _repository.EmbyClient = EmbyClient;

            _repository.GetStuff();

            return true;
        }
    }
}
