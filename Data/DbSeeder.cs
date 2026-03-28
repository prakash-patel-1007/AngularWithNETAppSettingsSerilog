using System;
using System.Linq;
using AngularWithNET.Domain;
using AngularWithNET.Features.Auth.Services;
using Microsoft.Extensions.Logging;

namespace AngularWithNET.Data
{
    public static class DbSeeder
    {
        public static void SeedDemoUsers(AppDbContext db, PasswordService passwordService, ILogger logger)
        {
            var demoUsers = new[]
            {
                new { Username = "admin", Password = "admin" },
                new { Username = "user",  Password = "user"  }
            };

            foreach (var demo in demoUsers)
            {
                if (db.Users.Any(u => u.Username == demo.Username))
                    continue;

                db.Users.Add(new User
                {
                    Username = demo.Username,
                    PasswordHash = passwordService.HashPassword(demo.Password),
                    CreatedAt = DateTime.UtcNow
                });
                logger.LogInformation("Seeded demo user: {Username}", demo.Username);
            }

            db.SaveChanges();
        }
    }
}
