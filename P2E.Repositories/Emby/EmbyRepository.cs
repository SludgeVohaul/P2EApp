using System;
using System.Collections.Generic;
using System.Linq;
using MediaBrowser.Model.Collections;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Querying;
using P2E.ExtensionMethods;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.Repositories.Emby;
using MediaBrowser.Model.Entities;

namespace P2E.Repositories.Emby
{
    public class EmbyRepository : IEmbyRepository
    {
        private readonly ILogger _logger;

        public EmbyRepository(ILogger logger)
        {
            _logger = logger;
        }

        public void GetStuff(IEmbyClient client)
        {
            // TODO cleanup
            // Get the ten most recently added items for the current user.
            var query = new ItemQuery
            {
                UserId = client.CurrentUserId,

                SortBy = new[] {ItemSortBy.DateCreated},
                SortOrder = SortOrder.Descending,

                // Get media only, don't return folder items
                //Filters = new[] {ItemFilter.IsNotFolder},
                Filters = new[] { ItemFilter.IsFolder },

                IncludeItemTypes = new[] { "Boxset" },

                Limit = 10,

                // Search recursively through the user's library
                Recursive = true
            };

            try
            {
                var getItemsTask = client.GetItemsAsync(query);

                getItemsTask.Wait();
                var items = getItemsTask.Result;

                var args = new Dictionary<string, string>
                {
                    {"IsLocked", "false"},
                    {"Name", "Addams Collection"},
                    {"ParentId", ""},
                    {"Ids", ""}
                };
                var colTask = client.PostAsync<CollectionCreationResult>(client.GetApiUrl("Collections"), args);
                colTask.Wait();
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
