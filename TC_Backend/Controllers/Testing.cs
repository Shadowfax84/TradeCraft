using Finance.Net.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TC_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class Testing : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "investor")]
        public async Task Run(IYahooFinanceService yahooService)
        {
            var profile = await yahooService.GetProfileAsync("BEL.NS");

            Console.WriteLine($"Address: {profile.Adress}");
            Console.WriteLine($"Sector: {profile.Sector}");
            Console.WriteLine($"Industry: {profile.Industry}");
            Console.WriteLine($"Description: {profile.Description}");
        }
    }
}
