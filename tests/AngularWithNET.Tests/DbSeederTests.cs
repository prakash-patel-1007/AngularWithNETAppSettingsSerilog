using System;
using System.Linq;
using AngularWithNET.Data;
using AngularWithNET.Features.Auth.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AngularWithNET.Tests
{
    public class DbSeederTests : IDisposable
    {
        private readonly SqliteConnection _connection;

        public DbSeederTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        private AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;
            var db = new AppDbContext(options);
            db.Database.EnsureCreated();
            return db;
        }

        [Fact]
        public void SeedDemoUsers_CreatesAdminAndUser()
        {
            using var db = CreateContext();
            var ps = new PasswordService();
            var logger = NullLogger<AppDbContext>.Instance;

            DbSeeder.SeedDemoUsers(db, ps, logger);

            Assert.Equal(2, db.Users.Count());
            Assert.True(db.Users.Any(u => u.Username == "admin"));
            Assert.True(db.Users.Any(u => u.Username == "user"));
        }

        [Fact]
        public void SeedDemoUsers_PasswordsAreHashed()
        {
            using var db = CreateContext();
            var ps = new PasswordService();
            var logger = NullLogger<AppDbContext>.Instance;

            DbSeeder.SeedDemoUsers(db, ps, logger);

            var admin = db.Users.First(u => u.Username == "admin");
            Assert.NotEqual("admin", admin.PasswordHash);
            Assert.True(ps.VerifyPassword(admin.PasswordHash, "admin"));
        }

        [Fact]
        public void SeedDemoUsers_IsIdempotent()
        {
            using var db = CreateContext();
            var ps = new PasswordService();
            var logger = NullLogger<AppDbContext>.Instance;

            DbSeeder.SeedDemoUsers(db, ps, logger);
            DbSeeder.SeedDemoUsers(db, ps, logger);

            Assert.Equal(2, db.Users.Count());
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
