using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false);
builder.Services
    .AddOcelot()
    .AddPolly();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("GatewayPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("GatewayPolicy");

app.UseHttpsRedirection();

app.MapWhen(context => context.Request.Path == "/", appBuilder =>
{
    appBuilder.Run(async context =>
    {
        await context.Response.WriteAsync("ICMarkets Blockchain API Gateway");
    });
});

await app.UseOcelot();

app.Run();