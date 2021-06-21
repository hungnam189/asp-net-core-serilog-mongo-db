using MongoDB.Bson;
using MongoDB.Driver;
using SerilogMongoDb.Database;
using SerilogMongoDb.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SerilogMongoDb.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IMongoCollection<Employee> _employees;
        public EmployeeRepository(ExtMongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _employees = database.GetCollection<Employee>(settings.CollectionName);
        }

        #region Read
        public async Task<List<Employee>> GetAsync()
        {
            var result = await _employees.FindAsync(_ => true).ConfigureAwait(false);
            return await result.ToListAsync().ConfigureAwait(false);
        }

        public async Task<Employee> GetAsync(string objectId)
        {
            var result = await _employees.FindAsync(emp => emp.Id == objectId).ConfigureAwait(false);
            return await result.FirstOrDefaultAsync().ConfigureAwait(false);
        }
        #endregion

        #region Create
        public async Task<string> CreateAsync(Employee employee)
        {
            await _employees.InsertOneAsync(employee).ConfigureAwait(false);
            return employee.Id;
        }
        #endregion

        public async Task<bool> UpdateAsync(string objectId, Employee employee)
        {
            var filter = Builders<Employee>.Filter.Eq(emp => emp.Id, objectId);
            var update = Builders<Employee>.Update
                .Set(emp => emp.Name, employee.Name)
                .Set(emp => emp.Phone, employee.Phone)
                .Set(emp => emp.Address, employee.Address)
                .Set(emp => emp.Email, employee.Email);

            var result = await _employees.UpdateOneAsync(filter, update).ConfigureAwait(false);

            return result.ModifiedCount == 1;
        }

        public async Task<bool> DeleteAsync(string objectId)
        {
            var filter = Builders<Employee>.Filter.Eq(emp => emp.Id, objectId);
            var result = await _employees.DeleteOneAsync(filter).ConfigureAwait(false);
            return result.DeletedCount == 1;
        }
    }
}
