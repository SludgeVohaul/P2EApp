using System;
using System.Net;
using RestSharp;

namespace P2E.Interfaces.Services.Plex.Exceptions
{
    public class UnexpectedHttpStatusCodeException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public long ResponseDuration { get; }
        public Method RequestMethod { get; }
        public Uri ResponseUri { get; }

        public UnexpectedHttpStatusCodeException(string message, HttpStatusCode statusCode, long responseDuration, Method requestMethod, Uri responseUri) : base(message)
        {
            StatusCode = statusCode;
            ResponseDuration = responseDuration;
            RequestMethod = requestMethod;
            ResponseUri = responseUri;
        }
    }
}