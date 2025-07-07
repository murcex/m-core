using KirokuG2.Internal.Portal.Pages;
using Murcex.Vyudro.Module.Client;

namespace KirokuG2.Internal.Portal.Core
{
	public class Initializer
	{
		public static void Execute(Dictionary<string, Dictionary<string, string>> cfg)
		{
			var siteManager = new VyudroManager();

			Configuration.Load(cfg);

			siteManager.Initialize(cfg, PageFuncSelector.Execute);

			Configuration.AddSiteManager(siteManager);
		}
	}
}