using KirokuG2;
using Microsoft.Extensions.Hosting;

KManager.Configure(false);

new HostBuilder().ConfigureFunctionsWorkerDefaults().Build().Run();
