using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Calculator.WebApi
{
    [RoutePrefix("calculator")]
    public class CalculatorController : ApiController
    {
        [Route("{a}/plus/{b}")]
        [HttpGet]
        public async Task<IHttpActionResult> Plus(int a, int b)
        {


            return Ok(new {Result = 5});
        }
    }
}