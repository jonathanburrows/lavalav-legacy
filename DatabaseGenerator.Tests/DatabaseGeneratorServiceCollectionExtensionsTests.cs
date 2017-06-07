using lvl.DatabaseGenerator.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace lvl.DatabaseGenerator.Tests
{
    [Collection(nameof(DatabaseGeneratorCollection))]
    public class DatabaseGeneratorServiceCollectionExtensionsTests
    {
        private IServiceProvider Services { get; }

        public DatabaseGeneratorServiceCollectionExtensionsTests(InMemoryDatabaseGenerationFixture inMemoryDatabaseGenerationFixture)
        {
            Services = inMemoryDatabaseGenerationFixture.ServiceProvider ?? throw new ArgumentNullException(nameof(inMemoryDatabaseGenerationFixture));
        }

        [Fact]
        public void It_will_allow_database_creator_to_be_resolved()
        {
            var databaseCreator = Services.GetRequiredService<DatabaseCreator>();
            Assert.NotNull(databaseCreator);
        }

        [Fact]
        public void It_will_allow_database_migrator_to_be_resolved()
        {
            var databaseMigrator = Services.GetRequiredService<DatabaseMigrator>();
            Assert.NotNull(databaseMigrator);
        }

        [Fact]
        public void It_will_allow_script_runner_to_be_resolved()
        {
            var scriptRunner = Services.GetRequiredService<ScriptRunner>();
            Assert.NotNull(scriptRunner);
        }

        [Fact]
        public void It_will_allow_database_generation_options_to_be_resolved()
        {
            var scriptRunner = Services.GetRequiredService<DatabaseGenerationOptions>();
            Assert.NotNull(scriptRunner);
        }

        [Fact]
        public void It_will_allow_database_generation_runner_to_be_resolved()
        {
            var scriptRunner = Services.GetRequiredService<DatabaseGenerationRunner>();
            Assert.NotNull(scriptRunner);
        }
    }
}
