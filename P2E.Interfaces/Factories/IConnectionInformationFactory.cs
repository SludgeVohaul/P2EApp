using P2E.Interfaces.CommandLine.ServerOptions;
using P2E.Interfaces.DataObjects;

namespace P2E.Interfaces.Factories
{
    public interface IConnectionInformationFactory
    {
        IConnectionInformation<T> CreateConnectionInformation<T>() where T : IConsoleConnectionOptions<T>;
    }
}