using UserManagementApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

var app = builder.Build();

// Use the ErrorHandlingMiddleware
app.UseMiddleware<UserManagementApi.Middlewares.ErrorHandlingMiddleware>();

// Use the AuthenticationMiddleware
app.UseMiddleware<UserManagementApi.Middlewares.AuthenticationMiddleware>();

// Add the custom logging middleware
app.UseMiddleware<UserManagementApi.Middlewares.LoggingMiddleware>();

app.UseAuthorization();
app.MapControllers();

app.Run();