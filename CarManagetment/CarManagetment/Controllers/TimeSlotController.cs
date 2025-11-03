using CarManagetment.Data;
using CarManagetment.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarManagetment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotController : ControllerBase
    {
        private readonly CarDBContext _context;
        public TimeSlotController(CarDBContext context)
        {
            _context = context;
        }

        // GET: api/timeslots
        [HttpGet]
        public ActionResult<IEnumerable<TimeSlot>> GetTimeSlots()
        {
            var timeSlots = _context.TimeSlot.ToList();
            return Ok(timeSlots);
        }

        // GET: api/timeslots/5
        [HttpGet("{id}")]
        public ActionResult<TimeSlot> GetTimeSlot(int id)
        {
            var timeSlot = _context.TimeSlot.Find(id);
            if (timeSlot == null)
            {
                return NotFound();
            }
            return Ok(timeSlot);
        }

        // POST: api/timeslots
        [HttpPost]
        public ActionResult<TimeSlot> CreateTimeSlot(TimeSlot timeSlot)
        {
            if (timeSlot == null)
            {
                return BadRequest("Time slot cannot be null");
            }
            _context.TimeSlot.Add(timeSlot);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetTimeSlot), new { id = timeSlot.TimeSlot_Id }, timeSlot);
        }

        // PUT: api/timeslots/5
        [HttpPut("{id}")]
        public IActionResult UpdateTimeSlot(int id, TimeSlot timeSlot)
        {
            if (id != timeSlot.TimeSlot_Id)
            {
                return BadRequest("Time slot ID mismatch");
            }
            _context.Entry(timeSlot).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.TimeSlot.Any(ts => ts.TimeSlot_Id == id))
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

        // DELETE: api/timeslots/5
        [HttpDelete("{id}")]
        public IActionResult DeleteTimeSlot(int id)
        {
            var timeSlot = _context.TimeSlot.Find(id);
            if (timeSlot == null)
            {
                return NotFound();
            }
            _context.TimeSlot.Remove(timeSlot);
            _context.SaveChanges();
            return NoContent();
        }
        // Helper method to check if a time slot exists
        private bool TimeSlotExists(int id)
        {
            return _context.TimeSlot.Any(ts => ts.TimeSlot_Id == id);
        }
    }
}
