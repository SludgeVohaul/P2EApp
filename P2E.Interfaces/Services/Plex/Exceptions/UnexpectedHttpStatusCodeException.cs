using System;
using System.Net;
using RestSharp;

namespace P2E.Interfaces.Services.Plex.Exceptions
{
    public class UnexpectedHttpStatusCodeException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public long ResponseDuration { get; set; }
        public Method RequestMethod { get; set; }
        public Uri ResponseUri { get; set; }

        public UnexpectedHttpStatusCodeException(string message, HttpStatusCode statusCode, long responseDuration, Method requestMethod, Uri responseUri) : base(message)
        {
            StatusCode = statusCode;
            ResponseDuration = responseDuration;
            RequestMethod = requestMethod;
            ResponseUri = responseUri;
        }
    }
}