using System;

namespace MShare_ASP.Services
{

    /// <summary>Time related services</summary>
    internal interface ITimeService
    {

        /// <summary>Gets current time</summary>
        DateTime UtcNow { get; }
    }
}
