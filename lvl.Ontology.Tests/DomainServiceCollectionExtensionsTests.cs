using FluentNHibernate.Cfg;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using Xunit;

namespace lvl.Ontology.Tests
{
    public class DomainServiceCollectionExtensionsTests
    {
        private IServiceProvider ServiceProvider { get; }

        public DomainServiceCollectionExtensionsTests()
        {
            ServiceProvider = new ServiceCollection().AddDomains().BuildServiceProvider();
        }

        [Fact]
        public void AfterAddingDomains_WhenResolvingNHibernateConfig_ValueIsReturned()
        {
            var options = ServiceProvider.GetRequiredService<FluentConfiguration>();

            Assert.True(false);
        }

        [Fact]
        public void ModelsInReferencedAssemblies_WhenAddingDomains_AreAddedToNHibernateConfig()
        {
            Assert.True(false);
        }

        [Fact]
        public void ModelsInExecutingAssembly_WhenAddingDomains_AreAddedToNHibernateConfig()
        {
            Assert.True(false);
        }

        [Fact]
        public void ModelsAlreadyAdded_WhenAddingDomains_AreNotAddedAgain()
        {
            Assert.True(false);
        }
    }
}