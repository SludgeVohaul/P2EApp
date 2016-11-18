﻿using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.Services.Emby;
using P2E.Interfaces.Repositories.Emby;

namespace P2E.Services.Emby
{
    public class EmbyService : IEmbyService
    {
        private IEmbyRepository _repository;

        public EmbyService(IEmbyRepository embyRepository)
        {
            _repository = embyRepository;
        }

        public bool TryExecute(IEmbyClient embyClient)
        {
            _repository.Client = embyClient;

            _repository.GetStuff();

            return true;
        }
    }
}
