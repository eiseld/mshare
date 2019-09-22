using System;

namespace MShare_ASP.Services.Exceptions
{

    /// <summary>Equivalent to HttpStatusCode.NotFound, 404</summary>
    public class ResourceNotFoundException : Exception
    {
        internal ResourceNotFoundException(string message = "") :
            base(message)
        {
        }
    }
}
