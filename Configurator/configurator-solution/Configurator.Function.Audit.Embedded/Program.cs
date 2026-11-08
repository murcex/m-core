using Configurator;
using Configurator.Audit.Embedded.Core;
using Microsoft.Extensions.Hosting;

var config = ConfiguratorManager.Execute();

if (config.TryGetValue("embedded", out var components))
{
    Global.TestValue = components["TestKey"];
}
else
{
    throw new InvalidOperationException("embedded index not found in config payload");
}

new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .Build()
    .Run();
