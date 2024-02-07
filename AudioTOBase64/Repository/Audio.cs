// Audio class
using AudioTOBase64.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
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


            string externalApiUrl = _configuration.GetSection("APIConnection")["URL"];
            string encodedString = Uri.EscapeDataString(base64String);
            var payload = new
            {
                model.EmployeeID,
                model.Email,
                File = encodedString, 
            };

            try
            {
               
                string jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
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
                        return "Fail";
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Fail");
            }
        }
        public string[] GetRandomPromptsFromDatabase()
        {
            Connect();

            string[] arrayvalue = new string[3];
            int i = 0;

            connection.Open();

            string query = "SELECT TOP (@Count) * FROM AudioPrompts WHERE IsActivePrompt = 1 ORDER BY NEWID()";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Count", 3);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        arrayvalue[i] = reader["Prompt"].ToString();
                        i++;
                    }
                }
            }

            connection.Close(); // Close the connection after usage

            return arrayvalue;
        }

    }
}
