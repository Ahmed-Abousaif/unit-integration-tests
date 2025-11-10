using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using UnitTestSample01.Model;

namespace XunitTestProject.IntegrationTests.Fixtures
{
    /// <summary>
    /// Database fixture for integration tests using LocalDB
    /// Follows Microsoft's recommended testing patterns for EF Core
    /// </summary>
    public class DatabaseFixture : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _connectionString;
        
        public DatabaseFixture()
        {
            // Create unique database name for test isolation
            var databaseName = $"IntegrationTestDb_{Guid.NewGuid():N}";
            _connectionString = $"Server=(localdb)\\mssqllocaldb;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true";

            // Setup dependency injection container
            var services = new ServiceCollection();
            
            // Configure DbContext with LocalDB
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(_connectionString)
                       .EnableSensitiveDataLogging()
                       .EnableDetailedErrors());

            // Configure logging for debugging
            services.AddLogging(builder =>
                builder.AddConsole()
                       .SetMinimumLevel(LogLevel.Information));

            _serviceProvider = services.BuildServiceProvider();

            // Create and migrate database
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            // Ensure database is created and up to date
            // First ensure the database exists
            context.Database.EnsureCreated();
            
            // Then apply any pending migrations (this handles the case where migrations were added after EnsureCreated)
            var pendingMigrations = context.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                // Delete and recreate to apply migrations properly
                context.Database.EnsureDeleted();
                context.Database.Migrate();
            }
        }

        /// <summary>
        /// Creates a new scope and returns the DbContext
        /// Each test should use a new scope to ensure proper isolation
        /// </summary>
        /// <returns>A scoped DbContext instance</returns>
        public IServiceScope CreateScope()
        {
            return _serviceProvider.CreateScope();
        }

        /// <summary>
        /// Gets a new DbContext instance within a scope
        /// </summary>
        /// <returns>AppDbContext instance</returns>
        public AppDbContext CreateContext()
        {
            var scope = CreateScope();
            return scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }

        /// <summary>
        /// Gets the connection string used by this fixture
        /// </summary>
        public string ConnectionString => _connectionString;

        /// <summary>
        /// Seeds the database with test data
        /// </summary>
        public void SeedTestData()
        {
            using var scope = CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            // Clear existing data
            context.Departments.RemoveRange(context.Departments);
            context.SaveChanges();

            // Add seed data
            var departments = new[]
            {
                new UnitTestSample01.Entities.Department 
                { 
                    Name = "Human Resources", 
                    Description = "Manages employee relations and recruitment" 
                },
                new UnitTestSample01.Entities.Department 
                { 
                    Name = "Information Technology", 
                    Description = "Manages technology infrastructure and systems" 
                },
                new UnitTestSample01.Entities.Department 
                { 
                    Name = "Finance", 
                    Description = "Handles financial operations and accounting" 
                }
            };

            context.Departments.AddRange(departments);
            context.SaveChanges();
        }

        /// <summary>
        /// Cleans the database by removing all data
        /// </summary>
        public void CleanDatabase()
        {
            using var scope = CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            // Remove all departments
            context.Departments.RemoveRange(context.Departments);
            context.SaveChanges();
        }

        public void Dispose()
        {
            // Clean up database
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.EnsureDeleted();
            }
            catch (Exception)
            {
                // Ignore cleanup errors during disposal
            }

            // Dispose service provider
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}