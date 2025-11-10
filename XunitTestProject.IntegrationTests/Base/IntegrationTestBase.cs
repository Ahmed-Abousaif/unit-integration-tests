using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UnitTestSample01.Model;
using UnitTestSample01.Services;
using XunitTestProject.IntegrationTests.Collections;
using XunitTestProject.IntegrationTests.Fixtures;

namespace XunitTestProject.IntegrationTests.Base
{
    /// <summary>
    /// Base class for integration tests that provides common setup and utilities
    /// </summary>
    [Collection("Database collection")]
    public abstract class IntegrationTestBase : IDisposable
    {
        protected readonly DatabaseFixture _databaseFixture;
        protected readonly IServiceScope _scope;
        protected readonly AppDbContext _context;
        protected readonly ILogger<DepartmentsService> _logger;
        protected readonly DepartmentsService _departmentsService;

        protected IntegrationTestBase(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            _scope = _databaseFixture.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            // Create logger for DepartmentsService
            var loggerFactory = _scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<DepartmentsService>();
            
            // Create the service under test
            _departmentsService = new DepartmentsService(_context, _logger);
            
            // Clean database before each test
            CleanDatabase();
        }

        /// <summary>
        /// Cleans the database to ensure test isolation
        /// </summary>
        protected virtual void CleanDatabase()
        {
            _databaseFixture.CleanDatabase();
        }

        /// <summary>
        /// Seeds the database with test data
        /// </summary>
        protected virtual void SeedTestData()
        {
            _databaseFixture.SeedTestData();
        }

        /// <summary>
        /// Creates a new department for testing
        /// </summary>
        /// <param name="name">Department name</param>
        /// <param name="description">Department description</param>
        /// <returns>New Department instance</returns>
        protected UnitTestSample01.Entities.Department CreateTestDepartment(string name, string? description = null)
        {
            return new UnitTestSample01.Entities.Department
            {
                Name = name,
                Description = description ?? $"Description for {name}"
            };
        }

        /// <summary>
        /// Asserts that the database contains exactly the specified number of departments
        /// </summary>
        /// <param name="expectedCount">Expected number of departments</param>
        protected void AssertDepartmentCount(int expectedCount)
        {
            var actualCount = _context.Departments.Count();
            Assert.Equal(expectedCount, actualCount);
        }

        /// <summary>
        /// Asserts that a department with the given name exists in the database
        /// </summary>
        /// <param name="name">Department name to check</param>
        /// <returns>The found department</returns>
        protected UnitTestSample01.Entities.Department AssertDepartmentExists(string name)
        {
            var department = _context.Departments.FirstOrDefault(d => d.Name == name);
            Assert.NotNull(department);
            return department;
        }

        /// <summary>
        /// Asserts that no department with the given name exists in the database
        /// </summary>
        /// <param name="name">Department name to check</param>
        protected void AssertDepartmentDoesNotExist(string name)
        {
            var department = _context.Departments.FirstOrDefault(d => d.Name == name);
            Assert.Null(department);
        }

        public virtual void Dispose()
        {
            _scope?.Dispose();
        }
    }
}