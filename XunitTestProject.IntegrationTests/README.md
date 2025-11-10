# Integration Tests for UnitTestSamples

This project demonstrates comprehensive integration testing for the `DepartmentsService` using **LocalDB** and **Entity Framework Core** with **SQL Server**, following Microsoft's recommended testing patterns.

## Overview

The integration tests verify the complete interaction between:
- **DepartmentsService** (business logic)
- **Entity Framework Core** (ORM)
- **SQL Server LocalDB** (database)

## Project Structure

```
XunitTestProject.IntegrationTests/
├── Base/
│   └── IntegrationTestBase.cs          # Base class for integration tests
├── Collections/
│   └── DatabaseCollection.cs          # xUnit collection definition
├── Fixtures/
│   └── DatabaseFixture.cs             # Database fixture with LocalDB setup
├── Helpers/
│   └── TestConfigurationHelper.cs     # Configuration utilities
├── DepartmentsServiceIntegrationTests.cs  # Main integration tests
└── DatabaseConnectionTests.cs         # Database connection verification tests
```

## Key Features

### 🏗️ **Database Fixture Pattern**
- Creates unique LocalDB database for each test run
- Automatic database creation and cleanup
- Migration support with EF Core
- Proper dependency injection setup

### 🔧 **Test Isolation**
- Each test collection uses its own database instance
- Clean database state before each test
- Proper resource disposal and cleanup

### 📊 **Comprehensive Test Coverage**
- **CRUD Operations**: Add, Update, Delete, Get operations
- **Validation Testing**: All business rule validations
- **Edge Cases**: Boundary conditions and error scenarios
- **Database Constraints**: SQL Server specific validations
- **Transaction Behavior**: Rollback and commit scenarios

## Configuration

### Dependencies
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.10" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="9.0.10" />
<PackageReference Include="Microsoft.Extensions.Logging" Version="9.0.10" />
<PackageReference Include="Moq" Version="4.20.72" />
```

### LocalDB Connection
```csharp
Server=(localdb)\\mssqllocaldb;Database={TestDbName};Trusted_Connection=True;MultipleActiveResultSets=true
```

## Usage Examples

### Basic Integration Test
```csharp
[Collection("Database collection")]
public class MyIntegrationTests : IntegrationTestBase
{
    public MyIntegrationTests(DatabaseFixture databaseFixture) 
        : base(databaseFixture) { }

    [Fact]
    public void MyTest_ShouldWork()
    {
        // Arrange
        var department = CreateTestDepartment("Test Dept");

        // Act
        _departmentsService.AddDepartment(department);

        // Assert
        AssertDepartmentCount(1);
        AssertDepartmentExists("Test Dept");
    }
}
```

### Database Seeding
```csharp
[Fact]
public void TestWithSeedData()
{
    // Arrange
    SeedTestData(); // Seeds 3 departments

    // Act
    var departments = _departmentsService.GetAllDepartments();

    // Assert
    Assert.Equal(3, departments.Count);
}
```

## Test Categories

### ✅ **AddDepartment Tests**
- Valid data persistence
- Duplicate name validation
- Input validation (null, empty, length)
- Transaction rollback verification

### ✅ **UpdateDepartment Tests**
- Data modification persistence
- Duplicate name prevention during updates
- Validation rule enforcement

### ✅ **DeleteDepartment Tests**
- Record removal verification
- Cascading delete behavior (if applicable)

### ✅ **Query Tests**
- GetAll, GetById, GetByName operations
- Search functionality
- Case sensitivity behavior
- Empty result handling

### ✅ **Database Constraint Tests**
- SQL Server column length limits
- Primary key constraints
- Index behavior verification

### ✅ **Concurrency Tests**
- Multiple simultaneous operations
- Database locking behavior

## Running Tests

### Run All Integration Tests
```bash
dotnet test XunitTestProject.IntegrationTests
```

### Run Specific Test Category
```bash
# Test only AddDepartment functionality
dotnet test --filter "FullyQualifiedName~AddDepartment"

# Test database connection
dotnet test --filter "FullyQualifiedName~DatabaseConnectionTests"
```

### Run with Detailed Output
```bash
dotnet test --logger "console;verbosity=normal"
```

## Prerequisites

1. **SQL Server LocalDB** must be installed
   - Included with Visual Studio
   - Available as standalone download
   - Verify with: `sqllocaldb info`

2. **.NET 9.0** SDK

3. **Entity Framework Tools** (if running migrations manually)
   ```bash
   dotnet tool install --global dotnet-ef
   ```

## Best Practices Demonstrated

### 🎯 **Microsoft's Recommended Patterns**
- Database fixtures for test isolation
- Proper dependency injection setup
- Configuration management for tests
- Resource cleanup and disposal

### 🔒 **Test Reliability**
- Unique database names prevent conflicts
- Proper transaction handling
- Deterministic test execution
- Clear test documentation

### 📈 **Performance Optimization**
- Shared fixtures reduce setup overhead
- Efficient database creation/cleanup
- Minimal test data requirements

## Troubleshooting

### LocalDB Issues
```bash
# Check LocalDB instances
sqllocaldb info

# Stop/Start LocalDB
sqllocaldb stop MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
```

### Test Isolation Problems
- Ensure unique database names in fixtures
- Verify proper disposal in test cleanup
- Check for hanging database connections

### Migration Issues
- Verify migrations are up to date
- Check EF Core configuration in tests
- Ensure test project references main project

## Benefits of This Approach

✅ **Real Database Testing**: Uses actual SQL Server instead of in-memory providers  
✅ **Migration Verification**: Tests work with actual EF migrations  
✅ **SQL Server Features**: Tests database-specific functionality  
✅ **Performance Reality**: Realistic performance characteristics  
✅ **Production Parity**: Close to production database behavior  
✅ **Easy Debugging**: Can inspect actual database during test development  

## Future Enhancements

- [ ] Docker SQL Server container option
- [ ] Test data builders for complex scenarios
- [ ] Performance benchmarking integration
- [ ] Automated test data cleanup strategies
- [ ] Multi-database provider testing