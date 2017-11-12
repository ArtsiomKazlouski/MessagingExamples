using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EHR.Cds.Models;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;

namespace EHR.Cds.Host.Controllers
{
    [Route("cds-services")]
    public class CdsController : Controller
    {
        private readonly IEnumerable<ICdsService> _services;

        public CdsController(IEnumerable<ICdsService> services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// Получить список cds сервисов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IActionResult Discovery()
        {
            var services = new Dictionary<string, IEnumerable<Service>>
            {
                {"services", _services.Select(t => t.DiscoveryIdentifier)}
            };
            return Ok(services);
        }

        /// <summary>
        /// Обратиться к сервису
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("{hookId}")]
        public async Task<IActionResult> Process(string hookId,[FromBody]RequestMessage request)
        {
            
            var hook = _services.FirstOrDefault(t => t.DiscoveryIdentifier.Id.Equals(hookId));
            if (hook == null)
                return BadRequest(
                    $"Не найден список правил с указанным именем. Все описанные правила можно просмотреть по адресу {Url.Action("Discovery")}");

            Response.Headers.Add("Location", Url.Action("Discovery"));
            return Ok(new Dictionary<string, IEnumerable<Card>>
            {
                {"cards",  await hook.HandleAsync(request) }
            });
        }
    }
}