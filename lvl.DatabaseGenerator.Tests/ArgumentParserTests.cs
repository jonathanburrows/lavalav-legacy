using System;
using Xunit;

namespace lvl.DatabaseGenerator.Tests
{
    public class ArgumentParserTests : IClassFixture<ArgumentParser>
    {
        private ArgumentParser ArgumentParser { get; }

        public ArgumentParserTests(ArgumentParser argumentParser)
        {
            ArgumentParser = argumentParser;
        }

        [Theory]
        [InlineData("--connection-string hello --assembly-path world --migrate")]
        [InlineData("--assembly-path world --connection-string hello --migrate")]
        [InlineData("--migrate --assembly-path world --connection-string hello")]
        public void WhenParsing_ConnectionStringIsPopulated_RegardlessOfPosition(string argumentLine)
        {
            var args = argumentLine.Split(' ');

            var options = ArgumentParser.Parse(args);

            Assert.NotNull(options.ConnectionString);
        }

        [Fact]
        public void WhenParsing_AndConnectionStringIsMissing_ThrowsArgumentException()
        {
            var argumentLine = "--assembly-path world --migrate";
            var args = argumentLine.Split(' ');

            Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(args));
        }

        [Fact]
        public void WhenParsing_AndConnectionStringHasSwitchWithNoValue_ThrowsArgumentException()
        {
            var argumentLine = "--assembly-path world --connection-string --migrate";
            var args = argumentLine.Split(' ');

            Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(args));
        }

        [Fact]
        public void WhenPrasing_AndConnectionStringSwitchIsLast_ThrowsArgumentException()
        {
            var argumentLine = "--assembly-path world --migrate --connection-string";
            var args = argumentLine.Split(' ');

            Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(args));
        }

        [Theory]
        [InlineData("--assembly-path world --connection-string hello --migrate")]
        [InlineData("--migrate --assembly-path world --connection-string hello")]
        [InlineData("--connection-string hello --migrate --assembly-path world")]
        public void WhenParsing_AssemblyPathIsPopulated_RegardlessOfPosition(string argumentLine)
        {
            var args = argumentLine.Split(' ');

            var options = ArgumentParser.Parse(args);

            Assert.NotNull(options.AssemblyPath);
        }

        [Fact]
        public void WhenParsing_AndAssemblyPathIsMissing_ThrowsArgumentException()
        {
            var argumentLine = "--migrate --connection-string hello";
            var args = argumentLine.Split(' ');

            Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(args));
        }

        [Fact]
        public void WhenParsing_AndAssemblyPathHasSwitchWithNoValue_ThrowsArgumentException()
        {
            var argumentLine = "--migrate --assembly-path --connection-string hello";
            var args = argumentLine.Split(' ');

            Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(args));
        }

        [Fact]
        public void WhenParsing_AndAssemblyPathSwitchIsLast_ThrowsArgumentException()
        {
            var argumentLine = "--migrate --connection-string hello --assembly-path";
            var args = argumentLine.Split(' ');

            Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(args));
        }

        [Theory]
        [InlineData("--pre-generation-script-bin world --connection-string hello --assembly-path world")]
        [InlineData("--assembly-path hello --pre-generation-script-bin world --connection-string hello")]
        [InlineData("--connection-string hello --assembly-path world --pre-generation-script-bin world")]
        public void WhenParsing_PreGenerationIsPopulated_RegardlessOfPosition(string argumentLine)
        {
            var args = argumentLine.Split(' ');

            var options = ArgumentParser.Parse(args);

            Assert.NotNull(options.PreGenerationScriptBin);
        }

        [Fact]
        public void WhenParsing_AndPreGenerationIsntProvided_OptionsAreStillReturned()
        {
            var argumentLine = "--migrate --connection-string hello --assembly-path world";
            var args = argumentLine.Split(' ');

            var options = ArgumentParser.Parse(args);

            Assert.Null(options.PreGenerationScriptBin);
        }

        [Fact]
        public void WhenParsing_AndPreGenerationHasSwitchWithNoValue_ArgumentExceptionIsThrown()
        {
            var argumentLine = "--migrate --pre-generation-script-bin --connection-string hello --assembly-path world";
            var args = argumentLine.Split(' ');

            Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(args));
        }

        [Fact]
        public void WhenParsing_AndPreGenerationSwitchIsLast_ArgumentExceptionIsThrown()
        {
            var argumentLine = "--migrate --connection-string hello --pre-generation-script-bin --assembly-path world";
            var args = argumentLine.Split(' ');

            Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(args));
        }

        [Theory]
        [InlineData("--post-generation-script-bin world --connection-string hello --assembly-path hello")]
        [InlineData("--assembly-path hello --post-generation-script-bin world --connection-string hello")]
        [InlineData("--connection-string hello --assembly-path world --post-generation-script-bin world")]
        public void WhenParsing_PostGenerationIsPopulated_RegardlessOfPosition(string argumentLine)
        {
            var args = argumentLine.Split(' ');

            var options = ArgumentParser.Parse(args);

            Assert.NotNull(options.PostGenerationScriptBin);
        }

        [Fact]
        public void WhenParsing_AndPostGenerationIsntProvided_OptionsAreStillReturned()
        {
            var argumentLine = "--migrate --connection-string hello --assembly-path world";
            var args = argumentLine.Split(' ');

            var options = ArgumentParser.Parse(args);

            Assert.Null(options.PostGenerationScriptBin);
        }

        [Fact]
        public void WhenParsing_AndPostGenerationHasSwitchWithNoValue_ArgumentExceptionIsThrown()
        {
            var argumentLine = "--migrate --post-generation-script-bin --connection-string hello --assembly-path world";
            var args = argumentLine.Split(' ');

            Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(args));
        }

        [Fact]
        public void WhenParsing_AndPostGenerationSwitchIsLast_ArgumentExceptionIsThrown()
        {
            var argumentLine = "--migrate --connection-string hello --assembly-path world --post-generation-script-bin";
            var args = argumentLine.Split(' ');

            Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(args));
        }

        [Theory]
        [InlineData("--migrate --connection-string hello --assembly-path world")]
        [InlineData("--assembly-path world --migrate --connection-string hello")]
        [InlineData("--connection-string hello --assembly-path world --migrate")]
        public void WhenParsing_MigrateIsFlaged_RegardlessOfPosition(string argumentLine)
        {
            var args = argumentLine.Split(' ');

            var options = ArgumentParser.Parse(args);

            Assert.True(options.Migrate);
        }

        [Fact]
        public void WhenParsing_AndMigrateIsntProvided_OptionsAreStillReturned()
        {
            var argumentLine = "--connection-string hello --assembly-path world";
            var args = argumentLine.Split(' ');

            var options = ArgumentParser.Parse(args);

            Assert.False(options.Migrate);
        }

        [Theory]
        [InlineData("--dry-run --connection-string hello --assembly-path world")]
        [InlineData("--assembly-path world --dry-run --connection-string hello")]
        [InlineData("--connection-string hello --assembly-path world --dry-run")]
        public void WhenParsing_DryRunIsFlaged_RegardlessOfPosition(string argumentLine)
        {
            var args = argumentLine.Split(' ');

            var options = ArgumentParser.Parse(args);

            Assert.True(options.DryRun);
        }

        [Fact]
        public void WhenParsing_AndDryRunIsntProvided_OptionsAreStillReturned()
        {
            var argumentLine = "--connection-string hello --assembly-path world";
            var args = argumentLine.Split(' ');

            var options = ArgumentParser.Parse(args);

            Assert.False(options.DryRun);
        }

        [Fact]
        public void WhenParsing_AndArgumentsAreNull_ArgumentNullExceptionIsThrown()
        {
            Assert.Throws<ArgumentNullException>(() => ArgumentParser.Parse(null));
        }
    }
}
