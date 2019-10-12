using System;

namespace MShare_ASP.Services
{
    internal class TimeService : ITimeService
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}