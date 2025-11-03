using CarManagetment.Data;
using CarManagetment.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarManagetment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GaragesController : ControllerBase
    {
        private readonly CarDBContext _context;
        public GaragesController(CarDBContext context)
        {
            _context = context;
        }

        // GET: api/garages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Garages>>> GetGarages()
        {
            var garages = await _context.garages.ToListAsync();
            if (garages == null || !garages.Any())
            {
                return NotFound();
            }
            return Ok(garages);
        }

        //Get: api/garages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Garages>> GetGarage(int id)
        {
            var garage = await _context.garages.FindAsync(id);
            if (garage == null)
            {
                return NotFound("Garage Not Found");
            }
            return garage;
        }

        // POST: api/garages
        [HttpPost]
        public async Task<ActionResult<Garages>> PostGarage(Garages garage)
        {
            if (garage == null)
            {
                return BadRequest("Garage data is null.");
            }
            _context.garages.Add(garage);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetGarage), new { id = garage.GarageId }, garage);
        }

        // PUT: api/garages/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGarage(int id, Garages garage)
        {
            if (id != garage.GarageId)
            {
                return BadRequest("Garage ID mismatch.");
            }
            _context.Entry(garage).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GarageExists(id))
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

        // DELETE: api/garages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGarage(int id)
        {
            try
            {
                var garage = await _context.garages.FindAsync(id);
                if (garage == null)
                {
                    return NotFound();
                }
                _context.garages.Remove(garage);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

            return NoContent();
        }
        private bool GarageExists(int id)
        {
            return _context.garages.Any(e => e.GarageId == id);
        }
    }
}