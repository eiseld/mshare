using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using Ex = MShare_ASP.Services.Exceptions;

namespace MShare_ASP.Middlewares
{

    /// <summary>All thrown (and uncaught) exceptions bubble here</summary>
    public class ErrorHandlingMiddleware
    {

        private readonly RequestDelegate next;

        /// <summary>Initializes the middleware, runs "before" requests, next ecapsulates the request</summary>
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>Invokes with the given context, this calls the custom exception handling logic</summary>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            } catch (Exception ex)
            {
                try
                {
                    await HandleExceptionAsync(context, ex);
                } catch (InvalidOperationException)
                {
                    Console.WriteLine("[ERROR]: Response has probably already started, make sure you don't throw exception inside a response. (e.g.: Don't use a lazy IEnumerable inside a returning Ok(...)!)");
                }
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var code = HttpStatusCode.InternalServerError;

            switch (ex)
            {
                case Ex.DatabaseException _: code = HttpStatusCode.InternalServerError; break;
                case Ex.BusinessException _: code = HttpStatusCode.Conflict; break;
                case Ex.ResourceGoneException _: code = HttpStatusCode.Gone; break;
                case Ex.ResourceForbiddenException _: code = HttpStatusCode.Forbidden; break;
                case Ex.ResourceNotFoundException _: code = HttpStatusCode.NotFound; break;
            }

            var errorMessage = new
            {
                errors = new Object[] { ex.Message }
#if DEBUG 
                ,
                stackTrace = ex.StackTrace.ToString(),
                innerException = ex.InnerException?.Message
#endif
            };
            var result = JsonConvert.SerializeObject(errorMessage);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
