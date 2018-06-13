using Newtonsoft.Json;

namespace AspNetCore.Identity.DocumentDb
{
    public abstract class DocumentBase
    {
        [JsonProperty(PropertyName = "documentType")]
        public virtual string DocumentType
        {
            get
            {
                return this.GetType().Name;
            }
        }

        [JsonProperty(PropertyName = "partitionKey")]
        public object PartitionKey { get; set; }
    }
}
