using CookBookBackend.Data;
using CookBookBackend.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CookBookBackend.Tests.Integration
{
    public class MyWebAppFactory : WebApplicationFactory<Program>
    {

        private SqliteConnection? _connection;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the real DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);


                _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();


                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                });
                
                


                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();

                // SeedTestData(db);
            });
        }

        private void SeedTestData(AppDbContext db)
        {
            throw new NotImplementedException();
        }

        public override async ValueTask DisposeAsync()
        {
            if (_connection != null) 
                await _connection.DisposeAsync();
            await base.DisposeAsync();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _connection?.Dispose();
            base.Dispose(disposing);
        }
    }

}
