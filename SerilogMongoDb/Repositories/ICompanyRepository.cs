using SerilogMongoDb.Database;
using SerilogMongoDb.Database.Entities;
using SerilogMongoDb.Models;

namespace SerilogMongoDb.Repositories
{
    public interface ICompanyRepository : IMongoDbRepository<CompanyModel>
    {
    }

    public class CompanyRepository : MongoDbRepository<CompanyModel>, ICompanyRepository
    {
        public CompanyRepository(ExtMongoDbSettings settings) : base(settings)
        {
        }
    }
}
