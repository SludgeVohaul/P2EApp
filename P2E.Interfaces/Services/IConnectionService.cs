using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P2E.Interfaces.DataObjects;

namespace P2E.Interfaces.Services
{
    public interface IConnectionService
    {
        bool TryLogin(IClient embyClient, IUserCredentials userCredentials);
        void Logout(IClient embyClient);
    }
}
