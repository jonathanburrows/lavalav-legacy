using System.IO;

namespace lvl.Ontology
{
    /// <summary>
    /// This is a workaround fix for SQLite's requirement of SQLite.Interop.dll needing to be
    /// copied into the executing program's directory.
    /// </summary>
    internal class SQLiteInteropCopier
    {
        private object x86Lock { get; } = new object();
        private object x64Lock { get; } = new object();

        public void Copy()
        {
            var currentAssembly = GetType().Assembly;

            var x86Info = currentAssembly.GetManifestResourceStream("lvl.Ontology.x86.SQLite.Interop.dll");
            var x86File = new FileInfo("x86/SQLite.Interop.dll");
            x86File.Directory.Create();
            if (!x86File.Exists)
            {
                lock (x86Lock)
                {
                    if (!x86File.Exists)
                    {
                        using (var x86Stream = x86File.Create())
                        {
                            x86Info.Seek(0, SeekOrigin.Begin);
                            x86Info.CopyTo(x86Stream);
                        }
                    }
                }
            }

            var x64Info = currentAssembly.GetManifestResourceStream("lvl.Ontology.x64.SQLite.Interop.dll");
            var x64File = new FileInfo("x64/SQLite.Interop.dll");
            if (!x64File.Exists)
            {
                lock (x64Lock)
                {
                    if (x64File.Exists)
                    {
                        x64File.Directory.Create();
                        using (var x64Stream = x64File.Create())
                        {
                            x64Info.Seek(0, SeekOrigin.Begin);
                            x64Info.CopyTo(x64Stream);
                        }
                    }
                }
            }
        }
    }
}
