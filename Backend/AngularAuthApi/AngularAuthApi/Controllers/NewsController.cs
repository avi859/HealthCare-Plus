using AngularAuthApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AngularAuthApi.Controllers
{
    [ApiController]
    [Route("api/news")]
    public class NewsController : ControllerBase
    {
        private readonly NewsService _newsService;

        public NewsController(NewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet("health")]
        public async Task<IActionResult> GetHealthNews()
        {
            var news = await _newsService.GetHealthNewsAsync();
            return Ok(news);
        }
    }
}
