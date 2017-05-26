using Newtonsoft.Json;

namespace lvl.Web.OData
{
    /// <summary>
    ///     Used for serialization/deserialization.
    /// </summary>
    public class ODataResponse
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string Context { get; set; }

        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
    }
}
