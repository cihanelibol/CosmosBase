using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CosmosBase
{
    public class RestRequest<TResponse>
    {
        private readonly HttpClient _httpClient;
        private string _authToken;

        public RestRequest(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Sets the authentication token to be included in the Authorization header.
        /// </summary>
        /// <param name="token">The authentication token (e.g., JWT).</param>
        public void SetAuthToken(string token)
        {
            _authToken = token;
            // Optionally, set the default Authorization header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
        }

        /// <summary>
        /// Adds the Authorization header to the request if the auth token is set.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        private void AddAuthorizationHeader(HttpRequestMessage request)
        {
            if (!string.IsNullOrEmpty(_authToken))
            {
                // Remove existing Authorization header if any
                if (request.Headers.Contains("Authorization"))
                {
                    request.Headers.Remove("Authorization");
                }
                // Add the Bearer token
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
            }
        }

        // GET Request
        public async Task<TResponse> GetAsync(string url)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            AddAuthorizationHeader(request);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error: {response.StatusCode}, {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // POST Request
        public async Task<TResponse> PostAsync<TRequest>(string url, TRequest requestData)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = jsonContent
            };
            AddAuthorizationHeader(request);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error: {response.StatusCode}, {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // PUT Request
        public async Task<TResponse> PutAsync<TRequest>(string url, TRequest requestData)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            using var request = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = jsonContent
            };
            AddAuthorizationHeader(request);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error: {response.StatusCode}, {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // DELETE Request
        public async Task<bool> DeleteAsync(string url)
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, url);
            AddAuthorizationHeader(request);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error: {response.StatusCode}, {response.ReasonPhrase}");
            }

            return response.IsSuccessStatusCode;
        }
    }
}