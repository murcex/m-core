using Murcex.PlyQor.Internal.Container;
using Murcex.PlyQor.Internal.Portal.Pages;
using Murcex.Vyudro.Module.Client;

namespace Murcex.PlyQor.Internal.Portal.Core
{
	public class Initializer
	{
		public static void Execute(Dictionary<string, Dictionary<string, string>> cfg)
		{
			PlyQorContainerManager.Initialize(cfg);

			var siteManager = new VyudroManager();
			siteManager.Initialize(cfg, PageFuncSelector.Execute);

			Configuration.AddSiteManager(siteManager);
		}
	}
}
