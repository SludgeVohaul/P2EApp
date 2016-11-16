using System;
using System.Collections.Generic;
using System.Linq;
using MediaBrowser.Model.Collections;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Querying;
using P2E.ExtensionMethods;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.Repositories;
using MediaBrowser.Model.Entities;

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
                //Filters = new[] {ItemFilter.IsNotFolder},
                Filters = new[] { ItemFilter.IsFolder },

                IncludeItemTypes = new[] { "Boxset" },

                Limit = 10,

                // Search recursively through the user's library
                Recursive = true
            };

            try
            {
                var getItemsTask = _embyClient.GetItemsAsync(query);

                getItemsTask.Wait();
                var items = getItemsTask.Result;

                var args = new Dictionary<string, string>
                {
                    {"IsLocked", "false"},
                    {"Name", "Addams Collection"},
                    {"ParentId", ""},
                    {"Ids", ""}
                };
                var colTask = _embyClient.PostAsync<CollectionCreationResult>(_embyClient.GetApiUrl("Collections"), args);
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
