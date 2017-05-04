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
        public void IfArgsNull_ArgumentNullExceptionIsThrown()
        {
            Assert.Throws<ArgumentNullException>(() => ArgumentParser.Parse(null));
        }

        [Fact]
        public void IfNoAssemblyPath_ArgumentExceptionIsThrown()
        {
            var args = new[] { "--output-bin=./" };

            Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(args));
        }

        [Fact]
        public void IfMultipleAssemblyPaths_ArgumentExceptionIsThrown()
        {
            var args = new[] { "--assembly-path=First.dll", "--assembly-path=Second.dll", "--output-path=./" };

            Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(args));
        }

        [Fact]
        public void IfCantFindAssemblyPath_FileNotFoundExceptionIsThrown()
        {
            var args = new[] { "--assembly-path=FakePath.dll", "--output-bin=hello" };

            Assert.Throws<FileNotFoundException>(() => ArgumentParser.Parse(args));
        }

        [Fact]
        public void IfNoOutputBin_ArgumentExceptionIsThrown()
        {
            var args = new[] { "--assembly-path=lvl.Ontology.dll" };

            Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(args));
        }

        [Fact]
        public void IfMultipleOutputBins_ArgumentExceptionIsThrown()
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
        public void IfMultipleDecoratorPaths_ArgumentExceptionIsThrown()
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
        public void IfMultipleNamespacesWithSameName_ArgumentExceptionIsThrown()
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
        public void AssemblyPath_WhenFirstInArguments_ReturnsValue()
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
        public void AssemblyPath_WhenSecondInArguments_ReturnsValue()
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
        public void AssemblyPath_WhenLastInArguments_ReturnsValue()
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
        public void OutputBin_WhenFirstInArguments_ReturnsValue()
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
        public void OutputBin_WhenSecondInArguments_ReturnsValue()
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
        public void OutputBin_WhenLastInArguments_ReturnsValue()
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
        public void DecoratorPath_WhenFirstInArguments_ReturnsValue()
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
        public void DecoratorPath_WhenSecondInArguments_ReturnsValue()
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
        public void DecoratorPath_WhenLastInArguments_ReturnsValue()
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
        public void Namespace_WhenFirstInArguments_ReturnsValue()
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
        public void Namespace_WhenSecondInArguments_ReturnsValue()
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
        public void Namespace_WhenLastInArguments_ReturnsValue()
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
        public void Namespace_WhenTwo_TwoAreMapped()
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
