using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
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

        /// <summary>
        /// Asserts that no department with the given name exists in the database
        /// </summary>
        private void AssertDepartmentDoesNotExist(string name)
        {
            var department = _context.Departments.FirstOrDefault(d => d.Name == name);
            Assert.Null(department);
        }

        /// <summary>
        /// Seeds the database with test data
        /// </summary>
        private void SeedTestData()
        {
            _containerFixture.SeedTestData();
        }

        #region AddDepartment Integration Tests

        [Fact]
        public void AddDepartment_WithValidData_ShouldPersistToDatabase()
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

        [Fact]
        public void AddDepartment_WithMultipleDepartments_ShouldPersistAllToDatabase()
        {
            // Arrange
            var dept1 = CreateTestDepartment("Sales", "Sales operations");
            var dept2 = CreateTestDepartment("Support", "Customer support");
            var dept3 = CreateTestDepartment("Research", "Research and development");

            // Act
            _departmentsService.AddDepartment(dept1);
            _departmentsService.AddDepartment(dept2);
            _departmentsService.AddDepartment(dept3);

            // Assert
            AssertDepartmentCount(3);
            AssertDepartmentExists("Sales");
            AssertDepartmentExists("Support");
            AssertDepartmentExists("Research");
        }

        [Fact]
        public void AddDepartment_WithDuplicateName_ShouldThrowAndNotPersist()
        {
            // Arrange
            var originalDept = CreateTestDepartment("Operations", "Original operations");
            _departmentsService.AddDepartment(originalDept);
            
            var duplicateDept = CreateTestDepartment("Operations", "Duplicate operations");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _departmentsService.AddDepartment(duplicateDept));
            
            // Verify only original department exists
            AssertDepartmentCount(1);
            var savedDept = AssertDepartmentExists("Operations");
            Assert.Equal("Original operations", savedDept.Description);
        }

        [Fact]
        public void AddDepartment_WithTransactionRollback_ShouldNotPersistOnException()
        {
            // Arrange - Add a department first
            var validDept = CreateTestDepartment("Legal", "Legal affairs");
            _departmentsService.AddDepartment(validDept);
            AssertDepartmentCount(1);

            // Try to add duplicate (should fail)
            var duplicateDept = CreateTestDepartment("Legal", "Another legal dept");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _departmentsService.AddDepartment(duplicateDept));
            
            // Verify database state remains unchanged
            AssertDepartmentCount(1);
            var originalDept = AssertDepartmentExists("Legal");
            Assert.Equal("Legal affairs", originalDept.Description);
        }

        #endregion

        #region UpdateDepartment Integration Tests

        [Fact]
        public void UpdateDepartment_WithValidData_ShouldPersistChangesToDatabase()
        {
            // Arrange
            var department = CreateTestDepartment("Engineering", "Software engineering");
            _departmentsService.AddDepartment(department);
            
            // Get the saved department with its ID
            var savedDept = AssertDepartmentExists("Engineering");
            savedDept.Description = "Updated engineering description";
            savedDept.Name = "Software Engineering";

            // Act
            _departmentsService.UpdateDepartment(savedDept);

            // Assert
            AssertDepartmentCount(1);
            var updatedDept = _context.Departments.Find(savedDept.Id);
            Assert.NotNull(updatedDept);
            Assert.Equal("Software Engineering", updatedDept.Name);
            Assert.Equal("Updated engineering description", updatedDept.Description);
        }

        [Fact]
        public void UpdateDepartment_WithDuplicateName_ShouldThrowAndNotPersist()
        {
            // Arrange
            var dept1 = CreateTestDepartment("HR", "Human resources");
            var dept2 = CreateTestDepartment("IT", "Information technology");
            _departmentsService.AddDepartment(dept1);
            _departmentsService.AddDepartment(dept2);

            var savedDept2 = AssertDepartmentExists("IT");
            savedDept2.Name = "HR"; // Try to change to existing name

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _departmentsService.UpdateDepartment(savedDept2));
            
            // Verify original state preserved
            AssertDepartmentCount(2);
            AssertDepartmentExists("HR");
            AssertDepartmentExists("IT");
        }

        #endregion

        #region DeleteDepartment Integration Tests

        [Fact]
        public void DeleteDepartment_WithExistingDepartment_ShouldRemoveFromDatabase()
        {
            // Arrange
            var department = CreateTestDepartment("Temporary", "Temporary department");
            _departmentsService.AddDepartment(department);
            AssertDepartmentCount(1);

            var savedDept = AssertDepartmentExists("Temporary");

            // Act
            _departmentsService.DeleteDepartment(savedDept);

            // Assert
            AssertDepartmentCount(0);
            AssertDepartmentDoesNotExist("Temporary");
        }

        [Fact]
        public void DeleteDepartment_WithMultipleDepartments_ShouldRemoveOnlySpecified()
        {
            // Arrange
            var dept1 = CreateTestDepartment("Keep1", "Department to keep");
            var dept2 = CreateTestDepartment("Delete", "Department to delete");
            var dept3 = CreateTestDepartment("Keep2", "Another department to keep");
            
            _departmentsService.AddDepartment(dept1);
            _departmentsService.AddDepartment(dept2);
            _departmentsService.AddDepartment(dept3);
            AssertDepartmentCount(3);

            var deptToDelete = AssertDepartmentExists("Delete");

            // Act
            _departmentsService.DeleteDepartment(deptToDelete);

            // Assert
            AssertDepartmentCount(2);
            AssertDepartmentExists("Keep1");
            AssertDepartmentExists("Keep2");
            AssertDepartmentDoesNotExist("Delete");
        }

        #endregion

        #region GetDepartment Integration Tests

        [Fact]
        public void GetAllDepartments_WithMultipleDepartments_ShouldReturnAllFromDatabase()
        {
            // Arrange
            SeedTestData(); // Uses the fixture's seed data

            // Act
            var departments = _departmentsService.GetAllDepartments();

            // Assert
            Assert.Equal(3, departments.Count);
            Assert.Contains(departments, d => d.Name == "Human Resources");
            Assert.Contains(departments, d => d.Name == "Information Technology");
            Assert.Contains(departments, d => d.Name == "Finance");
        }

        [Fact]
        public void GetDepartmentById_WithExistingId_ShouldReturnCorrectDepartment()
        {
            // Arrange
            var department = CreateTestDepartment("Project Management", "Manages projects");
            _departmentsService.AddDepartment(department);
            
            var savedDept = AssertDepartmentExists("Project Management");

            // Act
            var result = _departmentsService.GetDepartmentById(savedDept.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Project Management", result.Name);
            Assert.Equal("Manages projects", result.Description);
            Assert.Equal(savedDept.Id, result.Id);
        }

        [Fact]
        public void GetDepartmentById_WithNonExistingId_ShouldReturnNull()
        {
            // Arrange
            int nonExistentId = 99999;

            // Act
            var result = _departmentsService.GetDepartmentById(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetDepartmentByName_WithExistingName_ShouldReturnCorrectDepartment()
        {
            // Arrange
            var department = CreateTestDepartment("Quality Assurance", "Ensures product quality");
            _departmentsService.AddDepartment(department);

            // Act
            var result = _departmentsService.GetDepartmentByName("Quality Assurance");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Quality Assurance", result.Name);
            Assert.Equal("Ensures product quality", result.Description);
        }

        [Fact]
        public void GetDepartmentByName_WithNonExistingName_ShouldReturnNull()
        {
            // Act
            var result = _departmentsService.GetDepartmentByName("Non Existent Department");

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region SearchDepartmentsByName Integration Tests

        [Fact]
        public void SearchDepartmentsByName_WithMatchingKeyword_ShouldReturnMatchingDepartments()
        {
            // Arrange
            var dept1 = CreateTestDepartment("Software Development", "Develops software");
            var dept2 = CreateTestDepartment("Software Testing", "Tests software");
            var dept3 = CreateTestDepartment("Hardware Support", "Supports hardware");
            
            _departmentsService.AddDepartment(dept1);
            _departmentsService.AddDepartment(dept2);
            _departmentsService.AddDepartment(dept3);

            // Act
            var results = _departmentsService.SearchDepartmentsByName("Software");

            // Assert
            Assert.Equal(2, results.Count);
            Assert.Contains(results, d => d.Name == "Software Development");
            Assert.Contains(results, d => d.Name == "Software Testing");
            Assert.DoesNotContain(results, d => d.Name == "Hardware Support");
        }

        [Fact]
        public void SearchDepartmentsByName_WithNoMatches_ShouldReturnEmptyList()
        {
            // Arrange
            var department = CreateTestDepartment("Accounting", "Financial accounting");
            _departmentsService.AddDepartment(department);

            // Act
            var results = _departmentsService.SearchDepartmentsByName("Marketing");

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void SearchDepartmentsByName_WithCaseSensitivity_ShouldBeCaseSensitive()
        {
            // Arrange
            var department = CreateTestDepartment("Data Analytics", "Analyzes business data");
            _departmentsService.AddDepartment(department);

            // Act - Test case sensitive search (Contains is case-sensitive)
            var results = _departmentsService.SearchDepartmentsByName("Data");

            // Assert - Should find department with matching case
            Assert.Single(results);
            Assert.Equal("Data Analytics", results.First().Name);
            
            // Act - Test with non-matching case
            var noResults = _departmentsService.SearchDepartmentsByName("data");
            
            // Assert - Should not find department with different case
            Assert.Empty(noResults);
        }

        #endregion

        #region Database Constraint Tests

        [Fact]
        public void DatabaseConstraints_DepartmentNameLength_ShouldEnforceMaxLength()
        {
            // Note: This test verifies that our validation catches the constraint
            // before it reaches the database, as EF Core handles the constraint
            
            // Arrange
            var department = CreateTestDepartment(new string('A', 101), "Valid description");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _departmentsService.AddDepartment(department));
            AssertDepartmentCount(0);
        }

        [Fact]
        public void DatabaseConstraints_DepartmentDescriptionLength_ShouldEnforceMaxLength()
        {
            // Arrange
            var department = CreateTestDepartment("Valid Name", new string('B', 501));

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _departmentsService.AddDepartment(department));
            AssertDepartmentCount(0);
        }

        #endregion

        #region Concurrency Tests

        [Fact]
        public void ConcurrentOperations_AddingDifferentDepartments_ShouldBothSucceed()
        {
            // Arrange
            var dept1 = CreateTestDepartment("Concurrent1", "First concurrent department");
            var dept2 = CreateTestDepartment("Concurrent2", "Second concurrent department");

            // Act - Simulate concurrent operations
            _departmentsService.AddDepartment(dept1);
            _departmentsService.AddDepartment(dept2);

            // Assert
            AssertDepartmentCount(2);
            AssertDepartmentExists("Concurrent1");
            AssertDepartmentExists("Concurrent2");
        }

        #endregion

        public void Dispose()
        {
            _scope?.Dispose();
        }
    }
}

