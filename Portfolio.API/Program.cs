using Portfolio.Services;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

builder.Services.Configure<GitHubSettings>(builder.Configuration.GetSection("GitHub"));

builder.Services.AddScoped<PortfolioService>();
builder.Services.AddScoped<IPortfolioService, CachedPortfolioService>(provider => 
    new CachedPortfolioService(
        provider.GetRequiredService<PortfolioService>(),
        provider.GetRequiredService<IMemoryCache>()
    ));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
