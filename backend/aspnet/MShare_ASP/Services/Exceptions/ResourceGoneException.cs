using System;

namespace MShare_ASP.Services.Exceptions
{

    /// <summary>Equivalent to HttpStatusCode.Gone, 410</summary>
    public class ResourceGoneException : Exception
    {
        internal ResourceGoneException(string message = "") :
            base(message)
        {
        }
    }
}
