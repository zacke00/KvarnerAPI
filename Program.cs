using Microsoft.EntityFrameworkCore;
using KvarnerAPI.Models;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddDbContext<ItemContext>(options =>
{
    // Define the path to the database file
    var projectRoot = Directory.GetCurrentDirectory();
    var dbPath = Path.Combine(projectRoot, "Databases", "Item.db");
    
    // Ensure the directory exists
    Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
    
    options.UseSqlite($"Data Source={dbPath}");
    Console.WriteLine($"Database path: {dbPath}");
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

DefaultFilesOptions options = new DefaultFilesOptions();
options.DefaultFileNames.Add("index.html");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();