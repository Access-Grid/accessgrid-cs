using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AccessGrid
{
    /// <summary>
    /// Interface for HTTP client operations to enable mocking
    /// </summary>
    public interface IHttpClientWrapper : IDisposable
    {
        /// <summary>
        /// Base address for HTTP requests
        /// </summary>
        Uri BaseAddress { get; set; }

        /// <summary>
        /// Sends an HTTP request asynchronously
        /// </summary>
        /// <param name="request">The HTTP request message</param>
        /// <returns>The HTTP response message</returns>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
}