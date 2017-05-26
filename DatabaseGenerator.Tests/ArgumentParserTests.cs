using lvl.DatabaseGenerator.Tests.Fixtures;
using System;
using Xunit;

namespace lvl.DatabaseGenerator.Tests
{
    [Collection(nameof(DatabaseGeneratorCollection))]
    public class ArgumentParserTests
    {
        private ArgumentParser ArgumentParser { get; }

        public ArgumentParserTests(ArgumentParser argumentParser)
        {
            ArgumentParser = argumentParser ?? throw new ArgumentNullException(nameof(argumentParser));
        }

        [Fact]
        public void Has_flag_will_throw_argument_null_exception_when_args_are_null()
        {
            Assert.Throws<ArgumentNullException>(() => ArgumentParser.HasFlag(null, ""));
        }

        [Fact]
        public void Has_flag_will_throw_argument_null_exception_when_flag_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => ArgumentParser.HasFlag(new string[0], null));
        }

        [Fact]
        public void Has_flag_will_throw_invalid_operation_exception_when_flag_doesnt_start_with_dashes()
        {
            Assert.Throws<InvalidOperationException>(() => ArgumentParser.HasFlag(new string[0], "target"));
        }

        [Theory]
        [InlineData("--target --unimportant --irrelevant thing")]
        [InlineData("--unimportant --target --irrelevant thing")]
        [InlineData("--unimportant --irrelevant thing --target")]
        public void Has_flag_will_return_true_if_flag_is_found_regardless_of_order(string arg)
        {
            var args = arg.Split(' ');

            Assert.True(ArgumentParser.HasFlag(args, "--target"));
        }

        [Fact]
        public void Has_flag_will_return_false_if_flag_is_not_found()
        {
            var args = new[] { "--irrelevant" };

            Assert.False(ArgumentParser.HasFlag(args, "--target"));
        }

        [Fact]
        public void Has_flag_is_case_insensitive()
        {
            var args = new[] { "--tArGeT" };

            Assert.True(ArgumentParser.HasFlag(args, "--target"));
        }

        [Fact]
        public void Get_optional_will_throw_argument_null_exception_when_args_are_null()
        {
            Assert.Throws<ArgumentNullException>(() => ArgumentParser.GetOptional<string>(null, ""));
        }

        [Fact]
        public void Get_optional_will_throw_argument_null_exception_when_key_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => ArgumentParser.GetOptional<string>(new string[0], null));
        }

        [Fact]
        public void Get_optional_will_throw_invalid_operation_exception_when_next_value_is_key()
        {
            var args = new[] { "--target", "--next" };

            Assert.Throws<InvalidOperationException>(() => ArgumentParser.GetOptional<string>(args, "--target"));
        }

        [Fact]
        public void Get_optional_will_throw_invalid_operation_exception_when_key_is_last()
        {
            var args = new[] { "--first", "--target" };

            Assert.Throws<InvalidOperationException>(() => ArgumentParser.GetOptional<string>(args, "--target"));
        }

        [Fact]
        public void Get_optional_will_throw_invalid_operation_exception_when_key_doesnt_start_with_dashes()
        {
            Assert.Throws<InvalidOperationException>(() => ArgumentParser.GetOptional<string>(new string[0], "target"));
        }

        [Theory]
        [InlineData("--important value --unimportant --irrelevant thing")]
        [InlineData("--unimportant --important value --irrelevant thing")]
        [InlineData("--unimportant --irrelevant thing --important value")]
        public void Get_optional_will_return_value_regardless_of_order(string arg)
        {
            var args = arg.Split(' ');

            var value = ArgumentParser.GetOptional<string>(args, "--important");

            Assert.Equal(value, "value");
        }

        [Fact]
        public void Get_optional_is_case_insensitive()
        {
            var args = new[] { "--TarGet", "value" };

            var value = ArgumentParser.GetOptional<string>(args, "--target");

            Assert.Equal(value, "value");
        }

        [Fact]
        public void Get_optional_will_return_default_if_key_not_found()
        {
            var args = new string[0];

            var value = ArgumentParser.GetOptional<string>(args, "--non-existant");

            Assert.Null(value);
        }

        [Fact]
        public void Get_optional_can_parse_numbers()
        {
            var args = new[] { "--target", "1" };

            var value = ArgumentParser.GetOptional<int>(args, "--target");

            Assert.Equal(value, 1);
        }

        [Fact]
        public void Get_optional_can_parse_nullable_numbers()
        {
            var args = new[] { "--target", "1" };

            var value = ArgumentParser.GetOptional<int?>(args, "--target");

            Assert.Equal(value, 1);
        }

        [Fact]
        public void Get_optional_will_parse_booleans()
        {
            var args = new[] { "--target", "true" };

            var value = ArgumentParser.GetOptional<bool>(args, "--target");

            Assert.Equal(value, true);
        }

        [Fact]
        public void Get_required_will_throw_argument_null_exception_when_args_are_null()
        {
            Assert.Throws<ArgumentNullException>(() => ArgumentParser.GetRequired<string>(null, ""));
        }

        [Fact]
        public void Get_required_will_throw_argument_null_exception_when_key_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => ArgumentParser.GetRequired<string>(new string[0], null));
        }

        [Fact]
        public void Get_required_will_throw_invalid_operation_exception_when_next_value_is_key()
        {
            var args = new[] { "--target", "--next" };

            Assert.Throws<InvalidOperationException>(() => ArgumentParser.GetRequired<string>(args, "--target"));
        }

        [Fact]
        public void Get_required_will_throw_invalid_operation_exception_when_key_is_last()
        {
            var args = new[] { "--first", "--target" };

            Assert.Throws<InvalidOperationException>(() => ArgumentParser.GetRequired<string>(args, "--target"));
        }

        [Fact]
        public void Get_required_will_throw_invalid_operation_exception_when_key_is_not_found()
        {
            var args = new[] { "--first", "--target" };

            Assert.Throws<InvalidOperationException>(() => ArgumentParser.GetRequired<string>(args, "--target"));
        }

        [Fact]
        public void Get_required_will_throw_invalid_operation_exception_when_key_does_not_start_with_dashes()
        {
            var args = new[] { "--target", "target" };

            Assert.Throws<InvalidOperationException>(() => ArgumentParser.GetRequired<string>(args, "target"));
        }

        [Theory]
        [InlineData("--important value --unimportant --irrelevant thing")]
        [InlineData("--unimportant --important value --irrelevant thing")]
        [InlineData("--unimportant --irrelevant thing --important value")]
        public void Get_required_will_return_value_regardless_of_order(string arg)
        {
            var args = arg.Split(' ');

            var value = ArgumentParser.GetOptional<string>(args, "--important");

            Assert.Equal(value, "value");
        }

        [Fact]
        public void Get_required_is_case_insensitive()
        {
            var args = new[] { "--TarGet", "value" };

            var value = ArgumentParser.GetRequired<string>(args, "--target");

            Assert.Equal(value, "value");
        }

        [Fact]
        public void Get_required_will_return_default_if_key_not_found()
        {
            var args = new string[0];

            var value = ArgumentParser.GetOptional<string>(args, "--non-existant");

            Assert.Null(value);
        }

        [Fact]
        public void Get_required_can_parse_numbers()
        {
            var args = new[] { "--target", "1" };

            var value = ArgumentParser.GetOptional<int>(args, "--target");

            Assert.Equal(value, 1);
        }

        [Fact]
        public void Get_required_can_parse_nullable_numbers()
        {
            var args = new[] { "--target", "1" };

            var value = ArgumentParser.GetOptional<int?>(args, "--target");

            Assert.Equal(value, 1);
        }

        [Fact]
        public void Get_required_will_parse_booleans()
        {
            var args = new[] { "--target", "true" };

            var value = ArgumentParser.GetOptional<bool>(args, "--target");

            Assert.Equal(value, true);
        }
    }
}
