using Newtonsoft.Json;
using System.Net;

namespace feedbacktrx.filehandlermicroservice.Exceptions
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;

        public ExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            switch (ex)
            {
                case FileNullException _:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await response.WriteAsync("Bad request error");
                    break;

                case FileNotFoundException _:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    await response.WriteAsync("File not found!");
                    break;

                case FileNotCleanException _:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await response.WriteAsync("File not clean!");
                    break;

                case FileNameTooLongException _:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await response.WriteAsync("File name is too long!");
                    break;

                case WrongFileExtensionException _:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await response.WriteAsync("File extension is not allowed!");
                    break;

                //default:
                //    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //    await response.WriteAsync("An error occurred while processing your request");
                //    break;
            }
        }
    }
}
