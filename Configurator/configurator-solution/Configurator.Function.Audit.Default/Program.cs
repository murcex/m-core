using Configurator;
using Configurator.Audit.Default.Core;
using Microsoft.Extensions.Hosting;

const string configPath = @"D:\home\data\app\cfg\Config.ini";

if (File.Exists(configPath))
{
    File.Delete(configPath);
}

if (Directory.Exists(@"D:\home\data\app"))
{
    Directory.Delete(@"D:\home\data\app", true);
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
