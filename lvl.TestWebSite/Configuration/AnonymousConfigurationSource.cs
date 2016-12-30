using Microsoft.Extensions.Configuration;
using System;

namespace lvl.TestWebSite.Configuration
{
    /// <summary>
    /// Represents an anonymous object that can be used as a configuration file.
    /// </summary>
    /// <remarks>This was done to be consistent with Microsoft's configuration implementations.</remarks>
    internal class AnonymousConfigurationSource : IConfigurationSource
    {
        public object Anonymous { get; }

        public AnonymousConfigurationSource(object anonymous)
        {
            if (anonymous == null)
            {
                throw new ArgumentNullException(nameof(anonymous));
            }
            Anonymous = anonymous;
        }

        public IConfigurationProvider Build(IConfigurationBuilder configurationBuilder)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }

            return new AnonymousConfigurationProvider(this);
        }
    }
}
