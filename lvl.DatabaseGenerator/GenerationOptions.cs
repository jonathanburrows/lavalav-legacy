namespace lvl.DatabaseGenerator
{
    public class GenerationOptions
    {
        public string ConnectionString { get; set; }
        public string AssemblyPath { get; set; }
        public string PreGenerationScriptBin { get; set; }
        public string PostGenerationScriptBin { get; set; }
        public bool Migrate { get; set; }
        public bool DryRun { get; set; }
    }
}
