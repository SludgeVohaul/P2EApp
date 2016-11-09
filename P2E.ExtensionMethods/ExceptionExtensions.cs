using System;

namespace P2E.ExtensionMethods
{
    public static class ExceptionExtensions
    {
        public static Exception GetInnermostException(this System.Exception ex)
        {
            while (ex.InnerException != null) ex = ex.InnerException;
            return ex;
        }
    }
}