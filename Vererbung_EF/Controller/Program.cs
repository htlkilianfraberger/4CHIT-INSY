using Microsoft.EntityFrameworkCore;
using Model.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection") 
                     ?? "Server=127.0.0.1;uid=root;pwd=insy;database=Vererbung_EF"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:5198")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorOrigin");
app.UseAuthorization();

app.MapControllers();

app.Run();