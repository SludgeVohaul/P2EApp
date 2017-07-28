using System;
using RestSharp;

namespace P2E.Interfaces.Services.Plex.Exceptions
{
    public class ResponseFailureException : Exception
    {
        public Method RequestMethod { get; set; }
        public Uri RequestUri { get; set; }

        public ResponseFailureException(string message, Method requestMethod, Uri requestUri, Exception exception) : base(message, exception)
        {
            RequestMethod = requestMethod;
            RequestUri = requestUri;
        }
    }
}