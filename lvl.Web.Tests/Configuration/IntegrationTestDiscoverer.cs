using lvl.Ontology;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace lvl.Web.Tests.Configuration
{
    public class IntegrationTestDiscoverer : FactDiscoverer
    {
        private static ConcurrentDictionary<ITypeInfo, Type> MappedTypes { get; } = new ConcurrentDictionary<ITypeInfo, Type>();
        private static ConcurrentDictionary<Type, bool> EnabledCollectionTypes { get; } = new ConcurrentDictionary<Type, bool>();
        private static bool DisableOracle { get; } = DatabaseDisabledInConfiguration(DatabaseVendor.Oracle);
        private static bool DisableMsSql { get; } = DatabaseDisabledInConfiguration(DatabaseVendor.MsSql);

        public IntegrationTestDiscoverer(IMessageSink diagnosticMessageSink) : base(diagnosticMessageSink) { }

        public override IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            if (discoveryOptions == null)
            {
                throw new ArgumentNullException(nameof(discoveryOptions));
            }
            if (testMethod == null)
            {
                throw new ArgumentNullException(nameof(testMethod));
            }
            if (factAttribute == null)
            {
                throw new ArgumentNullException(nameof(factAttribute));
            }

            var integrationTests = base.Discover(discoveryOptions, testMethod, factAttribute);

            var collectionInfo = testMethod.TestClass.Class;
            var collectionType = GetMappedType(collectionInfo);

            bool isEnabled;
            if (!EnabledCollectionTypes.TryGetValue(collectionType, out isEnabled))
            {
                return integrationTests.Where(it => it.Method != testMethod.Method);
            }
            return integrationTests;
        }

        private Type GetMappedType(ITypeInfo typeInfo)
        {
            Type mappedType;
            if (!MappedTypes.TryGetValue(typeInfo, out mappedType))
            {
                var assembly = Assembly.Load(typeInfo.Assembly.Name);
                mappedType = assembly.GetType(typeInfo.Name);
                if (mappedType == null)
                {
                    throw new InvalidOperationException($"There is no class {typeInfo.Name} in the assembly {typeInfo.Assembly.Name}");
                }

                MappedTypes.TryAdd(typeInfo, mappedType);
            }
            return mappedType;
        }

        private bool CollectionUsesFixture<TFixture>(Type collectionType)
        {
            var fixtureType = typeof(TFixture);
            var ctors = collectionType.GetConstructors();
            var fixtures = ctors.SelectMany(ctor => ctor.GetParameters());
            return fixtures.Any(parameter => fixtureType.IsAssignableFrom(parameter.ParameterType));
        }

        private static bool DatabaseDisabledInConfiguration(DatabaseVendor databaseVendor)
        {
            return databaseVendor == DatabaseVendor.MsSql ? true : false;
        }
    }
}
