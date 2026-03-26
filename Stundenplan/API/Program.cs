using Microsoft.EntityFrameworkCore;
using Model;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics; // WICHTIG für ExceptionHandler

var builder = WebApplication.CreateBuilder(args);

// 1. DbContext registrieren (MySQL)
builder.Services.AddDbContext<StundenplanContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection") 
                     ?? "Server=127.0.0.1;uid=root;pwd=insy;database=Stundenplan"));

// 2. Controller mit JSON-Optionen
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

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

// --- NEU: Globaler Exception Handler ---
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        // Wir senden immer einen BadRequest (400) oder InternalServerError (500)
        context.Response.StatusCode = StatusCodes.Status400BadRequest; 
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        // Das JSON, das beim Client (Blazor) ankommt
        await context.Response.WriteAsJsonAsync(new 
        { 
            error = "Server-Fehler",
            message = exception?.Message // Hier steht z.B. deine Fehlermeldung drin
        });
    });
});
// ---------------------------------------

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stundenplan API V1");
    });
}

app.UseCors(); 
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();