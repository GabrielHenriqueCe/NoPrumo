using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NoPrumo.Application.Services;
using NoPrumo.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;

// Sem isso o ASP.NET renomeia a claim "sub" para uma URL gigante do WS-Federation.
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Libera o front (Vite, porta 5173) a chamar esta API.
builder.Services.AddCors(options =>
{
    options.AddPolicy("front", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Vem de User Secrets. Falhar aqui é melhor do que subir sem assinatura válida.
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Faltam as chaves Jwt:* em User Secrets.");

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<TokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Desliga a tradução de claims nos dois handlers, novo e antigo.
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
            // O padrão são 5 minutos de tolerância; 1 basta.
            ClockSkew = TimeSpan.FromMinutes(1),
        };
    });

builder.Services.AddAuthorization(options =>
{
    // Policy por permissão, não por nome de papel: mudar quem pode gerenciar
    // usuários é mexer no seed, não caçar `if (role == "admin")` no código.
    options.AddPolicy("manage_users", policy => policy.RequireClaim("permission", "manage_users"));

    // Estoque tem dois verbos. A tela inteira pede view_stock; lançar movimento
    // pede manage_stock. É assim que um perfil acompanha o estoque da obra sem
    // poder mexer nele — mesma tela, permissões diferentes.
    options.AddPolicy("view_stock", policy => policy.RequireClaim("permission", "view_stock"));
    options.AddPolicy("manage_stock", policy => policy.RequireClaim("permission", "manage_stock"));

    options.AddPolicy("manage_projects", policy => policy.RequireClaim("permission", "manage_projects"));
    options.AddPolicy("manage_employees", policy => policy.RequireClaim("permission", "manage_employees"));
    options.AddPolicy("manage_purchases", policy => policy.RequireClaim("permission", "manage_purchases"));
    options.AddPolicy("manage_safety", policy => policy.RequireClaim("permission", "manage_safety"));

    // Dinheiro é permissão à parte: ver contrato, salário, preço e margem.
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
        Description = "Gestão de clientes e obras para pequenas empresas de construção."
    });

    // Botão "Authorize" do Swagger, para testar endpoint protegido.
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

// Papéis, permissões e o primeiro admin. Roda a cada start e só insere o que falta.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DatabaseSeeder.SeedAsync(db);
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