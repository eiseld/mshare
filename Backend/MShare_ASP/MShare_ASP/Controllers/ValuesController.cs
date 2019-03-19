using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MShare_ASP.Services;

namespace MShare_ASP.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : BaseController {

        public ValuesController(IMshareService siteService) :
            base(siteService) {
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get() {
            return Ok(Service.Test.Select(x => x.Name));
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id) {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value) {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id) {
        }
    }
}
