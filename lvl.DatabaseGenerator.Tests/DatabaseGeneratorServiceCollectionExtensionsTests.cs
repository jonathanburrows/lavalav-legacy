using lvl.DatabaseGenerator.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace lvl.DatabaseGenerator.Tests
{
    public class DatabaseGeneratorServiceCollectionExtensionsTests : IClassFixture<InMemoryDatabaseGenerationFixture>
    {
        private IServiceProvider Services { get; }

        public DatabaseGeneratorServiceCollectionExtensionsTests(InMemoryDatabaseGenerationFixture inMemoryDatabaseGenerationFixture)
        {
            Services = inMemoryDatabaseGenerationFixture.ServiceProvider;
        }

        [Fact]
        public void AfterAddingDatabaseGeneration_ResolvingDatabaseCreator_ReturnsValue()
        {
            var databaseCreator = Services.GetRequiredService<DatabaseCreator>();
            Assert.NotNull(databaseCreator);
        }

        [Fact]
        public void AfterAddingDatabaseGeneration_ResolvingDatabaseMigrator_ReturnsValue()
        {
            var databaseMigrator = Services.GetRequiredService<DatabaseMigrator>();
            Assert.NotNull(databaseMigrator);
        }

        [Fact]
        public void AfterAddingDatabaseGeneration_ResolvingScriptRunner_ReturnsValue() {
            var scriptRunner = Services.GetRequiredService<ScriptRunner>();
            Assert.NotNull(scriptRunner);
        }
    }
}
