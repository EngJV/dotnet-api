// Crea un constructor de aplicaciones web utilizando los argumentos pasados al programa (args).
using api.Data;
using api.Interfaces;
using api.Models;
using api.Repository;
using api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers(); // AddControllers: Agrega servicios necesarios para la creación de controladores de API.
builder.Services.AddEndpointsApiExplorer(); // AddEndpointsApiExplorer: Agrega servicios necesarios para la exploración de los endpoints de la API.
builder.Services.AddSwaggerGen(); // AddSwaggerGen: Agrega servicios para generar documentación de la API utilizando Swagger.

builder.Services.AddSwaggerGen(option =>

// Agrega documentación de la API utilizando Swagger.
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddControllers().AddNewtonsoftJson(options => // AddNewtonsoftJson: Agrega servicios para serialización y deserialización de JSON utilizando Newtonsoft.Json.
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; // Configura la serialización para ignorar referencias circulares en los objetos.
});

builder.Services.AddDbContext<ApplicationDBContext>(options => // AddDbContext: Agrega un servicio de contexto de base de datos para la aplicación.
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); // UseSqlServer: Configura el contexto de base de datos para usar SQL Server y proporciona la cadena de conexión.

});

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
})
.AddEntityFrameworkStores<ApplicationDBContext>();


// Agrega servicios de autenticación y autorización.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
        )
    };
});

builder.Services.AddScoped<IStockRepository, StockRepository>(); // AddScoped: Agrega un servicio de alcance de solicitud para la interfaz IStockRepository y su implementación StockRepository.
builder.Services.AddScoped<ICommentRepository, CommentRepository>(); // AddScoped: Agrega un servicio de alcance de solicitud para la interfaz ICommentRepository y su implementación CommentRepository.
builder.Services.AddScoped<ITokenService, TokenService>(); // AddScoped: Agrega un servicio de alcance de solicitud para la interfaz ITokenService y su implementación TokenService.
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>(); // AddScoped: Agrega un servicio de alcance de solicitud para la interfaz IPortfolioRepository y su implementación PortfolioRepository.

var app = builder.Build(); // Construye la aplicación utilizando el constructor configurado anteriormente.

// Configure the HTTP request pipeline.
/* Si el entorno de la aplicación es de desarrollo (Development), 
configura Swagger para generar y mostrar la interfaz de usuario de la documentación API.
 */
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // UseSwagger: Habilita la generación de la especificación Swagger.
    app.UseSwaggerUI(); // UseSwaggerUI: Habilita la interfaz gráfica para explorar y probar los endpoints de la API.
}

app.UseHttpsRedirection(); // Fuerza el uso de HTTPS redirigiendo todas las solicitudes HTTP a HTTPS.


app.UseAuthentication();
app.UseAuthorization();

/* BOILERPLATE
// Define un punto de entrada para la API que devuelve una lista de pronósticos meteorológicos.
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// Define un punto de entrada para la API que devuelve una lista de pronósticos meteorológicos.
app.MapGet("/weatherforecast", () => // Define un endpoint HTTP GET en /weatherforecast.
{ 
    var forecast =  Enumerable.Range(1, 5).Select(index => // Enumerable.Range(1, 5): Genera una secuencia de números del 1 al 5.
        new WeatherForecast // Crea un objeto WeatherForecast para cada elemento de la secuencia.
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)), // DateOnly.FromDateTime(DateTime.Now.AddDays(index)): Calcula la fecha de cada día.
            Random.Shared.Next(-20, 55), // Random.Shared.Next(-20, 55): Genera una temperatura aleatoria entre -20 y 55 grados Celsius.
            summaries[Random.Shared.Next(summaries.Length)] // summaries[Random.Shared.Next(summaries.Length)]: Selecciona un resumen aleatorio de la lista de resumenes.
        ))
        .ToArray(); // Convierte la secuencia en una matriz.
    return forecast; // Devuelve la matriz de pronósticos meteorológicos.
})
.WithName("GetWeatherForecast") // Asigna un nombre al punto de entrada.
.WithOpenApi(); // Agrega metadatos de OpenAPI para documentar el punto de entrada.
*/

app.MapControllers(); // Mapea los controladores de la aplicación.

app.Run(); // Inicia la aplicación y comienza a escuchar las solicitudes entrantes.


/* BOILERPLATE 
// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary) // Define un registro WeatherForecast con tres propiedades:

//Date: Fecha del pronóstico.
//TemperatureC: Temperatura en grados Celsius.
//Summary: Descripción del clima.
 
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556); // Calcula la temperatura en grados Fahrenheit.
}

*/