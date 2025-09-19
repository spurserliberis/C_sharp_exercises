using Microsoft.EntityFrameworkCore;
using Controller_based_API.Models;
using Controller_based_API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Adds the database context to the DI container.
// Specifies that the database context will use an in-memory database.
builder.Services.AddDbContext<TodoContext>(opt =>
    opt.UseInMemoryDatabase("TodoList"));

builder.Services.AddTransient<ITodoRepository, TodoRepository>();
// The DI container is initialized here, allowing services to be
// registered and injected throughout the app.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
public partial class Program { }