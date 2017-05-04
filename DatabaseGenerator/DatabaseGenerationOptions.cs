using Microsoft.Extensions.Configuration;
using System;

// ReSharper disable once CheckNamespace In compliance with Microsoft's conventions.
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Configurations on how a database will be generated.
    /// </summary>
    public class DatabaseGenerationOptions
    {
        /// <summary>The connection to the database which will be created/migrated.</summary>
        public string ConnectionString { get; set; }

        /// <summary>The path to the application which references the models.</summary>
        public string AssemblyPath { get; set; }

        /// <summary>Scripts in this directory will be run before the database is generated.</summary>
        public string PreGenerationScriptBin { get; set; }

        /// <summary>Scripts in this directory will be run after the database is generated.</summary>
        public string PostGenerationScriptBin { get; set; }

        /// <summary>Will apply patches, instead of overwritting the database.</summary>
        public bool Migrate { get; set; }

        /// <summary>Will prevent the database from being changed, and will output a report instead.</summary>
        public bool DryRun { get; set; }

        public DatabaseGenerationOptions() { }

        public DatabaseGenerationOptions(IConfiguration configuration)
        {
            if(configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration.GetSection("database-generation").Bind(this);
        }
    }
}
