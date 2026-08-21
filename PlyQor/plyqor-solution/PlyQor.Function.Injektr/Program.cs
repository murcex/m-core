using Configurator;
using KirokuG2;
using Microsoft.Extensions.Hosting;
using PlyQor.Injektr.Core;

var config = ConfiguratorManager.Execute();
Configuration.Load(config);
KManager.Configure(true);

new HostBuilder().ConfigureFunctionsWorkerDefaults().Build().Run();
