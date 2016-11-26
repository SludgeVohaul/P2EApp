using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Logging;

namespace P2E.Services
{
    public class SpinWheel : IDisposable
    {
        private static readonly SemaphoreSlim SemSlim = new SemaphoreSlim(1, 1);

        private readonly ILogger _logger;
        private readonly string[] _spinPositions = { "/", "-", @"\", "|" };
        private bool _hasPendingCancellationRequest;

        private CancellationTokenSource _cancellationTokenSource;

        public SpinWheel(ILogger logger)
        {
            _logger = logger;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.Token.Register(SpinStop);
        }

        // TODO: check scenarios: logic calls service1.method1 which calls service2.method2
        public async Task SpinAsync([CallerFilePath]string callerFilePath = null, [CallerMemberName]string callerMemberName = "")
        {
            var spinPosition = 0;
#if DEBUG
            var startDateTime = DateTime.Now;
#endif
            await SemSlim.WaitAsync();
            try
            {
                var callerType = new StackFrame(1).GetMethod().DeclaringType;
                await Task.Run(() =>
                {
                    try
                    {
                        while (_hasPendingCancellationRequest == false)
                        {
                            spinPosition++;
                            Console.Write(_spinPositions[spinPosition % 4]);
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                            // Bypass Sleep() for better timing results.
                            if (_hasPendingCancellationRequest) break;
                            Thread.Sleep(100);
                        }
#if DEBUG
                        var millisecods = Math.Ceiling((DateTime.Now - startDateTime).TotalMilliseconds);
                        //var callerInfo = callerMemberType == null
                        //    ? $"{typeof(System.Runtime.CompilerServices.AsyncTaskMethodBuilder)}.{callerMemberName}"
                        //    : $"{callerMemberName}.{callerMemberName}";
                        //_logger.Debug($"Duration: {millisecods}ms ({callerInfo})");
                        // TODO ugly hack
                        _logger.Debug($"Duration: {millisecods}ms ({Path.GetFileName(callerFilePath).Replace(".cs","")}.{callerMemberName}");
#endif
                    }
                    catch (Exception ex)
                    {
                        // Catch exceptions here.
                        // See https://blogs.msdn.microsoft.com/ptorr/2014/12/10/async-exceptions-in-c/
                        _logger.ErrorException("Spinwheel is broken", ex, ex.Message);
                    }
                });
            }
            finally
            {
                SemSlim.Release();
            }
        }

        private void SpinStop()
        {
            _hasPendingCancellationRequest = true;
        }

        #region IDisposable
        // FYI: http://stackoverflow.com/questions/538060/proper-use-of-the-idisposable-interface
        ~SpinWheel()
        {
            // Instance is being finalized (i.e. destroyed) by the GC.
            // Call Dispose in case the user forgot to call IDisposable.Dispose
            Dispose(false); // Not called from IDisposable.Dispose, it's *not* safe
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool itIsSafeToDisposeManagedObjects)
        {
            // Free unmanaged resources

            // Free managed resources too, but only if being called from IDisposable.Dispose
            // (If being called from Finalize then the objects might not exist
            // anymore)
            if (itIsSafeToDisposeManagedObjects)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }
        #endregion
    }
}