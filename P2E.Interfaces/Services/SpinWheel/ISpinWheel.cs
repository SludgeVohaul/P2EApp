using System.Threading;
using System.Threading.Tasks;

namespace P2E.Interfaces.Services.SpinWheel
{
    public interface ISpinWheel
    {
        Task SpinAsync(CancellationToken cancellationToken);
        Task SpinAsync(int? totalItemsCount, CancellationToken cancellationToken);
        void IncreaseProcessedItemsCount();
    }
}