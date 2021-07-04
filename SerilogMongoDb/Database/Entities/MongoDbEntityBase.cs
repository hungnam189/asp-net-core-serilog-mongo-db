using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SerilogMongoDb.Database.Entities
{
    /// <summary>
    /// A non-instantiable base entity which defines
    /// members available across all entities.
    /// </summary>
    public abstract class MongoDbEntityBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}