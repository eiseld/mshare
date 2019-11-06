using System;

namespace MShare_ASP.Services.Exceptions
{
    /// <summary>Equivalent to HttpStatusCode.Conflict, 409</summary>
    public class BusinessException : Exception
    {
        internal BusinessException(string message = "") :
            base(message)
        {
        }
    }
}