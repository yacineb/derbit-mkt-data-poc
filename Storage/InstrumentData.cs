using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApi.Storage
{
    public class InstrumentData
    {
        [BsonElement("p")]
        public decimal Price { get; set; }
        
        [BsonElement("t")]
        public long Timestamp { get; set; }

        [BsonElement("n")]
        public string Name { get; set; }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}