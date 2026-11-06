using Configurator;
using Configurator.Audit.Default.Core;
using Microsoft.Extensions.Hosting;

const string configPath = @"C:\home\data\app\cfg\Config.ini";

if (File.Exists(configPath))
{
    File.Delete(configPath);
}

if (Directory.Exists(@"C:\home\data\app"))
{
    Directory.Delete(@"C:\home\data\app", true);
}

var config = ConfiguratorManager.Execute();

if (config.TryGetValue("default", out var components) &&
    components.TryGetValue("TestKey", out var test))
{
    Global.TestValue = test;
}

Global.ConfigExists = File.Exists(configPath);

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .Build();

host.Run();
