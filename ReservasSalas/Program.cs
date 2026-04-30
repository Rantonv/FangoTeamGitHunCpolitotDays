using ReservasSalas.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configurar ProblemDetails para manejo estandarizado de errores
builder.Services.AddProblemDetails();

// Registrar el servicio de reservas como Singleton (datos en memoria)
builder.Services.AddSingleton<IReservasService, ReservasService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Middleware para manejo de excepciones y ProblemDetails
app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
