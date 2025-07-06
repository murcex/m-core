using Configurator;
using KirokuG2.Function.Portal.Core;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
