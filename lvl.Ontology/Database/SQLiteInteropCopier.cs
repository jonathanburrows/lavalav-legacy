using System.IO;
using System.Reflection;

namespace lvl.Ontology.Database
{
    /// <summary>
    /// This is a workaround fix for SQLite's requirement of SQLite.Interop.dll needing to be
    /// copied into the executing program's directory.
    /// </summary>
    // ReSharper disable once InconsistentNaming Verbatim name of vendor
    internal class SQLiteInteropCopier
    {
        private static object ResourceLock { get; } = new object();

        public void Copy()
        {
            CopyResourceToFile("lvl.Ontology.x86.SQLite.Interop.dll", "x86/SQLite.Interop.dll");
            CopyResourceToFile("lvl.Ontology.x64.SQLite.Interop.dll", "x64/SQLite.Interop.dll");
        }

        private void CopyResourceToFile(string resourceName, string outputName)
        {
            var currentAssembly = GetType().Assembly;

            var executingPath = Assembly.GetExecutingAssembly().Location;

            // ReSharper disable once PossibleNullReferenceException can safely assume that it exists
            var executingDirectory = new FileInfo(executingPath).Directory.FullName;

            var resourceContent = currentAssembly.GetManifestResourceStream(resourceName);
            var outputPath = Path.Combine(executingDirectory, outputName);
            var outputFile = new FileInfo(outputPath);
            if (!outputFile.Exists)
            {
                lock (ResourceLock)
                {
                    if (!outputFile.Exists)
                    {
                        // ReSharper disable once PossibleNullReferenceException can safely assume that it exists
                        outputFile.Directory.Create();
                        using (var outputStream = outputFile.Create())
                        {
                            // ReSharper disable once PossibleNullReferenceException can safely assume that it exists
                            resourceContent.Seek(0, SeekOrigin.Begin);
                            resourceContent.CopyTo(outputStream);
                        }
                    }
                }
            }
        }
    }
}
