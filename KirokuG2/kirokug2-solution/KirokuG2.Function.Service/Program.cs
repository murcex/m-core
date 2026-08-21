using Configurator;
using KirokuG2.Service.Core;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var config = ConfiguratorManager.Execute();
var kirokuConfig = config["kiroku-service"];
Configuration.Load(kirokuConfig);

builder.Build().Run();
