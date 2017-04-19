using lvl.Ontology.Database;
using lvl.Repositories.Tests.Fixtures;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace lvl.Repositories.Tests.Configuration
{
    /// <summary>
    /// Disables tests which are dependent on a database, but are disabled in appsettings.json.
    /// 
    /// A unit test depends on a database if its parent class has an OracleRepositoryFixture or an MsSqlRepositoryFixture.
    /// </summary>
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
                var dependsOnMsSql = CollectionUsesFixture<MsSqlRepositoryFixture>(collectionType);
                var dependsOnOracle = CollectionUsesFixture<OracleRepositoryFixture>(collectionType);

                var msSqlConflict = dependsOnMsSql && DisableMsSql;
                var oracleConflict = dependsOnOracle && DisableOracle;

                isEnabled = !msSqlConflict && !oracleConflict;
                EnabledCollectionTypes.TryAdd(collectionType, isEnabled);
            }

            if (isEnabled)
            {
                return integrationTests;
            }
            else
            {
                return integrationTests.Where(it =>
                {
                    var matchingName = it.Method.Name == testMethod.Method.Name;
                    var matchingClass = it.TestMethod.TestClass.Class.Name == testMethod.TestClass.Class.Name;
                    return !(matchingName && matchingClass);
                });
            }
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
            var integrationSettings = ConfigurationReader.IntegrationSettings;
            if (databaseVendor == DatabaseVendor.MsSql)
            {
                return integrationSettings.MsSql.Disabled ?? false;
            }
            else if (databaseVendor == DatabaseVendor.Oracle)
            {
                return integrationSettings.Oracle.Disabled ?? false;
            }
            else
            {
                throw new NotImplementedException($"Integration tests for the database vendor {databaseVendor} are not supported.");
            }
        }
    }
}
