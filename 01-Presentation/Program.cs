using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NoPrumo.Application.Interfaces;
using NoPrumo.Application.Services;
using NoPrumo.Domain.Interfaces;
using NoPrumo.Infrastructure.Data;
using NoPrumo.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NoPrumo API",
        Version = "v1",
        Description = "Gestão de clientes e obras para pequenas empresas de construção."
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "NoPrumo API v1");
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();