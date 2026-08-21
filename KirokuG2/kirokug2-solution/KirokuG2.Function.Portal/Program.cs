using Configurator;
using KirokuG2;
using KirokuG2.Internal.Portal;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var config = ConfiguratorManager.Execute();
PortalManager.Initialize(config);
KManager.Configure(true);

builder.Build().Run();
