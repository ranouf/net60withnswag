using System;
using System.Net;

namespace SK.Exceptions
{
    public class LocalException : Exception
    {
        public int StatusCode { get; set; }

        public LocalException(
            string message,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError
        ) : base(message)
        {
            StatusCode = (int)statusCode;
        }

        public LocalException(
            Exception ex,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError
        ) : base(ex.Message)
        {
            StatusCode = (int)statusCode;
        }
    }
}
