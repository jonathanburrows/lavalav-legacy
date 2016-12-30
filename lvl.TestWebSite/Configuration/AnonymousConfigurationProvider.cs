using Microsoft.Extensions.Configuration;
using System;

namespace lvl.TestWebSite.Configuration
{
    /// <summary>
    /// Provides an anonymous object as a configuration file.
    /// </summary>
    /// <remarks>This was done to keep consistent with Microsoft's implementation of configurations.</remarks>
    internal class AnonymousConfigurationProvider : ConfigurationProvider
    {
        public AnonymousConfigurationProvider(AnonymousConfigurationSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (source.Anonymous == null)
            {
                throw new ArgumentNullException(nameof(source.Anonymous));
            }

            var parser = new AnonymousConfigurationParser();
            Data = parser.Parse(source.Anonymous);
        }
    }
}
