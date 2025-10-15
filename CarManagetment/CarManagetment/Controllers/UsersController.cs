using CarManagetment.Data;
using CarManagetment.DTOs;
using CarManagetment.Model;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarManagetment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly CarDBContext _context;

        public UsersController(CarDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            var userDTOs = users.Select(user => new UserDTO
            {
                user_id = user.user_id,
                user_name = user.user_name,
                email = user.email,
                Password_hash = user.Password_hash,
                Role = user.Role.ToString(),
            }).ToList();

            return Ok(userDTOs);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var userDTO = new UserDTO
            {
                user_id = user.user_id,
                user_name = user.user_name,
                email = user.email,
                Password_hash = user.Password_hash,
                Role = user.Role.ToString(),
            };
            return Ok(userDTO);
        }
        private string HashPassword(string password)
        {
            // Tạo salt
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Băm mật khẩu với PBKDF2
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Kết hợp salt và hash vào một chuỗi
            return $"{Convert.ToBase64String(salt)}:{hashedPassword}";
        }
        public enum Role
        {
            Admin,
            User,
            Technician,
            Customer
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> PosUser(UserDTO userdto)
        {
            if(!Enum.TryParse(userdto.Role,true, out Role role))
            {
                return BadRequest($"Invalid role specified: {userdto.Role}");
            }
            var user = new Users
            {
                user_name = userdto.user_name,
                email = userdto.email,
                Password_hash = HashPassword(userdto.Password_hash),
                Role = role.ToString() // Convert the enum to string for storage
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            userdto.user_id = user.user_id; // Set the user_id from the newly created user
            return CreatedAtAction(nameof(GetUser), new { id = user.user_id }, userdto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(s => s.user_id == id);
        }
    }
}
