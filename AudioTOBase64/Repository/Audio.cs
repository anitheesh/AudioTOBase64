// Audio class
using AudioTOBase64.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AudioTOBase64.Repository
{
    public class Audio
    {
        // Audio to base64
        public async Task<string> PostAudio(Class model)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await model.File.CopyToAsync(memoryStream);

                string base64String = Convert.ToBase64String(memoryStream.ToArray());
                return await PostToExternalApi(base64String, model);
            }
        }
        public string covertString;
        public async Task<string> PostToExternalApi(string base64String, Class model)
        {

            // Adjust the external API endpoint URL
            string externalApiUrl = "https://localhost:7189/api/Values";

            // Encode the Base64 string for query parameter
            string encodedString = Uri.EscapeDataString(base64String);

            // Construct the JSON payload
            var payload = new
            {
                model.EmployeeID,
                model.Email,
                File = encodedString, // Make sure this matches the property name expected by the server
            };

            try
            {
                // Serialize the payload to JSON
                string jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);

                // Make a POST request to the external API
                using (var httpClient = new HttpClient())
                {
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(externalApiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        covertString = encodedString;

                        var responseMessage = new
                        {
                            model.EmployeeID,
                            model.Email,
                            System.DateTime.Now,
                            encodedString,
                           
                        };
                        string json = System.Text.Json.JsonSerializer.Serialize(responseMessage);
                        return encodedString;
                    }
                    else
                    {
                        // If the request fails, throw an exception with the error message
                        throw new Exception($"External API request failed: {response.ReasonPhrase}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"External API request failed: {ex.Message}");
            }
        }
    }
}
