using Murcex.Vyudro.Internal.Audit.Pages;
using Murcex.Vyudro.Module.Client;

namespace Murcex.Vyudro.Internal.Audit.Core
{
	public class Initializer
	{
		public static void Execute(Dictionary<string, Dictionary<string, string>> cfg)
		{
			var siteManager = new VyudroManager();

			siteManager.Initialize(cfg, PageFuncSelector.Execute);

			Configuration.AddSiteManager(siteManager);
		}
	}
}