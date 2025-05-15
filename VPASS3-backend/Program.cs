using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json;
using VPASS3_backend.Context;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;
using VPASS3_backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Conexión a la base de datos
var connectionString = builder.Configuration.GetConnectionString("Connection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
);

//// Registro de Identity
//builder.Services.AddIdentity<User, IdentityRole>()
//    .AddEntityFrameworkStores<AppDbContext>()
//    .AddDefaultTokenProviders();

// Registro de Identity con el tipo de rol personalizado (Role)
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Configuración de la autenticación JWT
builder.Services.AddAuthentication(options =>
{
    // Se configura el esquema predeterminado de autenticación como JWT
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Desactivar la validación de HTTPS en el entorno local (para pruebas, en producción siempre debe estar en true)
    options.RequireHttpsMetadata = false;

    // Indicar que se debe guardar el token en la respuesta
    options.SaveToken = true;

    // Configuración de los parámetros de validación del token
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // No se valida el emisor del token (idealmente en producción se debería validar)
        ValidateIssuer = false,

        // No se valida el público (audience) del token (en producción, sí se debería hacer)
        ValidateAudience = false,

        // Se valida la fecha de expiración del token
        ValidateLifetime = true,

        // Se valida la clave con la que fue firmado el token
        ValidateIssuerSigningKey = true,

        // Se especifica la clave secreta utilizada para firmar el token (debe estar segura)
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // Aquí se toma la clave de la configuración
    };

    // Aquí se configuran los eventos relacionados con el proceso de autenticación
    options.Events = new JwtBearerEvents
    {
        // Este evento se ejecuta cuando el token no es válido o no está presente
        OnChallenge = async context =>
        {
            // Se maneja la respuesta y se impide que la respuesta predeterminada de 401 se envíe
            context.HandleResponse();

            // Se establece el código de estado HTTP como 401 (No autorizado)
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json"; // Se especifica el tipo de respuesta como JSON

            // Se crea una respuesta personalizada con un objeto ResponseDto para el mensaje de error
            var response = new ResponseDto(401, message: "No autorizado. El token es inválido o no fue proporcionado.");

            // Se definen opciones para que las propiedades se serialicen en camelCase
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Convierte propiedades a camelCase
            };

            // Se convierte el objeto a JSON utilizando las opciones definidas
            var json = JsonSerializer.Serialize(response, jsonOptions);
            await context.Response.WriteAsync(json); // Se envía la respuesta personalizada al cliente
        },

        // Este evento se ejecuta cuando se recibe una solicitud con un token, pero no tiene los permisos suficientes
        OnForbidden = async context =>
        {
            // Se establece el código de estado HTTP como 403 (Prohibido)
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json"; // Se especifica el tipo de respuesta como JSON

            // Se crea una respuesta personalizada con un objeto ResponseDto para el mensaje de error
            var response = new ResponseDto(403, message: "Acceso denegado. No tienes permiso para realizar esta acción.");

            // Se definen opciones para que las propiedades se serialicen en camelCase
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Convierte propiedades a camelCase
            };

            // Se convierte el objeto a JSON utilizando las opciones definidas
            var json = JsonSerializer.Serialize(response, jsonOptions);
            await context.Response.WriteAsync(json); // Se envía la respuesta personalizada al cliente
        }
    };

});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuración de Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "VPASS3",
        Version = "v1",
        Description = "Sistema de control de visitas",
        Contact = new OpenApiContact()
        {
            Name = "Gerardo Lucero Córdova",
            Email = "gerardoluceroc@gmail.com",
            Url = new Uri("https://github.com/gerardoluceroc")
        }
    });

    options.EnableAnnotations();

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme.",
        In = ParameterLocation.Header,
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
            },
            new string[]{}
        }
    });
});

//Configuracion de politica CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Cambia esto por la URL de tu React app
              .AllowAnyHeader()
              .AllowAnyMethod(); // Permite GET, POST, PUT, DELETE, etc.
    });
});

// Registrar el UserService para inyección de dependencias
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IEstablishmentService, EstablishmentService>();
builder.Services.AddScoped<IZoneService, ZoneService>();
builder.Services.AddScoped<IVisitorService, VisitorService>();
builder.Services.AddScoped<IVisitService, VisitService>();
builder.Services.AddScoped<IDirectionService, DirectionService>();
builder.Services.AddScoped<IZoneSectionService, ZoneSectionService>();
builder.Services.AddScoped<IParkingSpotService, ParkingSpotService>();
builder.Services.AddScoped<IVisitTypeService, VisitTypeService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();


// Aquí puedes agregar otros servicios si los tienes (como RoleService, etc.)


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ReadOnlyOwnProfile", policy =>
        policy.RequireRole("USER", "ADMIN", "SUPERADMIN"));

    options.AddPolicy("ManageOwnProfile", policy =>
        policy.RequireRole("ADMIN", "SUPERADMIN"));

    options.AddPolicy("ManageEverything", policy =>
        policy.RequireRole("SUPERADMIN"));
});





















var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CP2 API V1");
    c.RoutePrefix = string.Empty;
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Esto se encargará de atrapar cualquier excepción global que no se haya podido capturar dentro de un controlador.
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var response = new
        {
            StatusCode = 500,
            Message = "Ocurrió un error inesperado en el servidor."
        };

        await context.Response.WriteAsJsonAsync(response);
    });
});

app.UseCors("PermitirFrontend");

// Importante: primero autenticación, luego autorización
app.UseAuthentication();
//app.UseMiddleware<CustomUnauthorizedMiddleware>(); //Se agregan el middleware para las peticiones a rutas protegidas que se hagan sin token de autorización
app.UseAuthorization();

app.MapControllers();

app.Run();