using Microsoft.EntityFrameworkCore;
using GoBest.Data;
using GoBest.Auth;
using GoBest.Util;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

JWTConfig.ConfigureJWT(builder);
SwaggerConfig.ConfigureSwagger(builder);

builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddServicesFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
