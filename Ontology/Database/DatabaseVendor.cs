namespace lvl.Ontology.Database
{
    /// <summary>
    ///     Represents which database nhibernate will connect to.
    /// </summary>
    public enum DatabaseVendor
    {
        // ReSharper disable once InconsistentNaming Verbatim name of vendor.
        SQLite,
        MsSql,
        Oracle,
        Unsupported
    }
}
