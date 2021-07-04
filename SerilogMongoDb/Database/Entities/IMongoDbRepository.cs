using System.Collections.Generic;
using System.Threading.Tasks;

namespace SerilogMongoDb.Database.Entities
{
    public interface IMongoDbRepository<TEntity> where TEntity : MongoDbEntityBase
    {
        Task<bool> InsertAsync(TEntity entity);
        Task<bool> UpdateAsync(string id, TEntity entity);
        Task<bool> DeleteAsync(string id);
        Task<IReadOnlyList<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(string id);

        Task CreateManyAsync(IEnumerable<TEntity> entities);
        Task<long> CreateUseBulkWriteAsync(IEnumerable<TEntity> entities);
    }
}
