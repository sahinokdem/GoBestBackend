using Microsoft.EntityFrameworkCore;
using GoBest.Data;
using GoBest.Auth;
using GoBest.Util;
using System.Reflection;
using GoBest.Exceptions;

var builder = WebApplication.CreateBuilder(args);

JWTConfig.ConfigureJWT(builder);
SwaggerConfig.ConfigureSwagger(builder);

builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddServicesFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register HTTP client
builder.Services.AddHttpClient("ServiceAPI", client =>
{
    client.BaseAddress = new Uri("https://your-api-endpoint/");
    // Add any headers, auth, etc. as needed
});

// Register repositories
builder.Services.AddScoped<GoBest.Companies.CompanyRepository>();
builder.Services.AddScoped<GoBest.Stations.StationRepository>();
builder.Services.AddScoped<GoBest.Services.ServiceRepository>();

// Register services
builder.Services.AddScoped<GoBest.Services.RouteFinderService>();
builder.Services.AddHostedService<GoBest.Services.ServiceApiService>();

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
