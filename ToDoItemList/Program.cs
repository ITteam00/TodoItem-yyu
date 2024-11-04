using TodoItem.core;
using TodoItems.Core;
using TodoItem.core.Services;
using TodoItems.Core.Services;
using TodoItem.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSingleton < IToDoItemsService, InMemoryToDoItemsService>();
builder.Services.Configure<TodoItem.core.ToDoItemDatabaseSettings>(builder.Configuration.GetSection("ToDoItemDatabase"));
builder.Services.Configure<TodoItem.Infrastructure.TodoItemsDatabaseSettings>(builder.Configuration.GetSection("ToDoItemDatabase"));
builder.Services.AddSingleton<IToDoItemsService, ToDoItemsService>();
builder.Services.AddSingleton<ITodosRepository, TodoItemsRepository>();
builder.Services.AddSingleton<TodoItemService, TodoItemService>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.Run();

public partial class Program { }
