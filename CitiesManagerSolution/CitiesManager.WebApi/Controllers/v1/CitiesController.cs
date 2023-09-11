using CitiesManager.Core.Entities;
using CitiesManager.Infrastructure.DatabaseContext;
using CitiesManager.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CitiesManager.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [TypeFilter(typeof(TokenValidatorActionFilter))]
    public class CitiesController : CustomControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cities
        /// <summary>
        /// Returns a list of Cities object that contains cityID and cityName
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Produces("application/xml")]
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            if (_context.Cities == null)
            {
                return NotFound();
            }
            return await _context.Cities.ToListAsync();
        }

        // GET: api/Cities/5
        [HttpGet("{cityID}")]
        public async Task<ActionResult<City>> GetCity(Guid cityID)
        {
            if (_context.Cities == null)
            {
                return NotFound();
            }
            var city = await _context.Cities.FindAsync(cityID);

            if (city == null)
            {
                return Problem(detail: "Invalid cityID", statusCode: 404, title: "City Search");
            }

            return city;
        }

        // PUT: api/Cities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{cityID}")]
        public async Task<IActionResult> PutCity(Guid cityID, City city)
        {
            if (cityID != city.CityID)
            {
                return BadRequest();
            }

            var existingCity = await _context.Cities.FindAsync(city.CityID);
            if (existingCity == null)
            {
                return NotFound();
            }

            existingCity.CityName = city.CityName;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(cityID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Cities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<City>> PostCity(City city)
        {
            if (_context.Cities == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Cities'  is null.");
            }
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCity", new { cityID = city.CityID }, city);
        }

        // DELETE: api/Cities/5
        [HttpDelete("{cityID}")]
        public async Task<IActionResult> DeleteCity(Guid cityID)
        {
            if (_context.Cities == null)
            {
                return NotFound();
            }
            var city = await _context.Cities.FindAsync(cityID);
            if (city == null)
            {
                return NotFound();
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(Guid cityID)
        {
            return (_context.Cities?.Any(e => e.CityID == cityID)).GetValueOrDefault();
        }
    }
}
