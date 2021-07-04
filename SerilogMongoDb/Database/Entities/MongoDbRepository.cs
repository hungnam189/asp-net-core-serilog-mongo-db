using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SerilogMongoDb.Database.Entities
{
    /// <summary>
    /// A MongoDB repository. Maps to a collection with the same name
    /// as type TEntity.
    /// </summary>
    /// <typeparam name=”T”>Entity type for this repository</typeparam>
    public abstract class MongoDbRepository<TEntity> : IMongoDbRepository<TEntity> where TEntity : MongoDbEntityBase
    {
        private readonly IMongoCollection<TEntity> _collection;

        protected MongoDbRepository(ExtMongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            System.Attribute[] attrs = System.Attribute.GetCustomAttributes(typeof(TEntity));
            BsonCollectionAttribute bJsonAttribute = attrs != null ? (BsonCollectionAttribute)attrs[0] : null;
            var collectionName = bJsonAttribute != null ? bJsonAttribute.CollectionName : typeof(TEntity).Name;
            _collection = database.GetCollection<TEntity>(collectionName);
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync()
        {
            var result = await _collection.FindAsync(_ => true).ConfigureAwait(false);
            return await result.ToListAsync().ConfigureAwait(false);
        }

        public async Task<TEntity> GetByIdAsync(string id)
        {
            var result = await _collection.FindAsync(p => p.Id == id).ConfigureAwait(false);
            return await result.FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var filter = Builders<TEntity>.Filter.Eq(p => p.Id, id);
            var result = await _collection.DeleteOneAsync(filter).ConfigureAwait(false);
            return result.DeletedCount == 1;
        }


        public async Task<bool> InsertAsync(TEntity entity)
        {
            await _collection.InsertOneAsync(entity).ConfigureAwait(false);
            return !string.IsNullOrEmpty(entity.Id);
        }

        public async Task<bool> UpdateAsync(string id, TEntity entity)
        {
            if (string.IsNullOrEmpty(id))
                return await InsertAsync(entity).ConfigureAwait(false);

            var filter = Builders<TEntity>.Filter.Eq(emp => emp.Id, id);
            var replaceOne = await _collection.ReplaceOneAsync(filter, entity);
            return replaceOne.ModifiedCount == 1;
        }

        public async Task CreateManyAsync(IEnumerable<TEntity> entities)
        {
            await _collection.InsertManyAsync(entities).ConfigureAwait(false);
        }

        public async Task<long> CreateUseBulkWriteAsync(IEnumerable<TEntity> entities)
        {
            var bulkInserts = new List<WriteModel<TEntity>>();
            var addRanges = entities.Select(entity => new InsertOneModel<TEntity>(entity));
            bulkInserts.AddRange(addRanges);
            var result = await _collection.BulkWriteAsync(bulkInserts).ConfigureAwait(false);
            return result.InsertedCount;
        }
    }
}
