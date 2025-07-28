
using Entwin.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=entwin.db"));

builder.Services.AddHealthChecks();


builder.Services.AddSwaggerGen();
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri("http://localhost:5265/") });

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Testing")
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.MapHealthChecks("/health");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
