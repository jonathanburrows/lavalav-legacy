using lvl.Web.Tests.Fixtures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace lvl.Web.Tests
{
    [Collection(WebCollection.Name)]
    public class WebOptionsTests
    {
        [Fact]
        public void It_will_have_parameterless_constructor()
        {
            var parameterlessConstructor = typeof(WebOptions).GetConstructor(Type.EmptyTypes);
            Assert.NotNull(parameterlessConstructor);
        }

        [Fact]
        public void It_has_constructor_with_iconfiguration_parameter()
        {
            var configurationConstructor = typeof(WebOptions).GetConstructor(new[] { typeof(IConfiguration) });
            Assert.NotNull(configurationConstructor);
        }

        [Fact]
        public void It_throws_argument_null_exception_when_configuration_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new WebOptions(null));
        }
    }
}
