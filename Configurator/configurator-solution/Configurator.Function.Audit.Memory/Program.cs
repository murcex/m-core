using Configurator;
using Configurator.Audit.Memory.Core;
using Microsoft.Extensions.Hosting;

var config = ConfiguratorManager.Execute();

if (config.TryGetValue("default", out var components) &&
    components.TryGetValue("TestKey", out var test))
{
    Global.TestValue = test;
}

new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .Build()
    .Run();
