using CarManagetment.Data;
using CarManagetment.DTOs;
using CarManagetment.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarManagetment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TechnicalController : ControllerBase
    {
        private readonly CarDBContext _context;
        public TechnicalController(CarDBContext context)
        {
            _context = context;
        }

        // GET: api/technical
        [HttpGet]
        public ActionResult<IEnumerable<TechnicianDTO>> GetTechnicals()
        {
            var technicals = _context.technician.ToList();
            var technicalDTOs = technicals.Select(t => new TechnicianDTO
            {
                TechnicianId = t.TechnicianId,
                FullName = t.FullName,
                PhoneNumber = t.PhoneNumber,
                Email = t.Email,
                Status = t.Status
            }).ToList();
            return Ok(technicalDTOs);
        }

        // GET: api/technical/5
        [HttpGet("{id}")]
        public ActionResult<TechnicianDTO> GetTechnical(int id)
        {
            var technical = _context.technician.Find(id);
            if (technical == null)
            {
                return NotFound();
            }
            var technicalDTO = new TechnicianDTO
            {
                TechnicianId = technical.TechnicianId,
                FullName = technical.FullName,
                PhoneNumber = technical.PhoneNumber,
                Email = technical.Email,
                Status = technical.Status
            };
            return Ok(technicalDTO);
        }

        // POST: api/technical
        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult<TechnicianDTO> CreateTechnical(TechnicianDTO technicalDTO)
        {
            if (technicalDTO == null)
            {
                return BadRequest("Technical cannot be null");
            }
            var technical = new Technician
            {
                FullName = technicalDTO.FullName,
                PhoneNumber = technicalDTO.PhoneNumber,
                Email = technicalDTO.Email,
                Status = technicalDTO.Status
            };
            _context.technician.Add(technical);
            _context.SaveChanges();
            technicalDTO.TechnicianId = technical.TechnicianId; // Set the ID of the created entity
            return CreatedAtAction(nameof(GetTechnical), new { id = technical.TechnicianId }, technicalDTO);
        }

        // PUT: api/technical/5
        [HttpPut("{id}")]
        public IActionResult UpdateTechnical(int id, TechnicianDTO technicalDTO)
        {
            if (id != technicalDTO.TechnicianId)
            {
                return BadRequest("Technical ID mismatch");
            }
            var technical = _context.technician.Find(id);
            if (technical == null)
            {
                return NotFound();
            }
            technical.FullName = technicalDTO.FullName;
            technical.PhoneNumber = technicalDTO.PhoneNumber;
            technical.Email = technicalDTO.Email;
            technical.Status = technicalDTO.Status;
            //_context.Entry(technical).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        // DELETE: api/technical/5
        [HttpDelete("{id}")]
        public IActionResult DeleteTechnical(int id)
        {
            var technical = _context.technician.Find(id);
            if (technical == null)
            {
                return NotFound();
            }
            _context.technician.Remove(technical);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
