using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UnitTestSample01.Entities;
using UnitTestSample01.Model;
using UnitTestSample01.Services;
using XunitTestProject.IntegrationTests.Fixtures;

namespace XunitTestProject.IntegrationTests
{
    /// <summary>
    /// Integration tests for DepartmentsService using Testcontainers with PostgreSQL
    /// This demonstrates testing with a real PostgreSQL container instead of in-memory database
    /// PostgreSQL is faster and more reliable in containers than SQL Server
    /// </summary>
    [Collection("Database postgres container collection")]
    public class DepartmentsServiceContainerTests : IDisposable
    {
        private readonly DatabasePostgresContainerFixture _containerFixture;
        private readonly IServiceScope _scope;
        private readonly AppDbContext _context;
        private readonly ILogger<DepartmentsService> _logger;
        private readonly DepartmentsService _departmentsService;

        public DepartmentsServiceContainerTests(DatabasePostgresContainerFixture containerFixture)
        {
            _containerFixture = containerFixture;
            _scope = _containerFixture.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create logger for DepartmentsService
            var loggerFactory = _scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<DepartmentsService>();

            // Create the service under test
            _departmentsService = new DepartmentsService(_context, _logger);

            // Clean database before test
            CleanDatabase();
        }

        /// <summary>
        /// Cleans the database to ensure test isolation
        /// </summary>
        private void CleanDatabase()
        {
            _containerFixture.CleanDatabase();
        }

        /// <summary>
        /// Creates a new department for testing
        /// </summary>
        private Department CreateTestDepartment(string name, string? description = null)
        {
            return new Department
            {
                Name = name,
                Description = description ?? $"Description for {name}"
            };
        }

        /// <summary>
        /// Asserts that the database contains exactly the specified number of departments
        /// </summary>
        private void AssertDepartmentCount(int expectedCount)
        {
            var actualCount = _context.Departments.Count();
            Assert.Equal(expectedCount, actualCount);
        }

        /// <summary>
        /// Asserts that a department with the given name exists in the database
        /// </summary>
        private Department AssertDepartmentExists(string name)
        {
            var department = _context.Departments.FirstOrDefault(d => d.Name == name);
            Assert.NotNull(department);
            return department;
        }

        #region Testcontainer-based Integration Test

        [Fact]
        public void AddDepartment_WithValidData_ShouldPersistToRealPostgreSqlContainer()
        {
            // Arrange
            var department = CreateTestDepartment("Marketing", "Handles marketing and promotions");

            // Act
            _departmentsService.AddDepartment(department);

            // Assert
            AssertDepartmentCount(1);
            var savedDepartment = AssertDepartmentExists("Marketing");
            Assert.Equal("Handles marketing and promotions", savedDepartment.Description);
            Assert.True(savedDepartment.Id > 0); // Verify ID was assigned by database
        }

        #endregion

        public void Dispose()
        {
            _scope?.Dispose();
        }
    }
}

