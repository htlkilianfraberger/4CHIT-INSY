using Microsoft.EntityFrameworkCore;
using Model;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. DbContext registrieren (MySQL)
builder.Services.AddDbContext<StundenplanContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection") 
                     ?? "Server=127.0.0.1;uid=root;pwd=insy;database=Stundenplan"));

// 2. Controller mit JSON-Optionen (verhindert Zyklen bei 1:n Beziehungen)
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// 3. Swagger/OpenAPI Konfiguration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

// 4. CORS aktivieren (damit der Blazor-Client auf die API zugreifen darf)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stundenplan API V1");
    });
}

// WICHTIG: Die Reihenfolge der Middleware
app.UseCors(); 

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();