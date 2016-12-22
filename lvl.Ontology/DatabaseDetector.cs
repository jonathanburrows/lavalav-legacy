using NHibernate.Cfg;
using NHibernate.Driver;
using System.Text.RegularExpressions;

namespace lvl.Ontology
{
    /// <summary>
    /// Used to detect the vendor associated with a given connection string.
    /// </summary>
    public sealed class DatabaseDetector
    {
        /// <summary>
        /// Detects the vendor associated with a given connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to be analysed for the vendor.</param>
        /// <returns>The associated database vendor.</returns>
        /// <remarks>If null, then SQLite is returned.</remarks>
        public DatabaseVendor GetConnectionStringsVendor(string connectionString)
        {
            var sqlServerPattern = new Regex(@"(i?)(database)|(initial catalog)", RegexOptions.IgnoreCase);
            var oraclePattern = new Regex(@"(i?)(data source)", RegexOptions.IgnoreCase);

            if (string.IsNullOrEmpty(connectionString))
            {
                return DatabaseVendor.SQLite;
            }
            else if (sqlServerPattern.IsMatch(connectionString))
            {
                return DatabaseVendor.MsSql;
            }
            else if (oraclePattern.IsMatch(connectionString))
            {
                return DatabaseVendor.Oracle;
            }
            else
            {
                return DatabaseVendor.Unsupported;
            }
        }

        /// <summary>
        /// Returns the database vendor for the given configuration.
        /// </summary>
        /// <param name="configuration">The configuration which has had a database configured.</param>
        /// <returns>The vendor which the configuration has been set up for.</returns>
        public DatabaseVendor GetConfigurationsVendor(Configuration configuration)
        {
            var driverKey = NHibernate.Cfg.Environment.ConnectionDriver;
            var driver = configuration.GetProperty(driverKey);

            if (driver == typeof(SQLite20Driver).AssemblyQualifiedName)
            {
                return DatabaseVendor.SQLite;
            }
            else if (driver == typeof(SqlClientDriver).AssemblyQualifiedName)
            {
                return DatabaseVendor.Oracle;
            }
            else if (driver == typeof(OracleClientDriver).AssemblyQualifiedName)
            {
                return DatabaseVendor.Oracle;
            }
            else
            {
                return DatabaseVendor.Unsupported;
            }
        }
    }
}
