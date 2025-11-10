using XunitTestProject.IntegrationTests.Fixtures;

namespace XunitTestProject.IntegrationTests.Collections
{
    /// <summary>
    /// Collection definition for tests using Testcontainers
    /// All tests in this collection will share the same container instance
    /// </summary>
    [CollectionDefinition("Database container collection")]
    public class DatabaseContainerCollection : ICollectionFixture<DatabaseContainerFixture>
    {
        // This class is intentionally empty.
        // It is used to define the collection name and specify the fixture type.
    }
}

