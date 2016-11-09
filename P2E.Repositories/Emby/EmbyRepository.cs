using System;
using System.Linq;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Querying;
using P2E.ExtensionMethods;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.Repositories;

namespace P2E.Repositories.Emby
{
    public class EmbyRepository : IEmbyRepository
    {
        private readonly ILogger _logger;
        private readonly IEmbyClient _embyClient;

        public EmbyRepository(ILogger logger, IEmbyClient embyClient)
        {
            _logger = logger;
            _embyClient = embyClient;
        }

        public void GetStuff()
        {
            // Get the ten most recently added items for the current user.
            var query = new ItemQuery
            {
                UserId = _embyClient.CurrentUserId,

                SortBy = new[] {ItemSortBy.DateCreated},
                SortOrder = SortOrder.Descending,

                // Get media only, don't return folder items
                Filters = new[] {ItemFilter.IsNotFolder},

                Limit = 10,

                // Search recursively through the user's library
                Recursive = true
            };

            try
            {
                var getItemsTask = _embyClient.GetItemsAsync(query);

                getItemsTask.Wait();
                var items = getItemsTask.Result;
            }
            catch (AggregateException ae)
            {
                ae.Flatten().InnerExceptions
                    .ToList()
                    .Select(e => e.GetInnermostException())
                    .ToList()
                    .ForEach(e => _logger.ErrorException(e.Message, e));
            }
        }
    }
}
