using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccessGrid
{
    /// <summary>
    /// Interface for API service operations to enable mocking and testing
    /// </summary>
    public interface IApiService
    {
        /// <summary>
        /// Makes a GET request to the API
        /// </summary>
        /// <typeparam name="T">Type to deserialize response to</typeparam>
        /// <param name="endpoint">API endpoint</param>
        /// <param name="queryParams">Optional query parameters</param>
        /// <returns>Deserialized response</returns>
        Task<T> GetAsync<T>(string endpoint, Dictionary<string, string> queryParams = null);

        /// <summary>
        /// Makes a POST request to the API
        /// </summary>
        /// <typeparam name="T">Type to deserialize response to</typeparam>
        /// <param name="endpoint">API endpoint</param>
        /// <param name="data">Data to send in request body</param>
        /// <returns>Deserialized response</returns>
        Task<T> PostAsync<T>(string endpoint, object data);

        /// <summary>
        /// Makes a PUT request to the API
        /// </summary>
        /// <typeparam name="T">Type to deserialize response to</typeparam>
        /// <param name="endpoint">API endpoint</param>
        /// <param name="data">Data to send in request body</param>
        /// <returns>Deserialized response</returns>
        Task<T> PutAsync<T>(string endpoint, object data);

        /// <summary>
        /// Makes a PATCH request to the API
        /// </summary>
        /// <typeparam name="T">Type to deserialize response to</typeparam>
        /// <param name="endpoint">API endpoint</param>
        /// <param name="data">Data to send in request body</param>
        /// <returns>Deserialized response</returns>
        Task<T> PatchAsync<T>(string endpoint, object data);
    }
}