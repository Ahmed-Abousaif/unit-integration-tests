using XunitTestProject.IntegrationTests.Fixtures;

namespace XunitTestProject.IntegrationTests.Collections
{
    /// <summary>
    /// Collection definition for database fixture
    /// Ensures that tests within the same collection share the same database fixture instance
    /// but tests in different collections get their own instances
    /// </summary>
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
        // This class is empty. It's just used to define the collection.
        // The ICollectionFixture<DatabaseFixture> interface tells xUnit
        // that this collection uses the DatabaseFixture.
    }
}