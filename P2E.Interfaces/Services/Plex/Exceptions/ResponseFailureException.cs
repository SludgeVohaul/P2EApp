using System;
using RestSharp;

namespace P2E.Interfaces.Services.Plex.Exceptions
{
    public class ResponseFailureException : Exception
    {
        public Method RequestMethod { get; }
        public Uri RequestUri { get; }

        public ResponseFailureException(string message, Method requestMethod, Uri requestUri, Exception exception) : base(message, exception)
        {
            RequestMethod = requestMethod;
            RequestUri = requestUri;
        }
    }
}