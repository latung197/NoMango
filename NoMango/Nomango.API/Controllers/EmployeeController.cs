using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Nomango.API.Controllers
{
    public class EmployeeController : ApiController
    {
        [Route("Get")]
        public string Get()
        {
            return "Le Vanw Tung";
        }

        [HttpPost]
        [Route("PostStudent/{name}")] 
        public string PostStudent([FromUri] string name)
        {
            return name;
        }

        [Route("Post")]
        public string Post()
        {
            return "Le Van Tung";
        }
    }
}
