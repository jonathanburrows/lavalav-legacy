using System;

namespace lvl.Ontology.Authorization
{
    [Flags]
    public enum Actions
    {
        Read = 1 << 1,
        Create = 1 << 2,
        Update = 1 << 3,
        Delete = 1 << 4
    }
}
