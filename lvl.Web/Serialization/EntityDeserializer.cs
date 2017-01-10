using lvl.Ontology;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;

namespace lvl.Web.Serialization
{
    public class EntityDeserializer
    {
        private JsonSerializerSettings JsonSerializerSettings { get; }

        public EntityDeserializer(IOptions<JsonSerializerSettings> jsonSerializerSettingsOptions)
        {
            if (jsonSerializerSettingsOptions == null || jsonSerializerSettingsOptions.Value == null)
            {
                throw new ArgumentNullException(nameof(jsonSerializerSettingsOptions));
            }

            JsonSerializerSettings = jsonSerializerSettingsOptions.Value;
        }

        /// <summary>
        /// Provides a way to convert a stream to an entity.
        /// </summary>
        /// <param name="entityType">The type of entity which will be constructed.</param>
        /// <param name="deserializing">The stream whos data will poulate the entity.</param>
        /// <returns>The deserialized entity.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entityType"/> is null.</exception> 
        /// <exception cref="ArgumentNullException"><paramref name="deserializing"/> is null.</exception> 
        /// <exception cref="ArgumentException"><paramref name="entityType"/> does not implement from IEntity.</exception>
        /// <exception cref="JsonSerializationException">The stream cannot be deserialized to the given type.</exception>
        public IEntity Deserialize(Stream deserializing, Type entityType)
        {
            if (deserializing == null)
            {
                throw new ArgumentNullException(nameof(deserializing));
            }
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }
            if (!typeof(IEntity).IsAssignableFrom(entityType))
            {
                throw new ArgumentException($"cannot deserialize {entityType}, as it does not implement {nameof(IEntity)}");
            }

            using (var streamReader = new StreamReader(deserializing))
            {
                var serialized = streamReader.ReadToEnd();
                if (string.IsNullOrWhiteSpace(serialized))
                {
                    throw new ArgumentNullException(nameof(deserializing));
                }
                return (IEntity)JsonConvert.DeserializeObject(serialized, entityType, JsonSerializerSettings);
            }
        }
    }
}
