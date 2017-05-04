namespace lvl.TypescriptGenerator
{
    /// <summary>
    /// Represents metadata that can be applied to a property.
    /// </summary>
    public class TypeScriptDecorator
    {
        // The name of the decorator.
        public string Name { get; set; }

        // The arguments to be passed to the decorator.
        public object[] Arguments { get; set; } = new object[0];
    }
}
