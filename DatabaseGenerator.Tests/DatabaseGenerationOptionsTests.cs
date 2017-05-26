using lvl.DatabaseGenerator.Tests.Fixtures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;

namespace lvl.DatabaseGenerator.Tests
{
    [Collection(nameof(DatabaseGeneratorCollection))]
    public class DatabaseGenerationOptionsTests
    {
        [Fact]
        public void It_will_have_parameterless_constructor()
        {
            var parameterlessConstructor = typeof(DatabaseGenerationOptions).GetConstructor(Type.EmptyTypes);
            Assert.NotNull(parameterlessConstructor);
        }

        [Fact]
        public void It_has_constructor_with_iconfiguration_parameter()
        {
            var configurationConstructor = typeof(DatabaseGenerationOptions).GetConstructor(new[] { typeof(IConfiguration) });
            Assert.NotNull(configurationConstructor);
        }

        [Fact]
        public void It_throws_argument_null_exception_when_configuration_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new DatabaseGenerationOptions(null));
        }

        [Fact]
        public void It_successfully_constructs_object_when_database_generation_section_is_missing()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();

            // ReSharper disable once UnusedVariable for unit test.
            var options = new DatabaseGenerationOptions(configuration);
        }

        [Fact]
        public void It_sets_assembly_path_when_present()
        {
            var assemblyPath = "./assembly-path";
            var configurationOptions = new Dictionary<string, string>
            {
                [$"database-generation:{nameof(DatabaseGenerationOptions.AssemblyPath)}"] = assemblyPath
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new DatabaseGenerationOptions(configuration);

            Assert.Equal(options.AssemblyPath, assemblyPath);
        }

        [Fact]
        public void It_sets_pre_generation_script_bin_when_present()
        {
            var preGenerationScriptBin = "./scripts/pre-generation";
            var configurationOptions = new Dictionary<string, string>
            {
                [$"database-generation:{nameof(DatabaseGenerationOptions.PreGenerationScriptBin)}"] = preGenerationScriptBin
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new DatabaseGenerationOptions(configuration);

            Assert.Equal(options.PreGenerationScriptBin, preGenerationScriptBin);
        }

        [Fact]
        public void It_sets_post_generation_script_bin_when_present()
        {
            var postGenerationScriptBin = "./scripts/post-generation";
            var configurationOptions = new Dictionary<string, string>
            {
                [$"database-generation:{nameof(DatabaseGenerationOptions.PostGenerationScriptBin)}"] = postGenerationScriptBin
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new DatabaseGenerationOptions(configuration);

            Assert.Equal(options.PostGenerationScriptBin, postGenerationScriptBin);
        }

        [Fact]
        public void It_sets_migrate_to_false_if_not_present()
        {
            var configuration = new ConfigurationBuilder().Build();

            var options = new DatabaseGenerationOptions(configuration);

            Assert.False(options.Migrate);
        }

        [Fact]
        public void It_sets_migrate_to_false()
        {
            var configurationOptions = new Dictionary<string, string>
            {
                [$"database-generation:{nameof(DatabaseGenerationOptions.Migrate)}"] = false.ToString()
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new DatabaseGenerationOptions(configuration);

            Assert.False(options.Migrate);
        }

        [Fact]
        public void It_sets_migrate_to_true()
        {
            var configurationOptions = new Dictionary<string, string>
            {
                [$"database-generation:{nameof(DatabaseGenerationOptions.Migrate)}"] = true.ToString()
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new DatabaseGenerationOptions(configuration);

            Assert.True(options.Migrate);
        }

        [Fact]
        public void It_sets_dry_run_to_false_if_not_present()
        {
            var configuration = new ConfigurationBuilder().Build();

            var options = new DatabaseGenerationOptions(configuration);

            Assert.False(options.DryRun);
        }

        [Fact]
        public void It_sets_dry_run_to_false()
        {
            var configurationOptions = new Dictionary<string, string>
            {
                [$"database-generation:{nameof(DatabaseGenerationOptions.DryRun)}"] = false.ToString()
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new DatabaseGenerationOptions(configuration);

            Assert.False(options.DryRun);
        }

        [Fact]
        public void It_sets_dry_run_to_true()
        {
            var configurationOptions = new Dictionary<string, string>
            {
                [$"database-generation:{nameof(DatabaseGenerationOptions.DryRun)}"] = true.ToString()
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new DatabaseGenerationOptions(configuration);

            Assert.True(options.DryRun);
        }
    }
}
