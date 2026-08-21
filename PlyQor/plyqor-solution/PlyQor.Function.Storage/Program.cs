using Configurator;
using Microsoft.Extensions.Hosting;
using PlyQor.Engine;

var config = ConfiguratorManager.Execute();
PlyQorManager.Initialize(config);

new HostBuilder().ConfigureFunctionsWebApplication().Build().Run();
