// Audio class
using AudioTOBase64.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AudioTOBase64.Repository
{
    public class Audio
    {
        // Audio to base64
        private readonly IConfiguration _configuration;
        private SqlConnection connection;

        public Audio(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Connect()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            connection = new SqlConnection(connectionString);
        }
        public async Task<string> PostAudio(Class model)
        {

            string audioString = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await model.File.CopyToAsync(memoryStream);

                string base64String = Convert.ToBase64String(memoryStream.ToArray());
                Connect();
                SqlCommand cmd = new SqlCommand("SelectEmp", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@email", model.Email);
                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            audioString = reader.GetString(0);
                        }
                    }
                    
                }
                if(audioString != null) {
                    string checkstring = audioString.Substring(0, 3);

                    if (checkstring == base64String.Substring(0, 3))
                    {
                        return await PostToExternalApi(base64String, model);
                    }

                    
                }
                else
                {
                    SqlCommand cmdinsert = new SqlCommand("insertvalue", connection);
                    cmdinsert.CommandType = CommandType.StoredProcedure;
                    cmdinsert.Parameters.AddWithValue("@email", model.Email);
                    cmdinsert.Parameters.AddWithValue("@Employeeid", model.EmployeeID);
                    cmdinsert.Parameters.AddWithValue("@audio", base64String);
                    int result = cmdinsert.ExecuteNonQuery();
                    return await PostToExternalApi(base64String, model);

                }
                connection.Close();
                return "An error occurred while posting to the external API.";

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
                        
                        return "Success";
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
