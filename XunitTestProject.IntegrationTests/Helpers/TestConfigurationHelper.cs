using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace XunitTestProject.IntegrationTests.Helpers
{
    /// <summary>
    /// Helper class for integration test configuration
    /// </summary>
    public static class TestConfigurationHelper
    {
        /// <summary>
        /// Creates a test configuration with LocalDB connection string
        /// </summary>
        /// <param name="databaseName">Name of the test database</param>
        /// <returns>IConfiguration instance</returns>
        public static IConfiguration CreateTestConfiguration(string databaseName = "IntegrationTestDb")
        {
            var configBuilder = new ConfigurationBuilder();
            
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = $"Server=(localdb)\\mssqllocaldb;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true",
                ["Logging:LogLevel:Default"] = "Information",
                ["Logging:LogLevel:Microsoft.EntityFrameworkCore"] = "Warning"
            });

            return configBuilder.Build();
        }

        /// <summary>
        /// Configures services for integration testing
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="connectionString">Database connection string</param>
        public static void ConfigureTestServices(IServiceCollection services, string connectionString)
        {
            // Configure logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // Add other services as needed for integration testing
            // For example, you might add services that the DepartmentsService depends on
        }

        /// <summary>
        /// Generates a unique database name for test isolation
        /// </summary>
        /// <param name="testClassName">Name of the test class</param>
        /// <param name="testMethodName">Name of the test method</param>
        /// <returns>Unique database name</returns>
        public static string GenerateUniqueDatabaseName(string testClassName, string testMethodName)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var guid = Guid.NewGuid().ToString("N")[..8];
            return $"TestDb_{testClassName}_{testMethodName}_{timestamp}_{guid}";
        }
    }
}