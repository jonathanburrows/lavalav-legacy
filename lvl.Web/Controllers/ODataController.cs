using lvl.Ontology;
using lvl.Repositories;
using lvl.Web.Serialization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lvl.Web.Controllers
{
    /// <summary>
    /// Provides odata endpoints for all entities.
    /// </summary>
    [Route("[controller]")]
    public class ODataController
    {
        private TypeResolver TypeResolver { get; }
        private RepositoryFactory RepositoryFactory { get; }
        private EntityDeserializer EntityDeserializer { get; }

        public ODataController(TypeResolver typeResolver, RepositoryFactory repositoryFactory, EntityDeserializer entityDeserializer)
        {
            TypeResolver = typeResolver;
            RepositoryFactory = repositoryFactory;
            EntityDeserializer = entityDeserializer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        [HttpGet("{entityName}")]
        public async Task<IEnumerable<IEntity>> Get(string entityName)
        {

            if (entityName == null)
            {
                throw new ArgumentNullException(nameof(entityName));
            }

            var type = TypeResolver.Resolve(entityName);
            var repository = RepositoryFactory.Construct(type);

            return await repository.GetAsync();
        }
    }
}
