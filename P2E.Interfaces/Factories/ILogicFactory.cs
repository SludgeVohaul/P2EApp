using P2E.Interfaces.AppLogic;

namespace P2E.Interfaces.Factories
{
    public interface ILogicFactory
    {
        T CreateLogic<T>() where T : ILogic;
    }
}