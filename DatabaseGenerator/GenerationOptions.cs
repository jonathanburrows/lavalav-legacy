﻿namespace lvl.DatabaseGenerator
{
    /// <summary>
    /// Configurations on how a database will be generated.
    /// </summary>
    public class GenerationOptions
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
    }
}