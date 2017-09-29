using System.Threading.Tasks;

namespace P2E.Interfaces.AppLogic
{
    public interface IMainLogic : ILogic
    {
        Task<bool> RunAsync();
    }
}