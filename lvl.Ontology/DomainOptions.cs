using Microsoft.Extensions.Configuration;

namespace lvl.Ontology
{
    public class DomainOptions
    {
        public string ConnectionString { get; set; }

        public DomainOptions() { }

        public DomainOptions(IConfiguration configuration)
        {
            configuration.GetSection("domain").Bind(this);
        }
    }
}
