using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Ex = MShare_ASP.Services.Exceptions;

namespace MShare_ASP.Middlewares {
    /// <summary>
    /// All thrown (and uncaught) exceptions bubble here
    /// </summary>
    public class ErrorHandlingMiddleware {
        private readonly RequestDelegate next;
        /// <summary>
        /// Initializes the middleware, runs "before" requests, next ecapsulates the request
        /// </summary>
        /// <param name="next"></param>
        public ErrorHandlingMiddleware(RequestDelegate next) {
            this.next = next;
        }

        /// <summary>
        /// Invokes with the given context, this calls the custom exception handling logic
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context) {
            try {
                await next(context);
            } catch (Exception ex) {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex) {
            var code = HttpStatusCode.InternalServerError;

            switch (ex) {
                case Ex.DatabaseException _: code = HttpStatusCode.InternalServerError; break;
                case Ex.BusinessException _: code = HttpStatusCode.Conflict; break;
                case Ex.ResourceGoneException _: code = HttpStatusCode.Gone; break;
                case Ex.ResourceForbiddenException _: code = HttpStatusCode.Forbidden; break;
            }

            var result = JsonConvert.SerializeObject(new { errors = new Object[] { ex.Message } });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
