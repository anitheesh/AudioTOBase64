using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AudioTOBase64.Repository
{
    public class Audio
    {
        private readonly HttpClient _httpClient;

        public Audio(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> PostToExternalApi(string base64String)
        {
            // Adjust the external API endpoint URL
            string externalApiUrl = "https://example.com/api/upload";

            // Encode the Base64 string for query parameter
            string encodedString = Uri.EscapeDataString(base64String);

            // Construct the JSON payload
            var payload = new
            {
                audio = encodedString
               
            };

            try
            {
                // Serialize the payload to JSON
                string jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);

                // Post the JSON data to the external API
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync(externalApiUrl, content);

                // Check if the request was successful
                response.EnsureSuccessStatusCode();

                // Read the response content
                string responseBody = await response.Content.ReadAsStringAsync();

                // Return the response
                return responseBody;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"External API request failed: {ex.Message}");
            }
        }
    }
}
