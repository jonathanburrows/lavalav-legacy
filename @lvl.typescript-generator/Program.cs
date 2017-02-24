using lvl.Ontology;
using System;
using System.Linq;
using lvl.TypescriptGenerator.Extensions;
using System.IO;

namespace lvl.TypescriptGenerator
{
    public class Program
    {
        /// <summary>
        /// An application to generate typescript models from a given assembly.
        /// 
        /// Synopsis:
        /// @lvl.typescript-generator --assembly-path='assembly-path' --output-bin='output-bin' [--decorator-path='decorator-path'] ['C# namespace'='npm package'[, 'C# namespace'='npm package'...]] 
        /// 
        /// Example:
        /// @lvl.typescript-generator --assembly-path=lvl.TestDomain.dll --output-bin='src/models' lvl.Ontology='@lvl/core'
        /// 
        /// </summary>
        /// <param name="args">The command line arguments used to generate the models.</param>
        /// <exception cref="ArgumentNullException"><paramref name="args"/> is null.</exception> 
        public static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException();
            }

            var argumentParser = new ArgumentParser();
            var generationOptions = argumentParser.Parse(args);

            var assemblyLoader = new AssemblyLoader();
            var assembly = assemblyLoader.Load(generationOptions.AssemblyPath);

            var typeConverter = new TypeConverter();

            // Due to a dynamic load bug, typeof(IEntity) != typeof(IEntity), so use guid comparison also
            var ientity = typeof(IEntity);
            var entityTypes = assembly.GetExportedTypes().Where(t => ientity.IsAssignableFrom(t) || t.GUID == ientity.GUID);

            // we want to get any possible errors before writing, generate typescript first
            var tsTypes = entityTypes.Select(t => typeConverter.CsToTypeScript(t, generationOptions)).ToList();
            var tsOutputs = tsTypes.Select(t => new
            {
                FileName = t.Name.ToDashed() + ".ts",
                Content = t.ToTypeScript()
            }).ToList();

            foreach (var tsOutput in tsOutputs)
            {
                var filePath = Path.Combine(generationOptions.OutputBin, tsOutput.FileName);
                var fileInfo = new FileInfo(filePath);
                fileInfo.Directory?.Create();
                File.WriteAllText(fileInfo.FullName, tsOutput.Content);
            }

            // construct the barrel file
            var exportStatements = tsTypes.OrderBy(ts => ts.Name).Select(ts => $"export {{ {ts.Name} }} from './{ts.Name.ToDashed()}';");
            var barrelContents = string.Join(Environment.NewLine, exportStatements);
            var barrelPath = Path.Combine(generationOptions.OutputBin, "index.ts");

            var barrelInfo = new FileInfo(barrelPath);
            // possible there's no ts files, and directory does not exist.
            barrelInfo.Directory?.Create();
            File.WriteAllText(barrelInfo.FullName, barrelContents);
        }
    }
}
