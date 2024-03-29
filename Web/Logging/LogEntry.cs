﻿using FluentNHibernate.Data;
using lvl.Ontology;
using System;
using System.ComponentModel.DataAnnotations;

namespace lvl.Web.Logging
{
    /// <summary>
    ///     Represents information about server actions to be recorded.
    /// </summary>
    public class LogEntry : Entity, IAggregateRoot
    {
        [Required]
        public string MachineName { get; set; }

        [Required]
        public DateTime Logged { get; set; }

        [Required]
        public string LogLevel { get; set; }

        [Required]
        public string Message { get; set; }

        public string UserName { get; set; }
        public string Url { get; set; }
        public bool? Https { get; set; }
        public string Exception { get; set; }
        public string StackTrace { get; set; }
        public string EnvironmentName { get; set; }
    }
}
