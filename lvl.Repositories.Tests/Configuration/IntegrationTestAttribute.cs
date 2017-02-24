using System;
using Xunit;
using Xunit.Sdk;

namespace lvl.Repositories.Tests.Configuration
{
    /// <summary>
    /// Signifies the test will be run against multiple database vendors.
    /// 
    /// Unit tests depending on a certain database vendor can be turned off in appsettings.json
    /// 
    /// A unit test depends on a database if its parent class has an OracleRepositoryFixture or an MsSqlRepositoryFixture.
    /// </summary>
    [XunitTestCaseDiscoverer("lvl.Repositories.Tests.Configuration.IntegrationTestDiscoverer", "lvl.Repositories.Tests")]
    [AttributeUsage(AttributeTargets.Method)]
    public class IntegrationTestAttribute : FactAttribute { }
}
