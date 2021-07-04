using SerilogMongoDb.Database;
using SerilogMongoDb.Database.Entities;

namespace SerilogMongoDb.Models
{
    [BsonCollection("Companies")]
    public class CompanyModel : MongoDbEntityBase
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
}
