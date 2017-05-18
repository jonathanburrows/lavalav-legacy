using System;
using System.IO;
using System.Reflection;

namespace lvl.Ontology
{
    /// <summary>
    /// Used to load assemblies from a differnt directory into the app domain.
    /// </summary>
    public class AssemblyLoader
    {
        /// <summary>
        /// Loads an assembly from the given path, and loads any missing references from the assemblies directory.
        /// </summary>
        /// <param name="assemblyPath">The full path to the assembly to be loaded.</param>
        /// <returns>The loaded assembly.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="assemblyPath"/> is null</exception>
        /// <exception cref="FileNotFoundException">There was no assembly found at the given path</exception>
        public Assembly Load(string assemblyPath)
        {
            if (assemblyPath == null)
            {
                throw new ArgumentNullException(nameof(assemblyPath));
            }

            var assemblyFile = new FileInfo(assemblyPath);
            if (!assemblyFile.Exists)
            {
                throw new FileNotFoundException($"Assembly {assemblyFile.FullName} not found");
            }

            // When resolving dependencies, they might be found in the same directory as the loaded dll, so check there.
            var assemblyDirectory = assemblyFile.Directory?.FullName ?? "./";
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var missingAssemblyName = new AssemblyName(args.Name).Name;

                var missingAssemblyPath = Path.Combine(assemblyDirectory, $"{missingAssemblyName}.dll");
                if (File.Exists(missingAssemblyPath))
                {
                    return Assembly.LoadFrom(missingAssemblyPath);
                }

                // We also want to load an executable, but that requires a different load method
                var missingExecutablePath = Path.Combine(assemblyDirectory, $"{missingAssemblyName}.exe");
                if (File.Exists(missingExecutablePath))
                {
                    return Assembly.LoadFile(missingExecutablePath);
                }

                return null;
            };

            var absolutePath = new FileInfo(assemblyPath).FullName;
            return Assembly.LoadFile(absolutePath);
        }
    }
}