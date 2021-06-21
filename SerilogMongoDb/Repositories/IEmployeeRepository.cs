using MongoDB.Bson;
using SerilogMongoDb.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SerilogMongoDb.Repositories
{
    public interface IEmployeeRepository
    {
        // Read
        Task<List<Employee>> GetAsync();
        Task<Employee> GetAsync(string objectId);

        //Create
        Task<string> CreateAsync(Employee employee);

        Task<int> CreateManyAsync(IEnumerable<Employee> employees);

        Task<long> CreateUseBulkWriteAsync(IEnumerable<Employee> employees);

        // Update
        Task<bool> UpdateAsync(string objectId, Employee employee);
        Task<long> UpdateManyEmployeeAsync(IEnumerable<Employee> employees);

        // Delete 
        Task<bool> DeleteAsync(string objectId);
    }
}
