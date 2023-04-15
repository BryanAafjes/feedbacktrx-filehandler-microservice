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
                //case NotFoundException _:
                //    response.StatusCode = (int)HttpStatusCode.NotFound;
                //    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await response.WriteAsync("An error occurred while processing your request.");
                    break;
            }
        }
    }
}
