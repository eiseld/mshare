using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MShare_ASP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Controllers {
    public class BaseController : ControllerBase {
        protected IMshareService Service { get; }

        public BaseController(IMshareService mshareService) {
            Service = mshareService;
        }
    }
}
