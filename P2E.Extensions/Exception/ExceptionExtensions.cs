using System.Collections.Generic;

namespace P2E.Extensions.Exception
{
    public static class ExceptionExtensions
    {
        public static IEnumerable<System.Exception> GetInnerExceptions(this System.Exception ex)
        {
            if (ex == null) yield break;

            var innerException = ex;
            do
            {
                yield return innerException;
                innerException = innerException.InnerException;
            }
            while (innerException != null);
        }
    }
}