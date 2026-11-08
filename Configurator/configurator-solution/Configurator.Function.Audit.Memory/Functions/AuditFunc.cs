using Configurator.Audit.Memory.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;

namespace Configurator.Audit.Memory.Functions
{
    public class AuditFunc
    {
        private readonly ILogger<AuditFunc> _logger;

        public AuditFunc(ILogger<AuditFunc> logger) => _logger = logger;

        [Function("Audit")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            FunctionContext context)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string responseMessage = $"Test Result: {string.Equals(Global.TestValue, "TestValue", StringComparison.OrdinalIgnoreCase)}";

            return new OkObjectResult(responseMessage);
        }
    }
}
