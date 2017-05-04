﻿using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public class DomainOptions
    {
        /// <summary>
        /// Connection String to be used by all the application.
        /// </summary>
        public string ConnectionString { get; set; }

        public DomainOptions() { }

        public DomainOptions(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration.GetSection("domain").Bind(this);
        }
    }
}
