// Probanding 2
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using VPASS3_backend.Context;
using VPASS3_backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// crear variable para la cadena de conexi�n

// el "connection" es el mismo que se defini� en context, es por eso de su nombre.
var connectionString = builder.Configuration.GetConnectionString("Connection");

// registrar servicio para la conexi�n
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(connectionString)
 );

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

///////
///
builder.Services.AddSwaggerGen(options =>
{
    // Define un documento de especificaci�n OpenAPI para la versi�n "v1" de la API.
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        // Especifica el t�tulo de la API en la documentaci�n de Swagger.
        Title = "VPASS3",

        // Define la versi�n de la API.
        Version = "v1",

        // Proporciona una breve descripci�n de la API.
        Description = "Sistema de control de visitas",

        // Define informaci�n de contacto del desarrollador o responsable de la API.
        Contact = new OpenApiContact()
        {
            Name = "Gerardo Lucero C�rdova",  // Nombre del contacto.
            Email = "gerardoluceroc@gmail.com", // Correo de contacto.
            Url = new Uri("https://github.com/gerardoluceroc") // URL del perfil o sitio del contacto.
        }
    });

    // Habilita el uso de anotaciones en Swagger para mejorar la documentaci�n de los endpoints.
    options.EnableAnnotations();

    // Obtiene el nombre del archivo XML generado por la documentaci�n de comentarios en el c�digo.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    // Construye la ruta completa donde se encuentra el archivo XML.
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    // Incluye los comentarios del archivo XML en la documentaci�n de Swagger.
    options.IncludeXmlComments(xmlPath);

    // Agrega una definici�n de seguridad para autenticaci�n mediante tokens JWT.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", // Nombre del encabezado de autorizaci�n.
        Type = SecuritySchemeType.ApiKey, // Indica que la autenticaci�n se basa en claves API.
        Scheme = "Bearer ", // Especifica el esquema de autenticaci�n como Bearer Token.
        BearerFormat = "JWT", // Indica que se utilizar� un token en formato JWT.
        Description = "JWT Authorization header using the Bearer scheme.", // Descripci�n de c�mo funciona el esquema de autenticaci�n.
        In = ParameterLocation.Header, // Indica que el token JWT debe enviarse en los encabezados de la solicitud.
    });

    // Define los requisitos de seguridad para los endpoints de la API.
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme, // Indica que se est� haciendo referencia a un esquema de seguridad.
                    Id = "Bearer" // Debe coincidir con el Id definido en `AddSecurityDefinition`.
                },
            },
            new string[]{} // Indica que este esquema se aplica a todos los endpoints sin restricciones adicionales.
        }
    });
});

////////////////

// Aqu� registramos el servicio `UserService` en el contenedor de dependencias
builder.Services.AddScoped<UserService>();

// Registra el servicio RoleService
builder.Services.AddScoped<RoleService>();  // Esta es la nueva l�nea que agregamos

var app = builder.Build();

app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CP2 API V1");
    c.RoutePrefix = string.Empty; // Hace que Swagger est� en la ra�z (localhost:5000/)
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();









































// Original mio

//using Microsoft.EntityFrameworkCore;
//using Microsoft.OpenApi.Models;
//using System.Reflection;
//using VPASS3_backend.Context;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//// crear variable para la cadena de conexi�n

//// el "connection" es el mismo que se defini� en context, es por eso de su nombre.
//var connectionString = builder.Configuration.GetConnectionString("Connection");

//// registrar servicio para la conexi�n
//builder.Services.AddDbContext<AppDbContext>(
//    options => options.UseSqlServer(connectionString)
// );

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//app.UseSwagger(options =>
//{
//    options.SerializeAsV2 = true;
//});

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();















































































// Codigo original Alberto


//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("V1", new OpenApiInfo
//    {
//        Title = "CP2",
//        Version = "V1",
//        Description = "Sistema de control de Accesos",
//        Contact = new OpenApiContact()
//        {
//            Name = "Alberto Salamanca [ KylarDev ]",
//            Email = "asalamanca@automatismoslau.cl",
//            Url = new Uri("https://github.com/Kylar-stern30")
//        }
//    }); ; ;

//    options.EnableAnnotations();

//    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
//    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
//    options.IncludeXmlComments(xmlPath);
//    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Type = SecuritySchemeType.ApiKey,
//        Scheme = "Bearer ",
//        BearerFormat = "JWT",
//        Description = "JWT Authorization header using the Bearer scheme.",
//        In = ParameterLocation.Header,
//    });
//    options.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                },
//            },
//            new string[]{}
//        }
//    });

//});

//app.UseSwagger();
//app.UseSwaggerUI(option =>
//{
//    option.SwaggerEndpoint("V1/swagger.json", "V1");
//});

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
//app.MapRazorPages();

//app.Run();