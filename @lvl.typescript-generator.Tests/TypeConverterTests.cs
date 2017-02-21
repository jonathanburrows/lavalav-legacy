using lvl.TypescriptGenerator;
using lvl.TypeScriptGenerator.Tests.Fixtures;
using System;
using System.Collections.Generic;
using Xunit;

namespace lvl.TypeScriptGenerator.Tests
{
    public class TypeConverterTests
    {
        private TypeConverter TypeConverter { get; }
        private IReadOnlyDictionary<string, string> PackageForNamespace { get; }

        public TypeConverterTests()
        {
            TypeConverter = new TypeConverter();
            PackageForNamespace = new Dictionary<string, string>
            {
                ["lvl.ExternalLibrary"] = "@lvl/external-library"
            };
        }

        [Fact]
        public void ConvertingNullType_ThrowsArgumentNullException()
        {
            var packageForNamespace = new Dictionary<string, string>();

            Assert.Throws<ArgumentNullException>(() => TypeConverter.CsToTypeScript(null, packageForNamespace));
        }

        [Fact]
        public void ConvertingNullPackagesForNamespace_ThrowsArgumentNullException()
        {
            var type = GetType();

            Assert.Throws<ArgumentNullException>(() => TypeConverter.CsToTypeScript(type, null));
        }
    }
}
