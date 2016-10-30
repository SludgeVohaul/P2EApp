using System;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Querying;
using P2E.Extensions.Exception;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.Repositories;
using P2E.Interfaces.Services;

namespace P2E.Repositories
{
    public class EmbyRepository : EmbyBaseRepository, IEmbyRepository
    {
        public EmbyRepository(ILogger logger, IUserCredentialsService userCredentialsService, IEmbyConnectionService embyConnectionService, IEmbyClient embyClient)
            : base(logger, userCredentialsService, embyConnectionService, embyClient)
        {
        }

        public void GetStuff()
        {
            // Get the ten most recently added items for the current user.
            var query = new ItemQuery
            {
                UserId = EmbyClient.CurrentUserId,

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
                var getItemsTask = EmbyClient.GetItemsAsync(query);

                getItemsTask.Wait();
                var items = getItemsTask.Result;
            }
            catch (AggregateException ae)
            {
                foreach (var innerException in ae.GetInnerExceptions())
                {
                    if (innerException is AggregateException) continue;
                    Logger.Error(innerException.Message);
                }
            }
        }
    }
}
