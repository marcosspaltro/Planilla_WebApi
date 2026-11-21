global using Planilla_WebApi.Services.UserService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Planilla_WebApi.Conexiones;
using Planilla_WebApi.Services;
using Swashbuckle.AspNetCore.Filters;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Configuración de Kestrel con puertos fijos
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5056); // HTTP
    options.ListenAnyIP(7056, listenOptions =>
    {
        listenOptions.UseHttps(); // HTTPS (requiere certificado)
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Add services to the container.

// Las sucursales solo pueden cargar o modificar datos del día (ver Services/SoloFechaDeHoy.cs)
// Sucursales y franquicias solo ven y cargan su propia sucursal (ver Services/RestriccionSucursal.cs)
builder.Services.AddControllers(options =>
{
    options.Filters.Add<Planilla_WebApi.Services.SoloFechaDeHoyFilter>();
    options.Filters.Add<Planilla_WebApi.Services.RestriccionSucursalFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// Toda la API exige un token válido, salvo lo marcado con [AllowAnonymous] (el login)
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Cadena de conexión de las clases de Conexiones (antes estaba escrita en cada archivo)
Conexion.Cadena = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";

// Agregar el DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Control de envios repetidos (Idempotency-Key): crea la tabla si no existe
try
{
    IdempotenciaMiddleware.CrearTabla(builder.Configuration.GetConnectionString("DefaultConnection") ?? "");
    IdempotenciaMiddleware.Habilitado = true;
}
catch (Exception e)
{
    app.Logger.LogError(e, "No se pudo crear la tabla IdempotenciaClaves; el control de repetidos queda desactivado");
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<IdempotenciaMiddleware>();

app.MapControllers();

app.Run();
