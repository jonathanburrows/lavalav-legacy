﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

// ReSharper disable once CheckNamespace In compliance with Microsoft conventions
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Configurations for logging.
    /// </summary>
    public class LoggingOptions
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        public LoggingOptions() { }

        public LoggingOptions(IConfiguration configuration)
        {
            if(configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration.GetSection("logging").Bind(this);
        }
    }
}
