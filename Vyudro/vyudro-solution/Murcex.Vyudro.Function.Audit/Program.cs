using Configurator;
using KirokuG2;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Murcex.Vyudro.Internal.Audit;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var config = ConfiguratorManager.Execute();
PortalManager.Initialize(config);
KManager.Configure(true);

builder.Build().Run();
