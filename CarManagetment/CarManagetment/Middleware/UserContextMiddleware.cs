namespace CarManagetment.Middleware
{
    public class UserContextMiddleware
    {
        private readonly RequestDelegate _next;
        public UserContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the user is authenticated
            if (context.User.Identity?.IsAuthenticated == true)
            {
                // Extract user information from the claims
                var emailClaims = context.User.FindFirst("sub");
                if (emailClaims != null)
                {
                    // Set the UserId in the HttpContext.Items for later use
                    context.Items["Email"] = emailClaims.Value;
                }

            }
            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
