using Microsoft.EntityFrameworkCore;
using GoBest.Data;
using GoBest.Auth;
using GoBest.Util;
using System.Reflection;
using GoBest.Exceptions;
using GoBest.Routes;
using GoBest.Users;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var hashedPassword = BCrypt.Net.BCrypt.HashPassword("admin123");
Console.WriteLine(hashedPassword);

JWTConfig.ConfigureJWT(builder);
SwaggerConfig.ConfigureSwagger(builder);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddHttpClient<ApiService>(); 
builder.Services.AddHostedService<ServiceApiBackgroundJob>();

builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("CustomerOnly",
        p => p.RequireRole("Customer"));
    opts.AddPolicy("AdminOnly",
        p => p.RequireRole("Admin"));
    opts.AddPolicy("CompanyRepOnly",
        p => p.RequireRole("CompanyRep"));
    opts.AddPolicy("AdminAndCompanyRepOnly",
        p => p.RequireRole("Admin", "CompanyRep"));
    opts.AddPolicy("UserOnly",
        p => p.RequireRole("Customer", "Admin", "CompanyRep"));
});


builder.Services.AddServicesFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true; 
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
