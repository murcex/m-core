namespace ConfiguratorApp.Functions
{
    using Configurator.Storage;
    using KirokuG2;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using System;

    public class ConfiguratorFunc
    {
        private readonly ILogger<ConfiguratorFunc> _logger;

        public ConfiguratorFunc(ILogger<ConfiguratorFunc> logger)
        {
            _logger = logger;
        }

        [Function("Configurator")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            try
            {
                using (var klog = KManager.NewInstance("Configurator-Storage"))
                {
                    try
                    {
                        string cfkKey = req.Query["key"];

                        string cfgApp = req.Query["app"];

                        var document = CfgSvcManager.Execute(cfkKey, cfgApp, klog);

                        return new OkObjectResult(document);
                    }
                    catch (Exception ex)
                    {
                        klog.Error(ex.ToString());

                        return new OkObjectResult(null);
                    }
                }
            }
            catch (Exception ex)
            {
                KManager.Critical(ex.ToString());

                return new OkObjectResult(null);
            }
        }
    }
}
