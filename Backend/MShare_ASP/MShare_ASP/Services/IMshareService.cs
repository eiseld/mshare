using MShare_ASP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Services {
    public interface IMshareService {
        IEnumerable<Test> Test { get; }
    }
}
