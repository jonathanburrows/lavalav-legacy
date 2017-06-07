using lvl.TypescriptGenerator;
using System;
using System.IO;
using Xunit;

namespace lvl.TypeScriptGenerator.Tests
{
    public class ArgumentParserTests
    {
        private ArgumentParser ArgumentParser { get; }

        public ArgumentParserTests()
        {
            ArgumentParser = new ArgumentParser();
        }

        [Fact]
        public void It_will_throw_argument_null_exception_when_args_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => ArgumentParser.Parse(null));
        }

        [Fact]
        public void It_will_throw_argument_exception_when_theres_no_assembly_path()
        {
            var args = new[] { "--output-bin=./" };

            Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(args));
        }

        [Fact]
        public void It_will_throw_argument_exception_when_theres_multiple_assembly_paths()
        {
            var args = new[] { "--assembly-path=First.dll", "--assembly-path=Second.dll", "--output-path=./" };

            Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(args));
        }

        [Fact]
        public void It_will_throw_file_not_found_exception_when_the_assembly_path_cant_be_found()
        {
            var args = new[] { "--assembly-path=FakePath.dll", "--output-bin=hello" };

            Assert.Throws<FileNotFoundException>(() => ArgumentParser.Parse(args));
        }

        [Fact]
        public void It_will_throw_argument_exception_when_theres_no_output_bin()
        {
            var args = new[] { "--assembly-path=lvl.Ontology.dll" };

            Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(args));
        }

        [Fact]
        public void It_will_throw_argument_null_exception_when_theres_multiple_output_bins()
        {
            var args = new[]
            {
                "--assembly-path=lvl.Ontology.dll",
                "--output-bin=./",
                "--output-bin=./"
            };

            Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(args));
        }

        [Fact]
        public void It_will_throw_argument_exception_when_theres_multiple_decorator_paths()
        {
            var args = new[]
            {
                "--assembly-path=lvl.Ontology.dll",
                "--output-bin=./",
                "--decorator-path=./",
                "--decorator-path=./"
            };

            Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(args));
        }

        [Fact]
        public void It_will_throw_argument_exception_when_theres_multiple_namespaces_with_same_name()
        {
            var args = new[]
            {
                "--assembly-path=lvl.Ontology.dll",
                "--output-bin=./",
                "lvl.Ontology=@lvl/front-end",
                "lvl.Ontology=@lvl/front-end"
            };

            Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(args));
        }

        [Fact]
        public void It_will_populate_assembly_path_when_first_in_arguments()
        {
            var assemblyPath = "lvl.Ontology.dll";
            var args = new[]
            {
                $"--assembly-path={assemblyPath}",
                "--output-bin=./",
                "--decorator-path=./",
                "lvl.Ontology=@lvl/front-end"
            };

            var generationOptions = ArgumentParser.Parse(args);

            Assert.Equal(assemblyPath, generationOptions.AssemblyPath);
        }

        [Fact]
        public void It_will_populate_assembly_path_when_second_in_arguments()
        {
            var assemblyPath = "lvl.Ontology.dll";
            var args = new[]
            {
                "--output-bin=./",
                $"--assembly-path={assemblyPath}",
                "--decorator-path=./",
                "lvl.Ontology=@lvl/front-end"
            };

            var generationOptions = ArgumentParser.Parse(args);

            Assert.Equal(assemblyPath, generationOptions.AssemblyPath);
        }

        [Fact]
        public void It_will_populate_assembly_path_when_last_in_arguments()
        {
            var assemblyPath = "lvl.Ontology.dll";
            var args = new[]
            {
                "--output-bin=./",
                "--decorator-path=./",
                "lvl.Ontology=@lvl/front-end",
                $"--assembly-path={assemblyPath}"
            };

            var generationOptions = ArgumentParser.Parse(args);

            Assert.Equal(assemblyPath, generationOptions.AssemblyPath);
        }

        [Fact]
        public void It_will_populate_output_bin_when_first_in_arguments()
        {
            var outputBin = "./";
            var args = new[]
            {
                $"--output-bin={outputBin}",
                "--assembly-path=lvl.Ontology.dll",
                "--decorator-path=./",
                "lvl.Ontology=@lvl/front-end"
            };

            var generationOptions = ArgumentParser.Parse(args);

            Assert.Equal(outputBin, generationOptions.OutputBin);
        }

        [Fact]
        public void It_will_populate_output_bin_when_second_in_arguments()
        {
            var outputBin = "./";
            var args = new[]
            {
                "--assembly-path=lvl.Ontology.dll",
                $"--output-bin={outputBin}",
                "--decorator-path=./",
                "lvl.Ontology=@lvl/front-end"
            };

            var generationOptions = ArgumentParser.Parse(args);

            Assert.Equal(outputBin, generationOptions.OutputBin);
        }

        [Fact]
        public void It_will_populate_output_bin_when_last_in_arguments()
        {
            var outputBin = "./";
            var args = new[]
            {
                "--assembly-path=lvl.Ontology.dll",
                "--decorator-path=./",
                "lvl.Ontology=@lvl/front-end",
                $"--output-bin={outputBin}"
            };

            var generationOptions = ArgumentParser.Parse(args);

            Assert.Equal(outputBin, generationOptions.OutputBin);
        }

        [Fact]
        public void It_will_poplate_decorator_path_when_first_in_arguments()
        {
            var decoratorPath = "./";
            var args = new[]
            {
                $"--decorator-path={decoratorPath}",
                "--assembly-path=lvl.Ontology.dll",
                "--output-bin=./",
                "lvl.Ontology=@lvl/front-end"
            };

            var generationOptions = ArgumentParser.Parse(args);

            Assert.Equal(decoratorPath, generationOptions.DecoratorPath);
        }

        [Fact]
        public void It_will_poplate_decorator_path_when_second_in_arguments()
        {
            var decoratorPath = "./";
            var args = new[]
            {
                "--assembly-path=lvl.Ontology.dll",
                $"--decorator-path={decoratorPath}",
                "--output-bin=./",
                "lvl.Ontology=@lvl/front-end"
            };

            var generationOptions = ArgumentParser.Parse(args);

            Assert.Equal(decoratorPath, generationOptions.DecoratorPath);
        }

        [Fact]
        public void It_will_poplate_decorator_path_when_last_in_arguments()
        {
            var decoratorPath = "./";
            var args = new[]
            {
                "--assembly-path=lvl.Ontology.dll",
                "--output-bin=./",
                "lvl.Ontology=@lvl/front-end",
                $"--decorator-path={decoratorPath}"
            };

            var generationOptions = ArgumentParser.Parse(args);

            Assert.Equal(decoratorPath, generationOptions.DecoratorPath);
        }

        [Fact]
        public void It_will_poplate_namespace_when_first_in_arguments()
        {
            var namespaceKey = "lvl.Ontology";
            var namespacePackage = "@lvl/front-end";
            var args = new[]
            {
                $"{namespaceKey}={namespacePackage}",
                "--assembly-path=lvl.Ontology.dll",
                "--output-bin=./",
                "--decorator-path=./"
            };

            var generationOptions = ArgumentParser.Parse(args);
            var generatedMapping = generationOptions.PackageForNamespace[namespaceKey];

            Assert.Equal(namespacePackage, generatedMapping);
        }

        [Fact]
        public void It_will_poplate_namespace_when_second_in_arguments()
        {
            var namespaceKey = "lvl.Ontology";
            var namespacePackage = "@lvl/front-end";
            var args = new[]
            {
                "--assembly-path=lvl.Ontology.dll",
                $"{namespaceKey}={namespacePackage}",
                "--output-bin=./",
                "--decorator-path=./"
            };

            var generationOptions = ArgumentParser.Parse(args);
            var generatedMapping = generationOptions.PackageForNamespace[namespaceKey];

            Assert.Equal(namespacePackage, generatedMapping);
        }

        [Fact]
        public void It_will_poplate_namespace_when_last_in_arguments()
        {
            var namespaceKey = "lvl.Ontology";
            var namespacePackage = "@lvl/front-end";
            var args = new[]
            {
                "--assembly-path=lvl.Ontology.dll",
                "--output-bin=./",
                "--decorator-path=./",
                $"{namespaceKey}={namespacePackage}"
            };

            var generationOptions = ArgumentParser.Parse(args);
            var generatedMapping = generationOptions.PackageForNamespace[namespaceKey];

            Assert.Equal(namespacePackage, generatedMapping);
        }

        [Fact]
        public void It_will_populate_two_namespaces_when_two_are_present()
        {
            var args = new[]
            {
                "--assembly-path=lvl.Ontology.dll",
                "--output-bin=./",
                "lvl.Ontology=@lvl/front-end",
                "lvl.TestDomain=@lvl/test-domain"
            };

            var generationOptions = ArgumentParser.Parse(args);

            Assert.Equal(2, generationOptions.PackageForNamespace.Count);
        }
    }
}
