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
    /// Provides restful endpoints for all entities.
    /// </summary>
    [Route("[controller]")]
    public class ApiController : Controller
    {
        private TypeResolver TypeResolver { get; }
        private RepositoryFactory RepositoryFactory { get; }
        private EntityDeserializer EntityDeserializer { get; }

        public ApiController(TypeResolver typeResolver, RepositoryFactory repositoryFactory, EntityDeserializer entityDeserializer)
        {
            TypeResolver = typeResolver;
            RepositoryFactory = repositoryFactory;
            EntityDeserializer = entityDeserializer;
        }

        /// <summary>
        /// Retreives all entities of a given type.
        /// </summary>
        /// <param name="entityName">The type of entities to be retreived.</param>
        /// <returns>All entities of the given type.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entityName"/> is null.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="entityName"/> is not a mapped type.</exception>
        [HttpGet("{entityName}")]
        public async Task<IEnumerable<IEntity>> Get(string entityName)
        {
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

        /// <summary>
        /// Adds a given entity to the persistent collection.
        /// </summary>
        /// <param name="entityName">The type of the entity to be added.</param>
        /// <returns>The entity with a populated id.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entityName"/> is null</exception>
        /// <exception cref="ArgumentNullException">The entity to be created is null.</exception>
        /// <exception cref="InvalidOperationException">The given entity type is not mapped.</exception>
        /// <exception cref="Newtonsoft.Json.JsonSerializationException">The given entity could not be deserialized as the given type.</exception>
        /// <exception cref="InvalidOperationException">The given entity already has an identifier.</exception>
        /// <remarks>The entity to be created must be attached to the body.</remarks>
        [HttpPost("{entityName}")]
        public async Task<IEntity> Post(string entityName)
        {
            if (entityName == null) throw new ArgumentNullException(nameof(entityName));

            var entityType = TypeResolver.Resolve(entityName);
            var creating = EntityDeserializer.Deserialize(Request.Body, entityType);
            var repository = RepositoryFactory.Construct(entityType);
            await repository.CreateAsync(creating);

            return creating;
        }

        /// <summary>
        /// Updates an existing entity to have matching fields.
        /// </summary>
        /// <param name="entityName">The type of the entity to be updated.</param>
        /// <returns>A copy of the updated entity, with any generated fields updated.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entityName"/> is null</exception>
        /// <exception cref="ArgumentNullException">The entity to be updated is null.</exception>
        /// <exception cref="InvalidOperationException">The given entity type is not mapped.</exception>
        /// <exception cref="Newtonsoft.Json.JsonSerializationException">The given entity could not be deserialized as the given type.</exception>
        /// <exception cref="InvalidOperationException">There exist no entity with that identifier.</exception>
        /// <remarks>The entity to be updated must be attached to the body.</remarks>
        [HttpPut("{entityName}")]
        public async Task<IEntity> Put(string entityName)
        {
            if (entityName == null) throw new ArgumentNullException(nameof(entityName));

            var entityType = TypeResolver.Resolve(entityName);
            var updating = EntityDeserializer.Deserialize(Request.Body, entityType);
            var repository = RepositoryFactory.Construct(entityType);
            await repository.UpdateAsync(updating);

            return updating;
        }
    }
}
