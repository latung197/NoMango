using Nomango.API.Models;
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
        [HttpGet]
        [Route("GetStudent/Get")]
        public IEnumerable <Employee> Get()
        {
            return Employee.Employees;
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
