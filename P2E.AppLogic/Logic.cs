using P2E.Interfaces.AppLogic;
using P2E.Interfaces.Services;

namespace P2E.AppLogic
{
    public class Logic : ILogic
    {
        private readonly IItemSearchService _itemSearchService;

        public Logic(IItemSearchService itemSearchService)
        {
            _itemSearchService = itemSearchService;
        }

        public void Run()
        {
            _itemSearchService.TryExecute();

        }
    }
}
