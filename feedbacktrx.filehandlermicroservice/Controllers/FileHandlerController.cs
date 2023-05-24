using feedbacktrx.filehandlermicroservice.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.IO;

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
        [RequestSizeLimit(int.MaxValue)]
        public async Task<ActionResult> Upload(IFormFile file)
        {
            Guid fileGuid = await _service.SaveFile(file);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet("{fileName}")]
        public IActionResult StreamAudio(string fileName)
        {
            var audioStream = _service.GetFileStream(fileName);
            var mimeType = _service.GetMimeType(fileName);

            HttpContext.Response.Headers.Add("Accept-Ranges", "bytes");
            HttpContext.Response.Headers.Add("Cache-Control", "public, max-age=3600");

            var rangeHeader = HttpContext.Request.Headers["Range"].ToString();
            var range = RangeHeaderValue.Parse(rangeHeader);

            long start = range.Ranges.FirstOrDefault()?.From ?? 0;
            long end = range.Ranges.FirstOrDefault()?.To ?? audioStream.Length - 1;
            long length = end - start + 1;

            HttpContext.Response.StatusCode = 206;
            HttpContext.Response.Headers.Add("Content-Length", length.ToString());
            HttpContext.Response.Headers.Add("Content-Range", $"bytes {start}-{end}/{audioStream.Length}");

            audioStream.Seek(start, SeekOrigin.Begin);
            return File(audioStream, mimeType);
        }
    }
}
