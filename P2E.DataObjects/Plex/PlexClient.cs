using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaBrowser.Model.Logging;
using P2E.Interfaces.DataObjects;
using P2E.Interfaces.DataObjects.Plex;

namespace P2E.DataObjects.Plex
{
    public class PlexClient : IPlexClient
    {
        public IConnectionInformation ConnectionInformation { get; }

        public PlexClient(ILogger logger, IConnectionInformation connectionInformation)
        {
            ConnectionInformation = connectionInformation;
        }

        public async Task LoginAsync(string loginname, string password)
        {
            // todo
        }
        public async Task LogoutAsync()
        {
            // todo
        }

    }
}
