global using Planilla_WebApi.Services.UserService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
builder.Services.AddControllers(options => options.Filters.Add<Planilla_WebApi.Services.SoloFechaDeHoyFilter>());

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

// Errores no controlados: responder 500 desde acá (después de CORS) para que la web reciba
// el error con los encabezados CORS. Sin esto el navegador lo ve como "no se pudo llegar
// al servidor" y la carga sin conexión lo reintenta para siempre.
app.Use(async (ctx, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex) when (!ctx.Response.HasStarted)
    {
        app.Logger.LogError(ex, "Error no controlado en {Metodo} {Ruta}", ctx.Request.Method, ctx.Request.Path);
        ctx.Response.Clear();
        ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await ctx.Response.WriteAsync("Error interno de la API.");
    }
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<IdempotenciaMiddleware>();

app.MapControllers();

app.Run();
