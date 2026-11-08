using Configurator;
using Configurator.Storage;
using KirokuG2;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;

var config = ConfiguratorManager.Execute();

CfgSvcManager.Initialize(config);

KManager.Configure(true);

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .Build();

host.Run();

