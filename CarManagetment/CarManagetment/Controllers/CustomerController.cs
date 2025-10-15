using CarManagetment.Data;
using CarManagetment.DTOs;
using CarManagetment.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarManagetment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CarDBContext _context;
        public CustomerController(CarDBContext context)
        {
            _context = context;
        }
        // GET: api/customerall
        [HttpGet]
        public ActionResult<IEnumerable<CustomerDTO>> GetCustomers()
        {
            var customers = _context.Customer.ToList();
            var customerDTOs = customers.Select(c => new CustomerDTO
            {
                CustomerId = c.CustomerId,
                FullName = c.FullName,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                CreatedAt = c.CreatedAt
            }).ToList();
            return Ok(customerDTOs);
        }
        // GET: api/customer/5
        [HttpGet("{id}")]
        public ActionResult<CustomerDTO> GetCustomer(int id)
        {
            try
            {
                var customer = _context.Customer.Find(id);
                if (customer == null)
                {
                    return NotFound();
                }
                var customerDTO = new CustomerDTO
                {
                    CustomerId = customer.CustomerId,
                    FullName = customer.FullName,
                    Email = customer.Email,
                    PhoneNumber = customer.PhoneNumber,
                    CreatedAt = customer.CreatedAt
                };
                if (id <= 0)
                {
                    return BadRequest("Invalid customer ID.");
                    
                }
                return Ok(customerDTO);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
           
            
        }
        // POST: api/customer
        [HttpPost]
        public ActionResult<CustomerDTO> CreateCustomer(CustomerDTO customerDTO)
        {
            if (customerDTO == null)
            {
                return BadRequest("Customer data is null.");
            }
            var customer = new Customer
            {
                FullName = customerDTO.FullName,
                Email = customerDTO.Email,
                PhoneNumber = customerDTO.PhoneNumber,
                CreatedAt = DateTime.Now
            };
            _context.Customer.Add(customer);
            _context.SaveChanges();
            customerDTO.CustomerId = customer.CustomerId;
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customerDTO);
        }

        // PUT: api/customer/5
        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(int id, CustomerDTO customerDTO)
        {
            if (id != customerDTO.CustomerId)
            {
                return BadRequest("Customer ID mismatch.");
            }
            var customer = _context.Customer.Find(id);
            if (customer == null)
            {
                return NotFound();
            }
            customer.FullName = customerDTO.FullName;
            customer.Email = customerDTO.Email;
            customer.PhoneNumber = customerDTO.PhoneNumber;
            _context.Customer.Update(customer);
            _context.SaveChanges();
            return NoContent();
        }

        // DELETE: api/customer/5
        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            var customer = _context.Customer.Find(id);
            if (customer == null)
            {
                return NotFound();
            }
            _context.Customer.Remove(customer);
            _context.SaveChanges();
            return NoContent();
        }

    }
}
