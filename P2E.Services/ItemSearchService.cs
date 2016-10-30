using P2E.Interfaces.Repositories;
using P2E.Interfaces.Services;

namespace P2E.Services
{
    public class ItemSearchService : IItemSearchService
    {
        private readonly IEmbyRepository _repository;

        public ItemSearchService(IEmbyRepository repository)
        {
            _repository = repository;
        }

        public bool TryExecute()
        {
            try
            {
                if (_repository.TryConnect() == false) return false;
                //_repository.SetClientCapabilities();
                _repository.GetStuff();



                return true;
            }
            finally
            {
                _repository.Disconnect();
            }

            
        }
    }
}
