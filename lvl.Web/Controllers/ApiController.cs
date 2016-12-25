using lvl.Ontology;
using lvl.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lvl.Web.Controllers
{
    /// <summary>
    /// Provides restful endpoints for all entities.
    /// </summary>
    [Route("[controller]")]
    public class ApiController : Controller
    {
        private TypeResolver TypeResolver { get; }
        private RepositoryFactory RepositoryFactory { get; }

        public ApiController(TypeResolver typeResolver, RepositoryFactory repositoryFactory)
        {
            TypeResolver = typeResolver;
            RepositoryFactory = repositoryFactory;
        }

        /// <summary>
        /// Retreives all entities of a given type.
        /// </summary>
        /// <param name="entityName">The type of entities to be retreived.</param>
        /// <returns>All entities of the given type.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entityName"/> is null.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="entityName"/> is not a mapped type.</exception>
        [HttpGet("{entityName}")]
        public async Task<IEnumerable<IEntity>> Get(string entityName) {
            if (entityName == null) throw new ArgumentNullException(nameof(entityName));

            var type = TypeResolver.Resolve(entityName);
            var repository = RepositoryFactory.Construct(type);

            return await repository.GetAsync();
        }

        /// <summary>
        /// Returns an entity of the given type with a matching id.
        /// </summary>
        /// <param name="entityName">The type of the desired entity.</param>
        /// <param name="id">The identifier of the desired entity.</param>
        /// <returns>The matching entity.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entityName"/> is null</exception>
        /// <exception cref="InvalidOperationException">No matching entity could be found</exception>
        /// <exception cref="InvalidOperationException"><paramref name="entityName"/> is not a mapped type</exception>
        [HttpGet("{entityName}/{id}")]
        public async Task<IEntity> Get(string entityName, int id)
        {
            if (entityName == null) throw new ArgumentNullException(nameof(entityName));

            var entityType = TypeResolver.Resolve(entityName);
            var repository = RepositoryFactory.Construct(entityType);

            var entity = await repository.GetAsync(id);
            if (entity == null)
            {
                throw new InvalidOperationException($"Could not find {entityType.FullName} with an id of {id}");
            }

            return entity;
        }
    }
}
