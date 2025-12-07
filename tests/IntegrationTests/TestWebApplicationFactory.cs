using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(AppDbContext));

                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open(); // keep it open for the entire test run

                // Register the connection so DbContext uses THIS one
                services.AddSingleton(connection);

                // Add in-memory database for testing
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlite(connection);
                });

                // Build service provider and create database
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            });
            builder.UseEnvironment("Testing");
        }
    }
}
