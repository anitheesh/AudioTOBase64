using AudioTOBase64.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AudioTOBase64.Controllers
{
    public class AudioController : Controller
    {
        private readonly HttpClient _httpClient;



        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UploadAudioFile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadAudioFile([FromForm] Models.Class model)
        {
            if (model == null || model.File == null || model.File.Length == 0)
            {
                return BadRequest("Invalid file");
            }

            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await model.File.CopyToAsync(memoryStream);

                    string base64String = Convert.ToBase64String(memoryStream.ToArray());
                    Audio ad = new Audio(_httpClient);
                    await ad.PostToExternalApi(base64String);

                    return View();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}