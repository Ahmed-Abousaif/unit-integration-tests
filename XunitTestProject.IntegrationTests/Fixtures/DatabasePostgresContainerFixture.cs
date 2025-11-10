using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.MsSql;
using UnitTestSample01.Model;

namespace XunitTestProject.IntegrationTests.Fixtures
{
    /// <summary>
    /// Database fixture for integration tests using Testcontainers with PostgreSQL
    /// PostgreSQL is lighter, faster, and more reliable in containers than SQL Server
    /// </summary>
    public class DatabasePostgresContainerFixture : IAsyncLifetime
    {
        private readonly MsSqlContainer _postgresContainer;
        private IServiceProvider? _serviceProvider;

        public DatabasePostgresContainerFixture()
        {
            _postgresContainer = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .Build();
        }

        /// <summary>
        /// Initializes the container - called by xUnit before any tests run
        /// </summary>
        public async Task InitializeAsync()
        {
            Console.WriteLine("Starting PostgreSQL container... This should be quick!");
            
            // Start the PostgreSQL container
            await _postgresContainer.StartAsync();
            
            Console.WriteLine("PostgreSQL container started successfully!");
            Console.WriteLine($"Connection string: {_postgresContainer.GetConnectionString()}");

            // Setup dependency injection container
            var services = new ServiceCollection();

            // Configure DbContext with the container's connection string
            // Using PostgreSQL instead of SQL Server
            services.AddDbContext<AppDbContext>(options =>
                options.UseAzureSql(_postgresContainer.GetConnectionString())
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

            // Ensure database is created with migrations
            await context.Database.EnsureCreatedAsync();
            
            Console.WriteLine("PostgreSQL database schema created!");
        }

        /// <summary>
        /// Creates a new scope and returns it
        /// Each test should use a new scope to ensure proper isolation
        /// </summary>
        public IServiceScope CreateScope()
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("Container has not been initialized yet.");

            return _serviceProvider.CreateScope();
        }

        /// <summary>
        /// Gets a new DbContext instance within a scope
        /// </summary>
        public AppDbContext CreateContext()
        {
            var scope = CreateScope();
            return scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }

        /// <summary>
        /// Gets the connection string used by the container
        /// </summary>
        public string ConnectionString => _postgresContainer.GetConnectionString();

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

        /// <summary>
        /// Disposes the container - called by xUnit after all tests complete
        /// </summary>
        public async Task DisposeAsync()
        {
            Console.WriteLine("Stopping PostgreSQL container...");
            
            // Dispose service provider
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }

            // Stop and dispose the container
            await _postgresContainer.DisposeAsync();
            
            Console.WriteLine("PostgreSQL container stopped and removed.");
        }
    }
}

