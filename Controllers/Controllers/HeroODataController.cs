using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services;

namespace Controllers.Controllers
{
    public class HeroODataController : ODataController
    {
        private readonly IHeroService _heroService;

        public HeroODataController(IHeroService heroService)
        {
            _heroService = heroService;
        }

        [EnableQuery]
        [HttpGet("odata/Heroes")]
        public IActionResult Get()
        {
            return Ok(_heroService.GetHeroesOData());
        }
    }
}
