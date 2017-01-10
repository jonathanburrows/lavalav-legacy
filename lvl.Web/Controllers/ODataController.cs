using lvl.Repositories;
using lvl.Web.OData;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace lvl.Web.Controllers
{
    /// <summary>
    /// Provides odata endpoints for all entities.
    /// </summary>
    [Route("[controller]")]
    public class ODataController : Controller
    {
        private TypeResolver TypeResolver { get; }
        private RepositoryFactory RepositoryFactory { get; }
        private ODataQueryParser ODataParser { get; }

        public ODataController(TypeResolver typeResolver, RepositoryFactory repositoryFactory, ODataQueryParser odataParser)
        {
            if (typeResolver == null)
            {
                throw new ArgumentNullException(nameof(typeResolver));
            }
            if (repositoryFactory == null)
            {
                throw new ArgumentNullException(nameof(repositoryFactory));
            }
            if (odataParser == null)
            {
                throw new ArgumentNullException(nameof(odataParser));
            }

            TypeResolver = typeResolver;
            RepositoryFactory = repositoryFactory;
            ODataParser = odataParser;
        }

        /// <summary>
        /// Gets all the entities, applying the odata query string.
        /// </summary>
        /// <param name="entityName">The name of entities which will be queried.</param>
        /// <returns>The odata query results.</returns>
        [HttpGet("{entityName}")]
        public async Task<ODataResponse> Get(string entityName)
        {
            if (entityName == null)
            {
                throw new ArgumentNullException(nameof(entityName));
            }

            var type = TypeResolver.Resolve(entityName);
            var query = ODataParser.Parse(Request.Query, type);
            var repository = RepositoryFactory.Construct(type);

            var queryResult = await repository.GetAsync(query);
            return new ODataResponse
            {
                Context = UriHelper.GetDisplayUrl(Request),
                Value = queryResult.Items,
                Count = queryResult.Count
            };
        }
    }
}
