using Configurator;
using KirokuG2;
using KirokuG2.Internal.Loader.Components;
using KirokuG2.Loader;
using KirokuG2.Loader.Components;
using KirokuG2.Processor.Core;
using Microsoft.Extensions.Hosting;

var config = ConfiguratorManager.Execute();
var kirokuConfig = config["kiroku-processor"];
Configuration.Load(kirokuConfig);

LogProvider logProvider = new(Configuration.Storage, new KLogSeralializer());
SQLProvider sqlProvider = new();
sqlProvider.Initialized(Configuration.Database);
KLoaderManager.Configuration(logProvider, sqlProvider);
KManager.Configure(true);

new HostBuilder().ConfigureFunctionsWorkerDefaults().Build().Run();
