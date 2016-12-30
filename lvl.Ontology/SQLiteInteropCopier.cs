using System.IO;
using System.Reflection;

namespace lvl.Ontology
{
    /// <summary>
    /// This is a workaround fix for SQLite's requirement of SQLite.Interop.dll needing to be
    /// copied into the executing program's directory.
    /// </summary>
    internal class SQLiteInteropCopier
    {
        private static object resourceLock { get; } = new object { };

        public void Copy()
        {
            CopyResourceToFile("lvl.Ontology.x86.SQLite.Interop.dll", "x86/SQLite.Interop.dll");
            CopyResourceToFile("lvl.Ontology.x64.SQLite.Interop.dll", "x64/SQLite.Interop.dll");
        }
        
        private void CopyResourceToFile(string resourceName, string outputName)
        {
            var currentAssembly = GetType().Assembly;

            var executingPath = Assembly.GetExecutingAssembly().Location;
            var executingDirectory = new FileInfo(executingPath).Directory.FullName;

            var resourceContent = currentAssembly.GetManifestResourceStream(resourceName);
            var outputPath = Path.Combine(executingDirectory, outputName);
            var outputFile = new FileInfo(outputPath);
            if (!outputFile.Exists)
            {
                lock (resourceLock)
                {
                    if (!outputFile.Exists)
                    {
                        outputFile.Directory.Create();
                        using (var outputStream = outputFile.Create())
                        {
                            resourceContent.Seek(0, SeekOrigin.Begin);
                            resourceContent.CopyTo(outputStream);
                        }
                    }
                }
            }
        }
    }
}
