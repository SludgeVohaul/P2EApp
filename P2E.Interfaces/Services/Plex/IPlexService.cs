﻿using P2E.Interfaces.DataObjects.Plex;

namespace P2E.Interfaces.Services.Plex
{
    public interface IPlexService : IService
    {
        bool TryExecute(IPlexClient plexClient);
    }
}
