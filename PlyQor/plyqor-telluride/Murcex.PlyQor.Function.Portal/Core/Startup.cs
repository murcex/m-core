using Configurator;
using KirokuG2;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Murcex.PlyQor.Function.Portal.Core;
using Murcex.PlyQor.Internal.Portal;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Murcex.PlyQor.Function.Portal.Core
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
