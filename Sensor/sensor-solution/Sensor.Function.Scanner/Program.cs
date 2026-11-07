using Configurator;
using KirokuG2;
using Microsoft.Extensions.Hosting;
using Sensor;

var config = ConfiguratorManager.Execute();
SensorManager.Initialize(config);
KManager.Configure(true);

new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .Build()
    .Run();
