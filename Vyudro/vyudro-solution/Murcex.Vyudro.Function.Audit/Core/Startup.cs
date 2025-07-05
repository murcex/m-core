using Configurator;
using KirokuG2;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Murcex.Vyudro.Function.Audit.Core;
using Murcex.Vyudro.Internal.Audit;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Murcex.Vyudro.Function.Audit.Core
{
	public class Startup : FunctionsStartup
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
