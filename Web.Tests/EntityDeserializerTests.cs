using lvl.TestDomain;
using lvl.Web.Serialization;
using lvl.Web.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace lvl.Web.Tests
{
    [Collection(nameof(WebCollection))]
    public class EntityDeserializerTests
    {
        private IServiceProvider Services { get; }

        public EntityDeserializerTests(WebServiceProviderFixture webServiceProviderFixture)
        {
            Services = webServiceProviderFixture?.ServiceProvider ?? throw new ArgumentNullException(nameof(webServiceProviderFixture));
        }

        [Fact]
        public void WhenDeserializing_EntityIsPopulated()
        {
            var entityDeserializer = Services.GetRequiredService<EntityDeserializer>();
            var moon = new Moon { Id = 1 };
            var moonString = JsonConvert.SerializeObject(moon);
            var moonBytes = Encoding.UTF8.GetBytes(moonString);
            using (var moonStream = new MemoryStream(moonBytes))
            {
                var deserialized = entityDeserializer.Deserialize(moonStream, moon.GetType());

                Assert.Equal(deserialized.Id, moon.Id);
            }
        }

        [Fact]
        public void WhenDeserializing_AndEntityTypeIsNull_ArgumentNullExceptionIsThrown()
        {
            var entityDeserializer = Services.GetRequiredService<EntityDeserializer>();
            var moon = new Moon { Id = 1 };
            var moonSerialized = JsonConvert.SerializeObject(moon);
            var moonBytes = Encoding.UTF8.GetBytes(moonSerialized);
            using (var moonStream = new MemoryStream(moonBytes))
            {
                Assert.Throws<ArgumentNullException>(() => entityDeserializer.Deserialize(moonStream, null));
            }
        }

        [Fact]
        public void WhenDeserializing_AndTypeIsntEntity_ArgumentExceptionIsThrown()
        {
            var entityDeserializer = Services.GetRequiredService<EntityDeserializer>();
            var item = new NonEntity();
            var serialized = JsonConvert.SerializeObject(item);
            var bytes = Encoding.UTF8.GetBytes(serialized);
            using (var stream = new MemoryStream(bytes))
            {
                Assert.Throws<ArgumentException>(() => entityDeserializer.Deserialize(stream, item.GetType()));
            }
        }

        [Fact]
        public void WhenDeserializing_AndStreamIsNull_ArgumentNullExceptionIsThrown()
        {
            var entityDeserializer = Services.GetRequiredService<EntityDeserializer>();

            Assert.Throws<ArgumentNullException>(() => entityDeserializer.Deserialize(null, typeof(Moon)));
        }

        [Fact]
        public void WhenDeserializing_AndStreamCannotBeDeserialized_InvalidOperationExceptionIsThrown()
        {
            var entityDeserializer = Services.GetRequiredService<EntityDeserializer>();
            var invalidSerialized = JsonConvert.SerializeObject(@"{invalid: ""true""}");
            var invalidBytes = Encoding.UTF8.GetBytes(invalidSerialized);

            using (var invalidStream = new MemoryStream(invalidBytes))
            {
                Assert.Throws<JsonSerializationException>(() => entityDeserializer.Deserialize(invalidStream, typeof(Moon)));
            }
        }

        public class NonEntity { }
    }
}
