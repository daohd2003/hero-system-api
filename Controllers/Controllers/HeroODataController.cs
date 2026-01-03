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

        // Cấu hình giới hạn OData để chống tấn công:
        // - MaxExpansionDepth: Giới hạn $expand lồng nhau (Hero -> Faction -> Heroes là hết)
        // - MaxAnyAllExpressionDepth: Giới hạn độ sâu any()/all() trong $filter
        // - MaxNodeCount: Giới hạn độ phức tạp của query tree
        // - MaxOrderByNodeCount: Giới hạn số cột trong $orderby
        // - MaxSkip: Tránh scan quá nhiều record
        [EnableQuery(
            MaxExpansionDepth = 3,
            MaxAnyAllExpressionDepth = 2,
            MaxNodeCount = 100,
            MaxOrderByNodeCount = 5,
            MaxSkip = 1000)]
        [HttpGet("odata/Heroes")]
        public IActionResult Get()
        {
            return Ok(_heroService.GetHeroesOData());
        }
    }
}
