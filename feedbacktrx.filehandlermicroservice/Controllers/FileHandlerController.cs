using feedbacktrx.filehandlermicroservice.Service;
using Microsoft.AspNetCore.Mvc;

namespace feedbacktrx.filehandlermicroservice.Controllers
{
    [Route("")]
    [ApiController]
    public class FileHandlerController : ControllerBase
    {
        private readonly IFileHandlerService _service;

        public FileHandlerController(IFileHandlerService service)
        {
            _service = service;
        }

        [HttpPost]
        [RequestSizeLimit(52428800)]
        public async Task<ActionResult> Upload(IFormFile file)
        {
            string fileName = await _service.SaveFile(file);
            var response = new { filename = fileName };
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> StreamAudio(string fileName)
        {
            HttpContext.Response.Headers.Add("Accept-Ranges", "bytes");

            return await _service.GetFileStream(fileName);
        }
    }
}
