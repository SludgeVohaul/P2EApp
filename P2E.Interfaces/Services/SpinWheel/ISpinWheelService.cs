using System;
using System.Threading;
using System.Threading.Tasks;

namespace P2E.Interfaces.Services.SpinWheel
{
    public interface ISpinWheelService : IService
    {
        Task StartSpinWheelAsync(CancellationToken ct);
        Task StartSpinWheelAsync(int? totalItemsCount, CancellationToken ct);
        void StopSpinWheel(CancellationTokenSource cts);
        void OnItemProcessed(object sender, EventArgs e);
    }
}