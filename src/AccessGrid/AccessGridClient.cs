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
    public class AccessGridClient : IDisposable
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
            return await MakeRequestAsync<T>(new HttpMethod("PATCH"), endpoint, data);
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Generates HMAC signature for the payload according to the shared secret scheme.
        /// This exactly matches the Python implementation:
        ///
        /// def _generate_signature(self, payload: str) -> str:
        ///     encoded_payload = base64.b64encode(payload.encode())
        ///     signature = hmac.new(
        ///         self.secret_key.encode(), 
        ///         encoded_payload,
        ///         hashlib.sha256
        ///     ).hexdigest()
        ///     return signature
        /// </summary>
        private string GenerateSignature(string payload)
        {
            System.Console.WriteLine($"DEBUG: Generating signature for payload: {payload}");
            
            try
            {
                // STEP 1: Convert payload to UTF-8 bytes (payload.encode() in Python)
                byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
                System.Console.WriteLine($"DEBUG: Payload bytes: {BitConverter.ToString(payloadBytes)}");
                
                // STEP 2: Base64 encode those bytes (base64.b64encode() in Python)
                string base64String = Convert.ToBase64String(payloadBytes);
                System.Console.WriteLine($"DEBUG: Base64 encoded (string): {base64String}");
                
                // STEP 3: Create HMAC with the bytes of the base64 string
                byte[] base64Bytes = Encoding.ASCII.GetBytes(base64String);
                System.Console.WriteLine($"DEBUG: Base64 bytes for HMAC: {BitConverter.ToString(base64Bytes)}");
                
                // STEP 4: Create HMAC-SHA256 with UTF-8 bytes of the secret key
                byte[] keyBytes = Encoding.UTF8.GetBytes(_secretKey);
                System.Console.WriteLine($"DEBUG: Secret key bytes: {BitConverter.ToString(keyBytes)}");
                
                // STEP 5: Calculate HMAC
                using var hmac = new HMACSHA256(keyBytes);
                byte[] hashBytes = hmac.ComputeHash(base64Bytes);
                
                // STEP 6: Convert to lowercase hex string (hexdigest() in Python)
                string signature = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                
                System.Console.WriteLine($"DEBUG: Generated signature: {signature}");
                return signature;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"ERROR in signature generation: {ex.Message}");
                throw;
            }
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
                    string lastPart = parts[parts.Length - 1];
                    if (lastPart == "suspend" || lastPart == "resume" || lastPart == "unlink" || lastPart == "delete")
                    {
                        resourceId = parts[parts.Length - 2];
                    }
                    else
                    {
                        // Otherwise, the ID is typically the last part of the path
                        resourceId = lastPart;
                    }
                }
            }

            // Special handling for requests with no payload:
            // 1. POST requests with empty body (like unlink/suspend/resume)
            // 2. GET requests
            string payload;
            if ((method == HttpMethod.Post && data == null) || method == HttpMethod.Get)
            {
                // For listing cards endpoint (like what the Python list.py script does)
                if (method == HttpMethod.Get && endpoint == "/v1/key-cards")
                {
                    // IMPORTANT: Using exactly the same payload as Python with space after colon
                    payload = @"{""id"": ""key-cards""}";
                    System.Console.WriteLine($"DEBUG: Using key-cards payload for signature: {payload}");
                }
                // For other requests, use {"id": "card_id"} as the payload for signature generation
                else if (!string.IsNullOrEmpty(resourceId) && resourceId != "key-cards" && !resourceId.Contains("templates"))
                {
                    // Use verbatim string with space after colon to match Python exactly
                    payload = $@"{{""id"": ""{resourceId}""}}";
                    System.Console.WriteLine($"DEBUG: Using resource ID payload for signature: {payload}");
                }
                else
                {
                    payload = "{}";
                    System.Console.WriteLine($"DEBUG: Using empty payload for signature: {payload}");
                }
            }
            else
            {
                // For normal POST/PUT/PATCH with body, use the actual payload
                payload = data != null ? JsonSerializer.Serialize(data, _jsonOptions) : "";
                System.Console.WriteLine($"DEBUG: Using request body for signature: {payload}");
            }

            // Prepare query parameters before generating signature
            var finalQueryParams = queryParams != null 
                ? new Dictionary<string, string>(queryParams) 
                : new Dictionary<string, string>();
            
            // Add sig_payload for the listing endpoint specifically
            if (method == HttpMethod.Get && endpoint == "/v1/key-cards")
            {
                finalQueryParams["sig_payload"] = payload;
                System.Console.WriteLine($"DEBUG: Adding sig_payload for key-cards: {payload}");
            }
                
            // Generate signature
            string signature = GenerateSignature(payload);
            
            // For GET requests or POST requests with empty bodies that need the sig_payload parameter
            // Note: We've already added sig_payload for /v1/key-cards endpoint above
            if ((method == HttpMethod.Get || (method == HttpMethod.Post && data == null)) && !finalQueryParams.ContainsKey("sig_payload"))
            {
                if (!string.IsNullOrEmpty(resourceId) && resourceId != "key-cards" && !resourceId.Contains("templates"))
                {
                    // For resources that require an ID signature
                    string idPayload = $@"{{""id"": ""{resourceId}""}}";
                    finalQueryParams["sig_payload"] = idPayload;
                    System.Console.WriteLine($"DEBUG: Adding sig_payload: {idPayload}");
                }
            }

            // Build the URL with query parameters
            var requestUri = endpoint;
            if (finalQueryParams.Count > 0)
            {
                var queryString = string.Join("&", finalQueryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
                System.Console.WriteLine($"DEBUG: Query string: {queryString}");
                requestUri += (requestUri.Contains("?") ? "&" : "?") + queryString;
            }

            // Create request message
            var request = new HttpRequestMessage(method, requestUri);
            
            // Add headers
            request.Headers.Add("X-ACCT-ID", _accountId);
            request.Headers.Add("X-PAYLOAD-SIG", signature);
            request.Headers.Add("User-Agent", $"accessgrid.cs/{Version}");
            
            System.Console.WriteLine($"DEBUG: Request URL: {_httpClient.BaseAddress}{requestUri}");
            System.Console.WriteLine($"DEBUG: X-ACCT-ID: {_accountId}");
            System.Console.WriteLine($"DEBUG: X-PAYLOAD-SIG: {signature}");
            System.Console.WriteLine($"DEBUG: Payload for signature: {payload}");

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

            System.Console.WriteLine($"DEBUG: Response status: {(int)response.StatusCode} {response.StatusCode}");
            System.Console.WriteLine($"DEBUG: Response content: {responseContent}");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new AuthenticationException($"Invalid credentials: {responseContent}");
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

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}