using Microsoft.EntityFrameworkCore;
using KvarnerAPI.Models;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policies => policies.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});
builder.Services.AddDbContext<ItemContext>(options =>
{
    var dbPath = Path.Combine(AppContext.BaseDirectory, "Databases", "Items.db");
    Directory.CreateDirectory(Path.GetDirectoryName(dbPath)); // Ensure the directory exists
    options.UseSqlite($"Data Source={dbPath}");
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

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();