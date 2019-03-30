using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Services {
    internal interface ITimeService {
        DateTime UtcNow { get; }
    }
}
