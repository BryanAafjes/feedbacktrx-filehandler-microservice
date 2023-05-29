using feedbacktrx.filehandlermicroservice.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace feedbacktrx.filehandlermicroservice.Controllers
{
    [Route("")]
    [ApiController]
    public class FileHandlerController : ControllerBase
    {
        private readonly IFileHandlerService _service;
        private ILogger _logger;

        public FileHandlerController(IFileHandlerService service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        [RequestSizeLimit(52428800)]
        public async Task<ActionResult> Upload(IFormFile file)
        {
            _logger.LogInformation("TEST IF IT EVEN REACHES HERE");
            string fileName = await _service.SaveFile(file);
            var response = new { filename = fileName };
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> StreamAudio(string fileName)
        {
            _logger.LogInformation("TEST IF IT EVEN REACHES HERE");
            HttpContext.Response.Headers.Add("Accept-Ranges", "bytes");

            return await _service.GetFileStream(fileName);
        }
    }
}
