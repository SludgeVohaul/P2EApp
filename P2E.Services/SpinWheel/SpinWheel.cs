using System;
using System.Threading;
using System.Threading.Tasks;
using AppConsole;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Services.SpinWheel;

namespace P2E.Services.SpinWheel
{
    public class SpinWheel : ConsoleLock, ISpinWheel
    {
        private readonly object _itemCountLockObject = new object();

        private readonly IAppLogger _logger;

        private readonly string[] _spinPositions = { "|", "/", "-", @"\" };

        private int _processedItemsCount;

        public SpinWheel(IAppLogger logger)
        {
            _logger = logger;
        }

        public async Task SpinAsync(CancellationToken cancellationToken)
        {
            await SpinAsync(null, cancellationToken);
        }

        public async Task SpinAsync(int? totalItemsCount, CancellationToken cancellationToken)
        {
            var spinPosition = 0;

            await Task.Run(async () =>
            {
                try
                {
                    Console.CursorVisible = false;
                    _logger.Debug("Spinwheel is spinning...");
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    while (cancellationToken.IsCancellationRequested == false)
                    {
                        var spinWheelString = totalItemsCount.HasValue
                            ? $"({GetProcessedItemsCount()} / {totalItemsCount.Value}) {_spinPositions[spinPosition]}"
                            : $"{_spinPositions[spinPosition]}";

                        lock (ConsoleLockObject)
                        {
                            Console.Out.Write(spinWheelString);
                            Console.SetCursorPosition(Console.CursorLeft - spinWheelString.Length, Console.CursorTop);
                        }

                        spinPosition = spinPosition == 3 ? 0 : spinPosition + 1;
                        try
                        {
                            await Task.Delay(100, cancellationToken);
                        }
                        catch (OperationCanceledException)
                        {
                            _logger.Debug("SpinWheel execution has been canceled.");
                        }
                    }
                    watch.Stop();
                    _logger.Debug($"SpinWheel spin time: {watch.ElapsedMilliseconds} ms.");
                }
                catch (Exception ex)
                {
                    // Catch exceptions here.
                    // See https://blogs.msdn.microsoft.com/ptorr/2014/12/10/async-exceptions-in-c/
                    _logger.ErrorException("Spinwheel is broken:", ex, ex.Message);
                }
                finally
                {
                    Console.CursorVisible = true;
                }
            }, cancellationToken);
        }

        public void IncreaseProcessedItemsCount()
        {
            lock (_itemCountLockObject)
            {
                _processedItemsCount++;
            }
        }

        private int GetProcessedItemsCount()
        {
            lock (_itemCountLockObject)
            {
                return _processedItemsCount;
            }
        }
    }
}