using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestSample01.Entities;
using UnitTestSample01.Model;
using UnitTestSample01.Services;

namespace XunitTestProject
{
    public class DeprtamentsServiceUnitTests
    {
        [Fact]
        public void Add_Department_Will_Not_Throw_Exception()
        {
            // Arrange
            DbContextOptionsBuilder<AppDbContext> optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "TestDatabase");
            AppDbContext appDbContext = new AppDbContext(optionsBuilder.Options);
            var loggerMock = new Mock<ILogger<DepartmentsService>>();
            DepartmentsService departmentsService = new DepartmentsService(appDbContext, loggerMock.Object);
            Department department = new Department
            {
                Name = "Human Resources",
                Description = "Handles recruitment and employee relations."
            };
            // Act
            departmentsService.AddDepartment(department);
            // Assert
        }

        [Fact]
        public void Add_Department_Throws_Exception_When_Name_Is_Null()
        {
            // Arrange
            DbContextOptionsBuilder<AppDbContext> optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "TestDatabase");
            AppDbContext appDbContext = new AppDbContext(optionsBuilder.Options);
            var loggerMock = new Mock<ILogger<DepartmentsService>>();
            DepartmentsService departmentsService = new DepartmentsService(appDbContext, loggerMock.Object);
            Department department = new Department
            {
                Name = null,
                Description = "Handles recruitment and employee relations."
            };
             
            Assert.Throws<ArgumentException>(() => departmentsService.AddDepartment(department));
        }

        [Fact]
        public void Add_Department_Throws_Exception_When_Department_Is_Null()
        {
            // Arrange
            DbContextOptionsBuilder<AppDbContext> optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "TestDatabaseNullDept");
            AppDbContext appDbContext = new AppDbContext(optionsBuilder.Options);
            var loggerMock = new Mock<ILogger<DepartmentsService>>();
            DepartmentsService departmentsService = new DepartmentsService(appDbContext, loggerMock.Object);
            Department? department = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => departmentsService.AddDepartment(department!));
        }

        [Fact]
        public void Add_Department_Throws_Exception_When_Name_Is_Empty()
        {
            // Arrange
            DbContextOptionsBuilder<AppDbContext> optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "TestDatabaseEmptyName");
            AppDbContext appDbContext = new AppDbContext(optionsBuilder.Options);
            var loggerMock = new Mock<ILogger<DepartmentsService>>();
            DepartmentsService departmentsService = new DepartmentsService(appDbContext, loggerMock.Object);
            Department department = new Department
            {
                Name = "",
                Description = "Handles recruitment and employee relations."
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => departmentsService.AddDepartment(department));
        }

        [Fact]
        public void Add_Department_Throws_Exception_When_Name_Is_Whitespace()
        {
            // Arrange
            DbContextOptionsBuilder<AppDbContext> optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "TestDatabaseWhitespaceName");
            AppDbContext appDbContext = new AppDbContext(optionsBuilder.Options);
            var loggerMock = new Mock<ILogger<DepartmentsService>>();
            DepartmentsService departmentsService = new DepartmentsService(appDbContext, loggerMock.Object);
            Department department = new Department
            {
                Name = "   ",
                Description = "Handles recruitment and employee relations."
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => departmentsService.AddDepartment(department));
        }

        [Fact]
        public void Add_Department_Throws_Exception_When_Name_Exceeds_100_Characters()
        {
            // Arrange
            DbContextOptionsBuilder<AppDbContext> optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "TestDatabaseLongName");
            AppDbContext appDbContext = new AppDbContext(optionsBuilder.Options);
            var loggerMock = new Mock<ILogger<DepartmentsService>>();
            DepartmentsService departmentsService = new DepartmentsService(appDbContext, loggerMock.Object);
            Department department = new Department
            {
                Name = new string('A', 101), // 101 characters
                Description = "Valid description"
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => departmentsService.AddDepartment(department));
        }

        [Fact]
        public void Add_Department_Throws_Exception_When_Description_Exceeds_500_Characters()
        {
            // Arrange
            DbContextOptionsBuilder<AppDbContext> optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "TestDatabaseLongDesc");
            AppDbContext appDbContext = new AppDbContext(optionsBuilder.Options);
            var loggerMock = new Mock<ILogger<DepartmentsService>>();
            DepartmentsService departmentsService = new DepartmentsService(appDbContext, loggerMock.Object);
            Department department = new Department
            {
                Name = "Valid Department",
                Description = new string('B', 501) // 501 characters
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => departmentsService.AddDepartment(department));
        }

        [Fact]
        public void Add_Department_Throws_Exception_When_Duplicate_Name_Exists()
        {
            // Arrange
            DbContextOptionsBuilder<AppDbContext> optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "TestDatabaseDuplicate");
            AppDbContext appDbContext = new AppDbContext(optionsBuilder.Options);
            var loggerMock = new Mock<ILogger<DepartmentsService>>();
            DepartmentsService departmentsService = new DepartmentsService(appDbContext, loggerMock.Object);
            
            // Add first department
            Department firstDepartment = new Department
            {
                Name = "Finance",
                Description = "Financial operations"
            };
            departmentsService.AddDepartment(firstDepartment);
            
            // Try to add duplicate department
            Department duplicateDepartment = new Department
            {
                Name = "Finance",
                Description = "Another finance department"
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => departmentsService.AddDepartment(duplicateDepartment));
        }

        [Fact]
        public void Add_Department_Success_With_Name_At_Maximum_Length()
        {
            // Arrange
            DbContextOptionsBuilder<AppDbContext> optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "TestDatabaseMaxName");
            AppDbContext appDbContext = new AppDbContext(optionsBuilder.Options);
            var loggerMock = new Mock<ILogger<DepartmentsService>>();
            DepartmentsService departmentsService = new DepartmentsService(appDbContext, loggerMock.Object);
            Department department = new Department
            {
                Name = new string('C', 100), // Exactly 100 characters
                Description = "Valid description"
            };

            // Act
            departmentsService.AddDepartment(department);

            // Assert
            var addedDepartment = appDbContext.Departments.FirstOrDefault(d => d.Name == department.Name);
            Assert.NotNull(addedDepartment);
            Assert.Equal(department.Name, addedDepartment.Name);
        }

        [Fact]
        public void Add_Department_Success_With_Description_At_Maximum_Length()
        {
            // Arrange
            DbContextOptionsBuilder<AppDbContext> optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "TestDatabaseMaxDesc");
            AppDbContext appDbContext = new AppDbContext(optionsBuilder.Options);
            var loggerMock = new Mock<ILogger<DepartmentsService>>();
            DepartmentsService departmentsService = new DepartmentsService(appDbContext, loggerMock.Object);
            Department department = new Department
            {
                Name = "Valid Department Name",
                Description = new string('D', 500) // Exactly 500 characters
            };

            // Act
            departmentsService.AddDepartment(department);

            // Assert
            var addedDepartment = appDbContext.Departments.FirstOrDefault(d => d.Name == department.Name);
            Assert.NotNull(addedDepartment);
            Assert.Equal(department.Description, addedDepartment.Description);
        }

        [Fact]
        public void Add_Department_Success_With_Null_Description()
        {
            // Arrange
            DbContextOptionsBuilder<AppDbContext> optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "TestDatabaseNullDesc");
            AppDbContext appDbContext = new AppDbContext(optionsBuilder.Options);
            var loggerMock = new Mock<ILogger<DepartmentsService>>();
            DepartmentsService departmentsService = new DepartmentsService(appDbContext, loggerMock.Object);
            Department department = new Department
            {
                Name = "Department Without Description",
                Description = null!
            };

            // Act
            departmentsService.AddDepartment(department);

            // Assert
            var addedDepartment = appDbContext.Departments.FirstOrDefault(d => d.Name == department.Name);
            Assert.NotNull(addedDepartment);
            Assert.Equal(department.Name, addedDepartment.Name);
            Assert.Null(addedDepartment.Description);
        }

        [Fact]
        public void Add_Department_Success_With_Empty_Description()
        {
            // Arrange
            DbContextOptionsBuilder<AppDbContext> optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "TestDatabaseEmptyDesc");
            AppDbContext appDbContext = new AppDbContext(optionsBuilder.Options);
            var loggerMock = new Mock<ILogger<DepartmentsService>>();
            DepartmentsService departmentsService = new DepartmentsService(appDbContext, loggerMock.Object);
            Department department = new Department
            {
                Name = "Department With Empty Description",
                Description = ""
            };

            // Act
            departmentsService.AddDepartment(department);

            // Assert
            var addedDepartment = appDbContext.Departments.FirstOrDefault(d => d.Name == department.Name);
            Assert.NotNull(addedDepartment);
            Assert.Equal(department.Name, addedDepartment.Name);
            Assert.Equal("", addedDepartment.Description);
        }
    }
}
