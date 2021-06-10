using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Versioning;

namespace SerilogMongoDb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ContadorController : ControllerBase
    {
        private readonly Contador _CONTADOR = new Contador();
        private readonly ILogger _logger;

        public ContadorController()
        {
            _logger =  Log.ForContext<ContadorController>();
        }

        [HttpGet]
        public object Get([FromServices] IConfiguration configuration)
        {

            lock (_CONTADOR)
            {
                _CONTADOR.Incrementar();

                var result = new
                {
                    _CONTADOR.ValorAtual,
                    Environment.MachineName,
                    Local = "Teste",
                    Sistema = Environment.OSVersion.VersionString,
                    Variavel = configuration["TesteAmbiente"],
                    TargetFramework = Assembly
                        .GetEntryAssembly()?
                        .GetCustomAttribute<TargetFrameworkAttribute>()?
                        .FrameworkName
                };
                _logger.ForContext("RequestInput", result)
                       .Information("Contador API");

                return result;
            }
        }
    }
}
