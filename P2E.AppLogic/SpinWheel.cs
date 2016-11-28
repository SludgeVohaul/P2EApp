using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Logging;

namespace P2E.AppLogic
{
    internal class SpinWheel
    {
        private static readonly SemaphoreSlim SemSlim = new SemaphoreSlim(1, 1);

        private const string SpinDurationErrorValue = "---";

        private readonly ILogger _logger;
        private readonly string[] _spinPositions = { "/", "-", @"\", "|" };

        internal DateTime StartTime { get; private set;}
        internal DateTime StopTime { get; private set; }

        internal string SpinDuration => (StartTime == DateTime.MinValue || StopTime == DateTime.MinValue)
            ? SpinDurationErrorValue
            : (StopTime - StartTime).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);

        internal SpinWheel(ILogger logger)
        {
            _logger = logger;
        }

        internal async Task SpinAsync(CancellationToken cancellationToken)
        {
            var spinPosition = 0;

            if (SemSlim.CurrentCount == 0)
            {
                _logger.Debug("Wheel already spinning!");
                return;
            }

            await SemSlim.WaitAsync(cancellationToken);
            try
            {
                await Task.Run(() =>
                {
                    _logger.Debug("SemSlim enter.");
                    try
                    {
                        StartTime = DateTime.Now;
                        while (cancellationToken.IsCancellationRequested == false)
                        {
                            spinPosition++;
                            // TODO remove
                            //var bla = new DateTime(543435543, 1, 1, 1, 1, 1);
                            Console.Write(_spinPositions[spinPosition%4]);
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                            // Bypass Sleep() for better timing results.
                            if (cancellationToken.IsCancellationRequested) break;
                            Thread.Sleep(50);
                        }

                        StopTime = DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        // Catch exceptions here.
                        // See https://blogs.msdn.microsoft.com/ptorr/2014/12/10/async-exceptions-in-c/
                        // TODO - ex.Message is not being logged.
                        _logger.ErrorException("Spinwheel is broken", ex, ex.Message);
                        StartTime = DateTime.MinValue;
                        StopTime = DateTime.MinValue;
                    }
                }, cancellationToken);
            }
            finally
            {
                _logger.Debug($"SemSlim.Release()");
                SemSlim.Release();
            }
        }
    }
}