using CarManagetment.Data;
using CarManagetment.Helpers;
using CarManagetment.Model;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarManagetment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginController : ControllerBase
    {
        private readonly CarDBContext _context;

        private readonly string _secretKey = "this_iss_my@_super_secret_key_which_iss_verys_longs_V2Wz1XeFJRspacnet0kzX1hcmo8Um7Vu"; // Replace with your actual salt value
        private readonly string _issuer = "http://localhost:7225/"; // Replace with your actual issuer
        private readonly string _audience = "http://localhost:7225/"; // Replace with your actual audience
        private readonly JwtTokenHelper _jwtTokenHelper;
        public UserLoginController(CarDBContext context)
        {
            _context = context;
        }


        [HttpPost("login")]
        public async Task<ActionResult<Users>> Login(UserLogin userLoginDTO)
        {
            var user = _context.Users.FirstOrDefault(u => u.email == userLoginDTO.Email);
            if (user == null || !VerifyPassword(user.Password_hash, userLoginDTO.Password))
            {
                return Unauthorized("Invalid credentials");
            }
            //Get customerId if user is customer
            var customer = await _context.Customer.FirstOrDefaultAsync(c => c.CustomerId == user.user_id);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.email),
                new Claim("userId", user.user_id.ToString()),
                //new Claim("CustomerId",customer.CustomerId.ToString()),
                //new Claim("FullName", customer.FullName ?? "") // Thêm FullName vào token
                new Claim(ClaimTypes.Role,user.Role.ToString())
            };


            var token = GenerateJwtToken(user.email, user.Role, user.user_id);
            return Ok(new { Token = token });
        }

        // Password hashing function
        public static string HashPassword(string password)
        {
            // Create salt
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            // Hash password with PBKDF2
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            // Combine salt and hash
            return $"{Convert.ToBase64String(salt)}:{hashed}";
        }

        public static bool VerifyPassword(string hashedPasswordWithSalt, string password)
        {
            // Extract salt and hash from stored string
            var parts = hashedPasswordWithSalt.Split(':');
            if (parts.Length != 2)
            {
                return false;
            }
            var salt = Convert.FromBase64String(parts[0]);
            var hashedPassword = parts[1];

            // Hash the entered password with the stored salt
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Compare the hash of the entered password with the stored hash
            return hashed == hashedPassword;
        }


        private string GenerateJwtToken(string Fullname, string role, int userId)
        {
            var claims = new[]
            {
                new Claim("userId", userId.ToString()),
                //new Claim("customerId", customerId.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, Fullname),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
