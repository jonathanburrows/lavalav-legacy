using System.Collections.Generic;
using System.Linq;

namespace lvl.Web.Cors
{
    public class CorsSettings
    {
        public IEnumerable<string> AllowHeaders { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> AllowMethods { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> AllowOrigins { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> ExposedHeaders { get; set; } = Enumerable.Empty<string>();
    }
}
