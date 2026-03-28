using System;
using System.IO;
using System.Linq;
using AngularWithNET.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace AngularWithNET
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            try
            {
                var host = CreateHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var db = services.GetRequiredService<AppDbContext>();
                    ValidateDatabasePath(db);
                    db.Database.Migrate();
                    Log.Information("Database migration completed successfully");

                    var env = services.GetRequiredService<IWebHostEnvironment>();
                    if (env.IsDevelopment())
                    {
                        var passwordService = services.GetRequiredService<Features.Auth.Services.PasswordService>();
                        var logger = services.GetRequiredService<Microsoft.Extensions.Logging.ILogger<AppDbContext>>();
                        Data.DbSeeder.SeedDemoUsers(db, passwordService, logger);
                    }
                }

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
                Environment.ExitCode = 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void ValidateDatabasePath(AppDbContext db)
        {
            var connectionString = db.Database.GetConnectionString();
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured. Set ConnectionStrings:DefaultConnection in appsettings.json.");
            }

            var dataSource = connectionString
                .Split(';')
                .Select(p => p.Trim())
                .FirstOrDefault(p => p.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase));

            if (dataSource == null) return;

            var dbPath = dataSource.Substring("Data Source=".Length).Trim();
            if (string.IsNullOrEmpty(dbPath) || dbPath == ":memory:") return;

            var dir = Path.GetDirectoryName(Path.GetFullPath(dbPath));
            if (string.IsNullOrEmpty(dir)) return;

            if (!Directory.Exists(dir))
            {
                try
                {
                    Directory.CreateDirectory(dir);
                    Log.Information("Created database directory: {DbDirectory}", dir);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Cannot create database directory '{dir}'. Ensure the path is writable.", ex);
                }
            }

            var testFile = Path.Combine(dir, $".write-test-{Guid.NewGuid():N}");
            try
            {
                File.WriteAllText(testFile, "test");
                File.Delete(testFile);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Database directory '{dir}' is not writable. Ensure the application has write permissions.", ex);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
