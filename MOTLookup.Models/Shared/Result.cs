using System.Net;

namespace MOTLookup.Models.Shared
{
    public sealed class Result<T>
    {
        public bool IsSuccess { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }
    }

}
