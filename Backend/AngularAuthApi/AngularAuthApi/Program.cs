using AngularAuthApi.Context;
using AngularAuthApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "wwwroot" // Ensure static files are correctly served
});

// Add services to the container.
builder.Services.AddControllers();

// Register HttpClient for NewsService
builder.Services.AddHttpClient<NewsService>();  // Fix here

builder.Services.AddScoped<NewsService>();  // No change here, correctly scoped

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnStr"));
});

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Middleware pipeline configuration
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI();

// Set up HTTPS redirection
app.UseHttpsRedirection();

// CORS, Authentication, and Authorization
app.UseCors("MyPolicy");
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Explicitly bind to port 8080 (matching Dockerfile)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://+:{port}");

// Run the application
app.Run();
