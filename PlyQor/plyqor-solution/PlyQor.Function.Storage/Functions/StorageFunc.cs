using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using PlyQor.Engine;
using System.IO;
using System.Threading.Tasks;

namespace PlyQor.Functions
{
    public class StorageFunc
    {
        [Function("Storage")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var result = PlyQorManager.Query(requestBody);

            return new OkObjectResult(result);
        }
    }
}
