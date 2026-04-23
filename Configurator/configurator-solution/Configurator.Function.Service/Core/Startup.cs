using Configurator;
using Configurator.Storage;
using KirokuG2;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(ConfiguratorApp.Core.Startup))]

namespace ConfiguratorApp.Core
{
	public class Startup : FunctionsStartup
	{
		public override void Configure(IFunctionsHostBuilder builder)
		{
			var config = ConfiguratorManager.Execute();

			CfgSvcManager.Initialize(config);

			KManager.Configure(true);
		}
	}
}
