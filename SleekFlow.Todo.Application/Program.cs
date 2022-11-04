using SleekFlow.Todo.Application.EventHandler;
using SleekFlow.Todo.Application.EventSubscription;
using SleekFlow.Todo.Domain;
using SleekFlow.Todo.Infrastructure;
using SleekFlow.Todo.Infrastructure.EmbeddedEventStoreDB;
using IEventStore = SleekFlow.Todo.Infrastructure.EmbeddedEventStoreDB.IEventStore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddSingleton<IEventStore, EmbeddedEventStoreDb>();
builder.Services.AddSingleton<ITodoItemProjectionEventHandler, TodoItemProjectionEventHandler>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddEventSubscription<TodoItemProjectionSubscriptionBackgroundService>(opts =>
{
    opts.ApplicationName = "SleekFlowTodo";
    opts.Username = builder.Configuration["EVENTSTORE_USERNAME"];
    opts.Password = builder.Configuration["EVENTSTORE_PASSWORD"];
    opts.SubscribeToStream = "$ce-TodoItem";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
