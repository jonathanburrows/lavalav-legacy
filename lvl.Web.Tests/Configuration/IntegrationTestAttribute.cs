using System;
using Xunit;
using Xunit.Sdk;

namespace lvl.Web.Tests.Configuration
{
    [XunitTestCaseDiscoverer("lvl.Web.Tests.Configuration.IntegrationTestDiscoverer", "lvl.Web.Tests")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class IntegrationTestAttribute : FactAttribute { }
}
