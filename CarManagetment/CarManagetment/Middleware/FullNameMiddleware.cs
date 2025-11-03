using CarManagetment.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CarManagetment.Middleware
{
    public class FullNameMiddleware
    {
        private readonly RequestDelegate _next;
        public FullNameMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, CarDBContext dBContext)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var jwtHandler = new JwtSecurityTokenHandler();

                if (jwtHandler.CanReadToken(token))
                {
                    var jwtToken = jwtHandler.ReadJwtToken(token);
                    var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "id");
                    var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                    var customerIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "CustomerId");

                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    {
                        var user = await dBContext.Users
                            .FirstOrDefaultAsync(u => u.user_id == userId);

                        if (user != null)
                        {
                            context.Items["FullName"] = user.user_name;
                            context.Items["CustomerId"] = userIdClaim.Value; 
                            //context.Items["CustomerId"] = customerIdClaim?.Value; // Lưu CustomerId vào context
                            context.Items["Role"] = roleClaim?.Value; // Lưu Role vào context
                        }
                    }
                }
            }

            await _next(context);
        }
    }

}
