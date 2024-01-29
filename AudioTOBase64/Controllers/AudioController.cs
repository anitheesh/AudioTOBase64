using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;

namespace AudioTOBase64.Controllers
{
    public class AudioController : Controller
    {
       
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> UploadAudioFile()
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

                    return Ok(base64String);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
       
    }
}
