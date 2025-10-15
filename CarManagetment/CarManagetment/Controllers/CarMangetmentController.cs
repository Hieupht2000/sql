using CarManagetment.Data;
using CarManagetment.DTOs;
using CarManagetment.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarManagetment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarMangetmentController : ControllerBase
    {
        private readonly CarDBContext _context;
        public CarMangetmentController(CarDBContext context)
        {
            _context = context;
        }
        // GET: api/cars  
        [HttpGet]
        public ActionResult<IEnumerable<CarDTO>> GetCars()
        {
            var cars = _context.Car.ToList();
            var carDTOs = cars.Select(c => new CarDTO
            {
                CarId = c.CarId,
                CustomerId = c.CustomerId,
                FullName = c.FullName,
                LicensePlate = c.LicensePlate,
                Brand = c.Brand,
                Model = c.Model,
                Year = c.Year,
                Odometer = c.Odometer
            }).ToList();
            return Ok(carDTOs);
        }
        // GET: api/cars/5  
        [HttpGet("{id}")]
        public ActionResult<CarDTO> GetCar(int id)
        {
            var car = _context.Car.Find(id);
            if (car == null)
            {
                return NotFound();
            }
            var carDTO = new CarDTO
            {
                CarId = car.CarId,
                CustomerId = car.CustomerId,
                FullName = car.FullName,
                LicensePlate = car.LicensePlate,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                Odometer = car.Odometer
            };
            return Ok(carDTO);
        }
        // POST: api/cars  
        [HttpPost]
        public ActionResult<CarDTO> CreateCar(CarDTO carDTO)
        {
            if (carDTO == null)
            {
                return BadRequest("Car data is null.");
            }
            var car = new Car
            {
                CustomerId = carDTO.CustomerId,
                FullName = carDTO.FullName,
                LicensePlate = carDTO.LicensePlate,
                Brand = carDTO.Brand,
                Model = carDTO.Model,
                Year = carDTO.Year,
                Odometer = carDTO.Odometer
            };
            _context.Car.Add(car);
            _context.SaveChanges();
            carDTO.CarId = car.CarId; // Set the CarId after saving  
            return CreatedAtAction(nameof(GetCar), new { id = car.CarId }, carDTO);
        }

        // PUT: api/cars/5
        [HttpPut("{id}")]
        public ActionResult<CarDTO> UpdateCar(int id, CarDTO carDTO)
        {
            if (carDTO == null || carDTO.CarId != id)
            {
                return BadRequest("Car data is null or ID mismatch.");
            }
            var car = _context.Car.Find(id);
            if (car == null)
            {
                return NotFound();
            }
            car.CustomerId = carDTO.CustomerId;
            car.FullName = carDTO.FullName;
            car.LicensePlate = carDTO.LicensePlate;
            car.Brand = carDTO.Brand;
            car.Model = carDTO.Model;
            car.Year = carDTO.Year;
            car.Odometer = carDTO.Odometer;
            _context.Car.Update(car);
            _context.SaveChanges();
            return NoContent();
        }

        // DELETE: api/cars/5
        [HttpDelete("{id}")]
        public ActionResult DeleteCar(int id)
        {
            var car = _context.Car.Find(id);
            if (car == null)
            {
                return NotFound();
            }
            _context.Car.Remove(car);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
