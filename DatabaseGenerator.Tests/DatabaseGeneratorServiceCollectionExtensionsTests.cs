using lvl.DatabaseGenerator.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace lvl.DatabaseGenerator.Tests
{
    [Collection(DatabaseGeneratorCollection.Name)]
    public class DatabaseGeneratorServiceCollectionExtensionsTests
    {
        private IServiceProvider Services { get; }

        public DatabaseGeneratorServiceCollectionExtensionsTests(InMemoryDatabaseGenerationFixture inMemoryDatabaseGenerationFixture)
        {
            Services = inMemoryDatabaseGenerationFixture.ServiceProvider ?? throw new ArgumentNullException(nameof(inMemoryDatabaseGenerationFixture));
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
        public void AfterAddingDatabaseGeneration_ResolvingScriptRunner_ReturnsValue()
        {
            var scriptRunner = Services.GetRequiredService<ScriptRunner>();
            Assert.NotNull(scriptRunner);
        }

        [Fact]
        public void AfterAddingDatabaseGeneration_ResolvingDatabaseGenerationOptions_ReturnsValue()
        {
            var scriptRunner = Services.GetRequiredService<DatabaseGenerationOptions>();
            Assert.NotNull(scriptRunner);
        }

        [Fact]
        public void AfterAddingDatabaseGeneration_ResolvingDatabaseGenerationRunner_ReturnsValue()
        {
            var scriptRunner = Services.GetRequiredService<DatabaseGenerationRunner>();
            Assert.NotNull(scriptRunner);
        }
    }
}
