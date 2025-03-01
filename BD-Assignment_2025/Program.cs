using BD_Assignment_2025;
using BD_Assignment_2025.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.RegisterServices();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionHandler>();
app.UseHttpsRedirection();
app.UseRouting();
app.RegisterAllEndpoints();

app.UseAuthorization();
app.MapControllers();

app.Run();
