using Microsoft.AspNetCore.Mvc;
using SerilogMongoDb.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using SerilogMongoDb.Models;

namespace SerilogMongoDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        public CompanyController(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        [HttpGet]
        public async Task<IReadOnlyList<CompanyModel>> Get() =>
            await _companyRepository.GetAllAsync().ConfigureAwait(false);

        [HttpPost]
        public async Task<ActionResult<bool>> CreateAsync([FromBody] CompanyModel command)
        {
            var result = await _companyRepository.InsertAsync(command).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
