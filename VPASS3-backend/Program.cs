using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json;
using VPASS3_backend.Context;
using VPASS3_backend.Data;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Interfaces.CommonAreaInterfaces;
using VPASS3_backend.Models;
using VPASS3_backend.Services;
using VPASS3_backend.Services.CommonAreaServices;

var builder = WebApplication.CreateBuilder(args);

// Conexi�n a la base de datos
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

// Configuraci�n de la autenticaci�n JWT
builder.Services.AddAuthentication(options =>
{
    // Se configura el esquema predeterminado de autenticaci�n como JWT
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Desactivar la validaci�n de HTTPS en el entorno local (para pruebas, en producci�n siempre debe estar en true)
    options.RequireHttpsMetadata = false;

    // Indicar que se debe guardar el token en la respuesta
    options.SaveToken = true;

    // Configuraci�n de los par�metros de validaci�n del token
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // No se valida el emisor del token (idealmente en producci�n se deber�a validar)
        ValidateIssuer = false,

        // No se valida el p�blico (audience) del token (en producci�n, s� se deber�a hacer)
        ValidateAudience = false,

        // Se valida la fecha de expiraci�n del token
        ValidateLifetime = true,

        // Se valida la clave con la que fue firmado el token
        ValidateIssuerSigningKey = true,

        // Se especifica la clave secreta utilizada para firmar el token (debe estar segura)
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // Aqu� se toma la clave de la configuraci�n
    };

    // Aqu� se configuran los eventos relacionados con el proceso de autenticaci�n
    options.Events = new JwtBearerEvents
    {
        // Este evento se ejecuta cuando el token no es v�lido o no est� presente
        OnChallenge = async context =>
        {
            // Se maneja la respuesta y se impide que la respuesta predeterminada de 401 se env�e
            context.HandleResponse();

            // Se establece el c�digo de estado HTTP como 401 (No autorizado)
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json"; // Se especifica el tipo de respuesta como JSON

            // Se crea una respuesta personalizada con un objeto ResponseDto para el mensaje de error
            var response = new ResponseDto(401, message: "No autorizado. El token es inv�lido o no fue proporcionado.");

            // Se definen opciones para que las propiedades se serialicen en camelCase
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Convierte propiedades a camelCase
            };

            // Se convierte el objeto a JSON utilizando las opciones definidas
            var json = JsonSerializer.Serialize(response, jsonOptions);
            await context.Response.WriteAsync(json); // Se env�a la respuesta personalizada al cliente
        },

        // Este evento se ejecuta cuando se recibe una solicitud con un token, pero no tiene los permisos suficientes
        OnForbidden = async context =>
        {
            // Se establece el c�digo de estado HTTP como 403 (Prohibido)
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json"; // Se especifica el tipo de respuesta como JSON

            // Se crea una respuesta personalizada con un objeto ResponseDto para el mensaje de error
            var response = new ResponseDto(403, message: "Acceso denegado. No tienes permiso para realizar esta acci�n.");

            // Se definen opciones para que las propiedades se serialicen en camelCase
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Convierte propiedades a camelCase
            };

            // Se convierte el objeto a JSON utilizando las opciones definidas
            var json = JsonSerializer.Serialize(response, jsonOptions);
            await context.Response.WriteAsync(json); // Se env�a la respuesta personalizada al cliente
        }
    };

});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuraci�n de Swagger
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

// Se obtiene la lista de orígenes permitidos desde la configuración (ahora leerá de launchSettings.json).
var allowedOriginsString = builder.Configuration["CORS_ALLOWED_ORIGINS"];
var allowedOrigins = allowedOriginsString?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(o => o.Trim())
                                        .ToArray();

// Configuracion de politica CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
    {
        if (allowedOrigins != null && allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            // Opcional: Si no se configuran orígenes, se podría optar por permitir ninguno o loguear una advertencia.
            // Para producción, NUNCA se debe usar AllowAnyOrigin() sin control.
            // En desarrollo, a veces se usa para depuración temporal.
            // policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); // ¡Precaución: No para producción!
            Console.WriteLine("Advertencia: No se han configurado orígenes CORS permitidos.");
        }
    });
});

// Registrar el UserService para inyecci�n de dependencias
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IEstablishmentService, EstablishmentService>();
builder.Services.AddScoped<IZoneService, ZoneService>();
builder.Services.AddScoped<IVisitService, VisitService>();
builder.Services.AddScoped<IDirectionService, DirectionService>();
builder.Services.AddScoped<IApartmentService, ApartmentService>();
builder.Services.AddScoped<IParkingSpotService, ParkingSpotService>();
builder.Services.AddScoped<IVisitTypeService, VisitTypeService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IBlacklistService, BlacklistService>();
builder.Services.AddScoped<IParkingSpotUsageLogService, ParkingSpotUsageLogService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<ICommonAreaService, CommonAreaService>();
builder.Services.AddScoped<ICommonAreaReservationService, CommonAreaReservationService>();
builder.Services.AddScoped<ICommonAreaUsageLogService, CommonAreaUsageLogService>();
builder.Services.AddScoped<IApartmentOwnershipService, ApartmentOwnershipService>();
builder.Services.AddScoped<IPackageService, PackageService>();

// Aqu� puedes agregar otros servicios si los tienes (como RoleService, etc.)
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

// Esto se encargar� de atrapar cualquier excepci�n global que no se haya podido capturar dentro de un controlador.
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var response = new
        {
            StatusCode = 500,
            Message = "Ocurri� un error inesperado en el servidor."
        };

        await context.Response.WriteAsJsonAsync(response);
    });
});


// --- INICIO DEL BLOQUE DE SIEMBRA DE DATOS ESENCIALES ---
// Se ejecuta siempre, para datos b�sicos de la aplicaci�n (roles, superadmin, direcciones).
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        Console.WriteLine("Iniciando proceso de siembra de datos esenciales...");
        await SeedData.Initialize(services);
        Console.WriteLine("Proceso de siembra de datos esenciales finalizado.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurri� un error al sembrar la base de datos con datos esenciales.");
        // Se considera relanzar la excepci�n para detener el inicio de la aplicaci�n si los datos esenciales son cr�ticos.
        // throw;
    }
}
// --- FIN DEL BLOQUE DE SIEMBRA DE DATOS ESENCIALES ---


// --- INICIO DEL BLOQUE DE SIEMBRA DE DATOS DE DESARROLLO/PRUEBA ---
// Este bloque solo se ejecuta cuando la aplicaci�n se est� ejecutando en el entorno de desarrollo.
//if (app.Environment.IsDevelopment())
//{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            Console.WriteLine("Iniciando proceso de siembra de datos de DESARROLLO...");
            await DevelopmentSeedData.Initialize(services);
            Console.WriteLine("Proceso de siembra de datos de DESARROLLO finalizado.");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Ocurri� un error al sembrar la base de datos con datos de desarrollo.");
        }
    }
//}
// --- FIN DEL BLOQUE DE SIEMBRA DE DATOS DE DESARROLLO/PRUEBA ---

app.UseHttpsRedirection();

app.UseCors("PermitirFrontend");

// Importante: primero autenticaci�n, luego autorizaci�n
app.UseAuthentication();
//app.UseMiddleware<CustomUnauthorizedMiddleware>(); //Se agregan el middleware para las peticiones a rutas protegidas que se hagan sin token de autorizaci�n
app.UseAuthorization();

app.MapControllers();

app.Run();