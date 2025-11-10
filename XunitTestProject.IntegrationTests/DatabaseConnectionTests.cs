using Microsoft.EntityFrameworkCore;
using UnitTestSample01.Model;
using XunitTestProject.IntegrationTests.Fixtures;
using XunitTestProject.IntegrationTests.Collections;

namespace XunitTestProject.IntegrationTests
{
    /// <summary>
    /// Tests to verify LocalDB connection and basic database operations
    /// </summary>
    [Collection("Database collection")]
    public class DatabaseConnectionTests : IDisposable
    {
        private readonly DatabaseFixture _databaseFixture;
        private readonly AppDbContext _context;

        public DatabaseConnectionTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            _context = _databaseFixture.CreateContext();
        }

        [Fact]
        public void Database_CanConnect_ShouldEstablishConnection()
        {
            // Act & Assert
            Assert.True(_context.Database.CanConnect());
        }

        [Fact]
        public void Database_HasCorrectProvider_ShouldUseSqlServer()
        {
            // Act
            var provider = _context.Database.ProviderName;

            // Assert
            Assert.Equal("Microsoft.EntityFrameworkCore.SqlServer", provider);
        }

        [Fact]
        public void Database_HasDepartmentTable_ShouldExist()
        {
            // Arrange & Act
            var tableExists = _context.Departments.Any();

            // Assert - If the table exists, this should not throw
            // We don't care about the result, just that the table can be queried
            Assert.True(true); // The fact that we get here means the table exists
        }

        [Fact]
        public void Database_CanInsertAndRetrieveDepartment_ShouldWork()
        {
            // Arrange
            var department = new UnitTestSample01.Entities.Department
            {
                Name = "Test Department",
                Description = "Test Description"
            };

            // Act
            _context.Departments.Add(department);
            _context.SaveChanges();

            var retrievedDepartment = _context.Departments
                .FirstOrDefault(d => d.Name == "Test Department");

            // Assert
            Assert.NotNull(retrievedDepartment);
            Assert.Equal("Test Department", retrievedDepartment.Name);
            Assert.Equal("Test Description", retrievedDepartment.Description);
            Assert.True(retrievedDepartment.Id > 0);
        }

        [Fact]
        public void Database_MigrationStatus_ShouldBeUpToDate()
        {
            // Act
            var pendingMigrations = _context.Database.GetPendingMigrations().ToList();

            // Assert
            Assert.Empty(pendingMigrations);
        }

        [Fact]
        public void Database_ConnectionString_ShouldBeLocalDb()
        {
            // Act
            var connectionString = _context.Database.GetConnectionString();

            // Assert
            Assert.NotNull(connectionString);
            Assert.Contains("(localdb)", connectionString);
            Assert.Contains("Trusted_Connection=True", connectionString);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}