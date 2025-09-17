using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AccessGrid
{
    /// <summary>
    /// Default implementation of IHttpClientWrapper that wraps HttpClient
    /// </summary>
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Creates a new HttpClientWrapper with a new HttpClient instance
        /// </summary>
        public HttpClientWrapper() : this(new HttpClient())
        {
        }

        /// <summary>
        /// Creates a new HttpClientWrapper with the specified HttpClient
        /// </summary>
        /// <param name="httpClient">The HttpClient to wrap</param>
        public HttpClientWrapper(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Base address for HTTP requests
        /// </summary>
        public Uri BaseAddress
        {
            get => _httpClient.BaseAddress;
            set => _httpClient.BaseAddress = value;
        }

        /// <summary>
        /// Sends an HTTP request asynchronously
        /// </summary>
        /// <param name="request">The HTTP request message</param>
        /// <returns>The HTTP response message</returns>
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return await _httpClient.SendAsync(request);
        }

        /// <summary>
        /// Disposes the wrapped HttpClient
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}