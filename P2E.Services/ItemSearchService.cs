using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.Factories;
using P2E.Interfaces.Services;

namespace P2E.Services
{
    public class ItemSearchService : IItemSearchService
    {
        private readonly IEmbyRepositoryFactory _embyRepositoryFactory;
        public IEmbyClient EmbyClient { get; set; }

        public ItemSearchService(IEmbyRepositoryFactory embyRepositoryFactory)
        {
            _embyRepositoryFactory = embyRepositoryFactory;
        }

        public bool TryExecute(IEmbyClient embyClient)
        {
            var repository = _embyRepositoryFactory.CreateEmbyRepository(embyClient);

            repository.GetStuff();

            return true;
        }
    }
}
