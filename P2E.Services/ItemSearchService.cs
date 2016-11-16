using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.Factories;
using P2E.Interfaces.Services;
using P2E.Interfaces.Repositories;

namespace P2E.Services
{
    public class ItemSearchService : IItemSearchService
    {
        private readonly IRepositoryFactory _repositoryFactory;
        public IEmbyClient EmbyClient { get; set; }

        public ItemSearchService(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public bool TryExecute(IEmbyClient embyClient)
        {
            var repository = _repositoryFactory.CreateRepository<IEmbyRepository>(embyClient);

            repository.GetStuff();

            return true;
        }
    }
}
