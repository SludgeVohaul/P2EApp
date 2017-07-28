using System;
using System.Threading;
using System.Threading.Tasks;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Services.SpinWheel;

namespace P2E.Services.SpinWheel
{
    public class SpinWheelService : ISpinWheelService
    {
        private static readonly SemaphoreSlim SemSlim = new SemaphoreSlim(1, 1);

        private readonly IAppLogger _logger;
        private readonly ISpinWheel _spinWheel;

        public SpinWheelService(IAppLogger logger, ISpinWheel spinWheel)
        {
            _logger = logger;
            _spinWheel = spinWheel;
        }

        public async Task StartSpinWheelAsync(CancellationToken ct)
        {
            await StartSpinWheelAsync(null, ct);
        }

        public async Task StartSpinWheelAsync(int? totalItemsCount, CancellationToken ct)
        {
            try
            {
                await SemSlim.WaitAsync(ct).ContinueWith(async t =>
                {
                    _logger.Debug("SemSlim in.");

                    try
                    {
                        await _spinWheel.SpinAsync(totalItemsCount, ct);
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorException("Cannot start SpinWheel:", ex, ex.Message);
                    }
                    finally
                    {
                        _logger.Debug("SemSlim out.");
                        SemSlim.Release();
                    }
                }, ct);
            }
            catch (OperationCanceledException)
            {
                _logger.Debug("Waiting for the semaphore has been canceled.");
            }
            catch (Exception ex)
            {
                _logger.ErrorException("SpinWheel is not available:", ex, ex.Message);
            }
        }

        public void StopSpinWheel(CancellationTokenSource cts)
        {
            cts.Cancel();
        }

        public void OnItemProcessed(object sender, EventArgs e)
        {
            _spinWheel.IncreaseProcessedItemsCount();
        }
    }
}