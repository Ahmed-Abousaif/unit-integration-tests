using XunitTestProject.IntegrationTests.Fixtures;

namespace XunitTestProject.IntegrationTests.Collections
{
    /// <summary>
    /// Collection definition for tests using Testcontainers with PostgreSQL
    /// All tests in this collection will share the same PostgreSQL container instance
    /// </summary>
    [CollectionDefinition("Database postgres container collection")]
    public class DatabasePostgresContainerCollection : ICollectionFixture<DatabasePostgresContainerFixture>
    {
        // This class is intentionally empty.
        // It is used to define the collection name and specify the fixture type.
    }
}

