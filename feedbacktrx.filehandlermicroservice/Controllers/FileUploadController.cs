using feedbacktrx.filehandlermicroservice.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace feedbacktrx.filehandlermicroservice.Controllers
{
    [Route("api/upload")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IFileUploadService _service;

        public FileUploadController(IFileUploadService service)
        {
            _service = service;
        }

        [HttpPost]
        [RequestSizeLimit(int.MaxValue)]
        public async Task<ActionResult> Upload(IFormFile file)
        {
            await _service.SaveFile(file);
            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
