using API.Middleware;
using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.HttpClients;
using Infrastructure.Repositories;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "ICMarkets Blockchain API",
        Version = "v1",
        Description = "API for fetching and storing blockchain data"
    });
});

// Configure SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

// MediatR for CQRS
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(IBlockchainService).Assembly));

// HttpClient for BlockCypher API
builder.Services.AddHttpClient<IBlockCypherClient, BlockCypherClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BlockCypherApi:BaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Repository Pattern & Unit of Work
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Application Services
builder.Services.AddScoped<IBlockchainService, BlockchainService>();

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database")
    .AddUrlGroup(new Uri("https://api.blockcypher.com/v1/btc/main"),
        name: "blockcypher-api",
        timeout: TimeSpan.FromSeconds(5));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();
// Migrate database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //context.Database.Migrate();
    context.Database.EnsureCreated();
}

// Configure middleware pipeline
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DefaultPolicy");

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
