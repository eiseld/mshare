using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Services {
    internal class TimeService : ITimeService {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
