using lvl.Web.Tests.Fixtures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Xunit;

namespace lvl.Web.Tests
{
    [Collection(WebCollection.Name)]
    public class LoggingOptionsTests
    {
        [Fact]
        public void It_will_have_parameterless_constructor()
        {
            var parameterlessConstructor = typeof(LoggingOptions).GetConstructor(Type.EmptyTypes);
            Assert.NotNull(parameterlessConstructor);
        }

        [Fact]
        public void It_has_constructor_with_iconfiguration_parameter()
        {
            var configurationConstructor = typeof(LoggingOptions).GetConstructor(new[] { typeof(IConfiguration) });
            Assert.NotNull(configurationConstructor);
        }

        [Fact]
        public void It_throws_argument_null_exception_when_configuration_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new LoggingOptions(null));
        }

        [Fact]
        public void It_successfully_constructs_object_when_database_generation_section_is_missing()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();

            var options = new LoggingOptions(configuration);

            Assert.NotNull(options);
        }

        [Fact]
        public void It_defaults_log_level_to_information()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();

            var options = new LoggingOptions(configuration);

            Assert.Equal(options.LogLevel, LogLevel.Information);
        }

        [Theory]
        [InlineData(LogLevel.Trace)]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.Warning)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Critical)]
        [InlineData(LogLevel.None)]
        public void It_binds_to_log_level_enum_values(LogLevel logLevel)
        {
            var configurationOptions = new Dictionary<string, string>
            {
                [$"logging:{nameof(LoggingOptions.LogLevel)}"] = ((int)logLevel).ToString()
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new LoggingOptions(configuration);

            Assert.Equal(options.LogLevel, logLevel);
        }

        [Theory]
        [InlineData(LogLevel.Trace)]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.Warning)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Critical)]
        [InlineData(LogLevel.None)]
        public void It_binds_to_log_level_strings(LogLevel logLevel)
        {
            var configurationOptions = new Dictionary<string, string>
            {
                [$"logging:{nameof(LoggingOptions.LogLevel)}"] = logLevel.ToString()
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new LoggingOptions(configuration);

            Assert.Equal(options.LogLevel, logLevel);
        }
    }
}