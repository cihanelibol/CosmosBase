using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Net;

namespace CosmosBase
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate next;
        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                //logger.LogError(ex, "Error : ", ex.Message, ex.InnerException.Message);
                await HandleExceptionAsync(context, ex);
            }

        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var response = new ApiResponse
            {
                IsSuccessful = false,
                StatusCode = GetStatusCode(ex),
                Message = ex.Message,
                Error = ex.InnerException != null ? ex.InnerException.Message : null,
            };

            if (context.RequestServices.GetService(typeof(IWebHostEnvironment)) is IWebHostEnvironment env &&
                          env.IsDevelopment())
            {
                response.Error = ex.StackTrace;

            }

            var jsonResponse = JsonConvert.SerializeObject(response);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = response.StatusCode;

            return context.Response.WriteAsync(jsonResponse);

        }


        private static int GetStatusCode(Exception ex)
        {
            return ex switch
            {
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized, 
                KeyNotFoundException => (int)HttpStatusCode.NotFound,            
                ArgumentException => (int)HttpStatusCode.BadRequest,             
                InvalidOperationException => (int)HttpStatusCode.Conflict,       
                NotImplementedException => (int)HttpStatusCode.NotImplemented,   
                TimeoutException => (int)HttpStatusCode.RequestTimeout,          
                _ => (int)HttpStatusCode.InternalServerError

            };

        }
    }
}
