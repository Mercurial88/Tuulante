using System.IdentityModel.Tokens.Jwt;

namespace UserManagementApi.Middlewares
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip token validation for the login endpoint
            if (context.Request.Path.StartsWithSegments("/api/users/login"))
            {
                await _next(context);
                return;
            }

            // Retrieve the token from the Authorization header
            var token = context.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");

            // If token is missing or invalid, return 401 Unauthorized
            if (string.IsNullOrEmpty(token) || !ValidateToken(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Invalid or missing token.");
                return;
            }

            await _next(context); // Pass control to the next middleware if the token is valid
        }

        private bool ValidateToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();

                // Check if the token can be read
                if (!handler.CanReadToken(token))
                    return false;

                // Read and validate the token
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                // Example: Check expiry of the token
                return jwtToken != null && jwtToken.ValidTo > System.DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }
    }
}