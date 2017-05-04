using NHibernate;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace lvl.DatabaseGenerator
{
    /// <summary>
    /// Runs scripts from a folder against the registered database.
    /// </summary>
    public class ScriptRunner : IDisposable
    {
        private ISessionFactory SessionFactory { get; }

        public ScriptRunner(Configuration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            SessionFactory = configuration.BuildSessionFactory();
        }

        /// <summary>
        /// Runs scripts from a given folder against the registered database.
        /// </summary>
        /// <param name="scriptBin">The directory which will contain the .sql files to be run.</param>
        /// <remarks>Run in order by file name.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="scriptBin"/> is null.</exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="scriptBin"/> is not a directory.</exception>
        public void RunScriptsInDirectory(string scriptBin)
        {
            if (scriptBin == null)
            {
                throw new ArgumentNullException(nameof(scriptBin));
            }

            var scriptDirectory = new DirectoryInfo(scriptBin);
            if (!scriptDirectory.Exists)
            {
                throw new DirectoryNotFoundException($"The script bin {scriptDirectory.FullName} does not exist.");
            }

            var scripts = GetOrderedScriptsFromDirectory(scriptDirectory);

            using (var session = SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                foreach (var script in scripts)
                {
                    session.CreateSQLQuery(script).ExecuteUpdate();
                }
                transaction.Commit();
            }
        }

        private IEnumerable<string> GetOrderedScriptsFromDirectory(DirectoryInfo directory)
        {
            var scriptFiles = directory.EnumerateFiles().Where(file => file.Extension.Equals(".sql", StringComparison.InvariantCultureIgnoreCase));
            var orderedFiles = scriptFiles.OrderBy(file => file.Name);
            return orderedFiles.Select(file => File.ReadAllText(file.FullName));
        }

        public void Dispose()
        {
            SessionFactory.Dispose();
        }
    }
}
