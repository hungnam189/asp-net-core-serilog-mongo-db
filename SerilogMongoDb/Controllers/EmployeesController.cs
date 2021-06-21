using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using SerilogMongoDb.Models;
using SerilogMongoDb.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SerilogMongoDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeesController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Employee>>> Get() =>
            await _employeeRepository.GetAsync().ConfigureAwait(false);

        [HttpGet("{id:length(24)}", Name = "GetEmployee")]
        public async Task<ActionResult<Employee>> GetAsync(string id)
        {
            var emp = await _employeeRepository.GetAsync(id).ConfigureAwait(false);
            return emp == null ? NotFound() : (ActionResult<Employee>)emp;
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreateAsync([FromBody] Employee command)
        {
            var employee = await _employeeRepository.CreateAsync(command).ConfigureAwait(false);
            return Ok(employee);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("{id:length(24)}")]
        public async Task<ActionResult<bool>> UpdateAsync(string id, Employee command)
        {
            var result = await _employeeRepository.UpdateAsync(id, command).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _employeeRepository.DeleteAsync(id).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
