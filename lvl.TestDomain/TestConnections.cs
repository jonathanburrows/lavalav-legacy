using lvl.Ontology;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace lvl.TestDomain
{
    /// <summary>
    /// Used by unit tests to read connection strings from appsettings.json to testing databases.
    /// </summary>
    public static class TestConnections
    {
        private static DatabaseDetector DatabaseDetector { get; }

        static TestConnections()
        {
            DatabaseDetector = new DatabaseDetector { };
        }

        public static string Oracle()
        {
            var oracleConnectionString = GetAssemblyConfiguration("oracle");
            if (DatabaseDetector.GetConnectionStringsVendor(oracleConnectionString) != DatabaseVendor.Oracle)
            {
                throw new InvalidOperationException("Oracle connection string is not in oracle format");
            }
            return oracleConnectionString;
        }

        public static string MsSql()
        {
            var sqlConnectionString = GetAssemblyConfiguration("ms-sql");
            if (DatabaseDetector.GetConnectionStringsVendor(sqlConnectionString) != DatabaseVendor.MsSql)
            {
                throw new InvalidOperationException("MsSql connection string is not in oracle format");
            }
            return sqlConnectionString;
        }

        private static string GetAssemblyConfiguration(string connectionKey)
        {
            var assembly = Assembly.GetAssembly(typeof(TestConnections));
            var assemblyConfiguration = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
            var environment = assemblyConfiguration.Configuration.ToLower();

            var resources = assembly.GetManifestResourceNames();
            var appConfigName = resources.FirstOrDefault(name => name.ToLower().EndsWith("appsettings.json"));
            var environmentConfigName = resources.FirstOrDefault(name => name.ToLower().EndsWith($"appsettings.{environment}.json"));

            if (appConfigName == null && environmentConfigName == null)
            {
                throw new InvalidOperationException($"Neither appsettings.json or appsettings.{environment}.json were embedded in the dll");
            }

            var appConfig = assembly.GetManifestResourceStream(appConfigName).WriteAsString();
            var appConnection = GetJsonConnection(appConfig, connectionKey);

            var environmentConfig = default(string);
            if (environmentConfigName != null)
            {
                environmentConfig = assembly.GetManifestResourceStream(environmentConfigName).WriteAsString();
            }
            var environmentConnection = GetJsonConnection(environmentConfig, connectionKey);

            var connection = environmentConnection ?? appConnection;
            if (connection != null)
            {
                return connection;
            }
            else if (appConfig != null && environmentConfig != null)
            {
                throw new InvalidOperationException($"The {connectionKey} connection was not defined in appsettings.json or appsettings.{environment}.json");
            }
            else if (appConfig != null)
            {
                throw new InvalidOperationException($"The {connectionKey} connection was not defined in appsettings.json");
            }
            else if (environmentConfig != null)
            {
                throw new InvalidOperationException($"The {connectionKey} connection was not defined in appsettings.{environment}.json");
            }
            else
            {
                throw new InvalidOperationException($"Neither appsettings.json or appsettings.{environment}.json were embedded in the dll");
            }
        }

        private static string WriteAsString(this Stream writing)
        {
            if (writing == null)
            {
                return null;
            }

            using (var streamReader = new StreamReader(writing))
            {
                return streamReader.ReadToEnd();
            }
        }

        private static string GetJsonConnection(string configuration, string connectionKey)
        {
            if (configuration == null)
            {
                return null;
            }

            var json = (JObject)JsonConvert.DeserializeObject(configuration);
            return json?["connections"]?[connectionKey].Value<string>();
        }
    }
}
