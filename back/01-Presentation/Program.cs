using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NoPrumo.Application.Interfaces;
using NoPrumo.Application.Services;
using NoPrumo.Application.Settings;
using NoPrumo.Infrastructure.Data;
using NoPrumo.Infrastructure.Repositories;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower)));
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("front", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Faltam as chaves Jwt:* em User Secrets.");

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<TokenService>();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IStageRepository, StageRepository>();
builder.Services.AddScoped<IStageService, StageService>();

var encryptionKeyBase64 = builder.Configuration["Documents:EncryptionKey"]
    ?? throw new InvalidOperationException("Falta Documents:EncryptionKey nos User Secrets. Veja o README para gerar.");
var hmacKeyBase64 = builder.Configuration["Documents:HmacKey"]
    ?? throw new InvalidOperationException("Falta Documents:HmacKey nos User Secrets. Veja o README para gerar.");

var encryptionKey = Convert.FromBase64String(encryptionKeyBase64);
if (encryptionKey.Length != 32)
{
    throw new InvalidOperationException("Documents:EncryptionKey deve ter 32 bytes (256 bits).");
}

var hmacKey = Convert.FromBase64String(hmacKeyBase64);
if (hmacKey.Length != 32)
{
    throw new InvalidOperationException("Documents:HmacKey deve ter 32 bytes (256 bits).");
}

var documentSettings = new DocumentSettings
{
    EncryptionKey = encryptionKey,
    HmacKey = hmacKey,
};

builder.Services.AddSingleton(documentSettings);
builder.Services.AddSingleton<DocumentProcessor>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.FromMinutes(1),
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("manage_users", policy => policy.RequireClaim("permission", "manage_users"));
    options.AddPolicy("view_stock", policy => policy.RequireClaim("permission", "view_stock"));
    options.AddPolicy("manage_stock", policy => policy.RequireClaim("permission", "manage_stock"));
    options.AddPolicy("manage_projects", policy => policy.RequireClaim("permission", "manage_projects"));
    options.AddPolicy("manage_employees", policy => policy.RequireClaim("permission", "manage_employees"));
    options.AddPolicy("manage_purchases", policy => policy.RequireClaim("permission", "manage_purchases"));
    options.AddPolicy("manage_safety", policy => policy.RequireClaim("permission", "manage_safety"));
    options.AddPolicy("view_finance", policy => policy.RequireClaim("permission", "view_finance"));
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
      options.UseMySql(
          connectionString,
          new MySqlServerVersion(new Version(8, 0, 46)))
             .UseSnakeCaseNamingConvention());

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NoPrumo API",
        Version = "v1",
        Description = "Sistema de gestão de obras para construtoras de médio e grande porte."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Cole apenas o token, sem a palavra Bearer."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DatabaseSeeder.SeedAsync(appDbContext);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "NoPrumo API v1");
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("front");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
