using CitiesManager.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CitiesManager.WebApi.Controllers.v2
{
    [ApiVersion("2.0")]
    public class CitiesController : CustomControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cities
        /// <summary>
        /// Returns a list of Cities object that contains only cityName
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Produces("application/xml")]
        public async Task<ActionResult<IEnumerable<string?>>> GetCities()
        {
            if (_context.Cities == null)
            {
                return NotFound();
            }
            var cities = await _context.Cities
                .OrderBy(city => city.CityName)
                .Select(city => city.CityName)
                .ToListAsync();
            return cities;
        }

        
    }
}
