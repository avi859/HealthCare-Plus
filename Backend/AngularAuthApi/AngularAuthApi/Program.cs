using AngularAuthApi.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(option =>
{
option.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnStr"));
});

//still not done by doing this , we have only configured this yet we have registered it inside the pipeline,
//Always remember we have to put our corspipeline or middleware above Authentication and authorization
builder.Services.AddCors(options =>
{
  options.AddPolicy("MyPolicy", builder =>
  {
    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
  });
});


var app = builder.Build();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("MyPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
