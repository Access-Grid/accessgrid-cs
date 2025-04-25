using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AccessGrid
{
    /// <summary>
    /// Main client for interacting with the AccessGrid API
    /// </summary>
    public class AccessGridClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _accountId;
        private readonly string _secretKey;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string Version = "1.0.0";

        /// <summary>
        /// Service for managing access cards
        /// </summary>
        public AccessCardsService AccessCards { get; }

        /// <summary>
        /// Service for managing console operations (enterprise features)
        /// </summary>
        public ConsoleService Console { get; }

        /// <summary>
        /// Creates a new AccessGrid client
        /// </summary>
        /// <param name="accountId">Your AccessGrid account ID</param>
        /// <param name="secretKey">Your AccessGrid secret key</param>
        /// <param name="baseUrl">API base URL (defaults to https://api.accessgrid.com)</param>
        /// <exception cref="ArgumentException">Thrown when account ID or secret key is not provided</exception>
        public AccessGridClient(string accountId, string secretKey, string baseUrl = "https://api.accessgrid.com")
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentException("Account ID is required", nameof(accountId));
            
            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentException("Secret Key is required", nameof(secretKey));

            _accountId = accountId;
            _secretKey = secretKey;
            
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl.TrimEnd('/'))
            };

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            // Initialize services
            AccessCards = new AccessCardsService(this);
            Console = new ConsoleService(this);
        }

        #region HTTP Methods
        /// <summary>
        /// Makes a GET request to the API
        /// </summary>
        internal async Task<T> GetAsync<T>(string endpoint, Dictionary<string, string> queryParams = null)
        {
            return await MakeRequestAsync<T>(HttpMethod.Get, endpoint, null, queryParams);
        }

        /// <summary>
        /// Makes a POST request to the API
        /// </summary>
        internal async Task<T> PostAsync<T>(string endpoint, object data)
        {
            return await MakeRequestAsync<T>(HttpMethod.Post, endpoint, data);
        }

        /// <summary>
        /// Makes a PUT request to the API
        /// </summary>
        internal async Task<T> PutAsync<T>(string endpoint, object data)
        {
            return await MakeRequestAsync<T>(HttpMethod.Put, endpoint, data);
        }

        /// <summary>
        /// Makes a PATCH request to the API
        /// </summary>
        internal async Task<T> PatchAsync<T>(string endpoint, object data)
        {
            return await MakeRequestAsync<T>(HttpMethod.Patch, endpoint, data);
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Generates HMAC signature for the payload according to the shared secret scheme:
        /// SHA256.update(shared_secret + base64.encode(payload)).hexdigest()
        /// </summary>
        private string GenerateSignature(string payload)
        {
            // Base64 encode the payload
            var encodedPayload = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
            
            // Create HMAC using the shared secret as the key and the base64 encoded payload as the message
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(encodedPayload));
            
            // Convert hash to hexadecimal string
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Makes an HTTP request to the API with proper authentication
        /// </summary>
        private async Task<T> MakeRequestAsync<T>(HttpMethod method, string endpoint, object data = null, Dictionary<string, string> queryParams = null)
        {
            // Extract resource ID from the endpoint if needed for signature
            string resourceId = null;
            if (method == HttpMethod.Get || (method == HttpMethod.Post && data == null))
            {
                // Extract the ID from the endpoint - patterns like /resource/{id} or /resource/{id}/action
                var parts = endpoint.Trim('/').Split('/');
                if (parts.Length >= 2)
                {
                    // For actions like unlink/suspend/resume, get the card ID (second to last part)
                    if (parts[^1] == "suspend" || parts[^1] == "resume" || parts[^1] == "unlink" || parts[^1] == "delete")
                    {
                        resourceId = parts[^2];
                    }
                    else
                    {
                        // Otherwise, the ID is typically the last part of the path
                        resourceId = parts[^1];
                    }
                }
            }

            // Special handling for requests with no payload:
            // 1. POST requests with empty body (like unlink/suspend/resume)
            // 2. GET requests
            string payload;
            if ((method == HttpMethod.Post && data == null) || method == HttpMethod.Get)
            {
                // For these requests, use {"id": "card_id"} as the payload for signature generation
                if (!string.IsNullOrEmpty(resourceId) && resourceId != "key-cards" && !resourceId.Contains("templates"))
                {
                    payload = JsonSerializer.Serialize(new { id = resourceId });
                }
                else
                {
                    payload = "{}";
                }
            }
            else
            {
                // For normal POST/PUT/PATCH with body, use the actual payload
                payload = data != null ? JsonSerializer.Serialize(data, _jsonOptions) : "";
            }

            // Generate signature
            var signature = GenerateSignature(payload);

            // Prepare query parameters
            var finalQueryParams = queryParams ?? new Dictionary<string, string>();
            
            // For GET requests or POST requests with empty bodies, we need to include the sig_payload parameter
            if ((method == HttpMethod.Get || (method == HttpMethod.Post && data == null)) && 
                !string.IsNullOrEmpty(resourceId) && 
                resourceId != "key-cards" && 
                !resourceId.Contains("templates"))
            {
                finalQueryParams["sig_payload"] = JsonSerializer.Serialize(new { id = resourceId });
            }

            // Build the URL with query parameters
            var requestUri = endpoint;
            if (finalQueryParams.Count > 0)
            {
                var queryString = string.Join("&", finalQueryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
                requestUri += (requestUri.Contains("?") ? "&" : "?") + queryString;
            }

            // Create request message
            var request = new HttpRequestMessage(method, requestUri);
            
            // Add headers
            request.Headers.Add("X-ACCT-ID", _accountId);
            request.Headers.Add("X-PAYLOAD-SIG", signature);
            request.Headers.Add("User-Agent", $"accessgrid.cs @ v{Version}");

            // Add content if needed
            if (data != null && method != HttpMethod.Get)
            {
                var content = new StringContent(JsonSerializer.Serialize(data, _jsonOptions), Encoding.UTF8, "application/json");
                request.Content = content;
            }

            // Send request
            var response = await _httpClient.SendAsync(request);

            // Process response
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new AuthenticationException("Invalid credentials");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.PaymentRequired)
            {
                throw new AccessGridException("Insufficient account balance");
            }
            else if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var errorData = !string.IsNullOrEmpty(responseContent) 
                        ? JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent, _jsonOptions) 
                        : null;
                    
                    var errorMessage = errorData?.ContainsKey("message") == true 
                        ? errorData["message"]?.ToString() 
                        : responseContent;
                    
                    throw new AccessGridException($"API request failed: {errorMessage}");
                }
                catch (JsonException)
                {
                    throw new AccessGridException($"API request failed: {responseContent}");
                }
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(object)responseContent;
            }

            // Deserialize the response
            try
            {
                return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
            }
            catch (JsonException ex)
            {
                throw new AccessGridException($"Failed to deserialize response: {ex.Message}", ex);
            }
        }
        #endregion
    }
}