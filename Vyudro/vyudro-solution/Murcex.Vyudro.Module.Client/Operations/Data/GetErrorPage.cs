using Murcex.Vyudro.Module.Client.Core;
using Murcex.Vyudro.Module.Client.Models;

namespace Murcex.Vyudro.Module.Client.Operations.Data
{
	public class GetErrorPage
	{
		public static bool Execute(Configuration configuration, string errorPageName, out Page page, out string message)
		{
			if (configuration.PageCache.TryGetValue(errorPageName, out page))
			{
				message = $"Generating Page {errorPageName}";
				return true;
			}
			else
			{
				message = $"Page {errorPageName} Not Found";
				return false;
			}
		}
	}
}
