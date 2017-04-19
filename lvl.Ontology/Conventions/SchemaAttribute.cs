using System;

namespace lvl.Ontology.Naming
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SchemaAttribute : Attribute
    {
        public string Name { get; }

        public SchemaAttribute(string name)
        {
            Name = name;
        }
    }
}
