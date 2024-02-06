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
                cmd.Parameters.AddWithValue("@EmployeeId", model.EmployeeID);
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
                return "Fail";

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
                        // If the request fails, throw an exception with the error message
                        throw new Exception("Fail");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Fail");
            }
        }
    }
}
