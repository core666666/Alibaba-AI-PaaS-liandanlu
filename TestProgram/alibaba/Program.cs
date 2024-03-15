using alibaba.Model;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<AITokenOptions>(builder.Configuration.GetSection("AITokenOptions"));

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
