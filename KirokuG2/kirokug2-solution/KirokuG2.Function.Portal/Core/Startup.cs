using Configurator;
using KirokuG2.Function.Portal.Core;
using KirokuG2.Internal.Portal;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace KirokuG2.Function.Portal.Core
{
	internal class Startup : FunctionsStartup
	{
		public override void Configure(IFunctionsHostBuilder builder)
		{
			// execute configurator
			var config = ConfiguratorManager.Execute();

			// load config into application setup
			PortalManager.Initialize(config);

			// setup kiroku logging
			KManager.Configure(true);
		}
	}
}
