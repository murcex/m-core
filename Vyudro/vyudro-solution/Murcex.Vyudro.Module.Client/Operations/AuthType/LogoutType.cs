using KirokuG2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Murcex.Vyudro.Module.Client.Core;
using Murcex.Vyudro.Module.Client.Enum;
using Murcex.Vyudro.Module.Client.Models;
using Murcex.Vyudro.Module.Client.Operations.Error;

namespace Murcex.Vyudro.Module.Client.Operations.AuthType
{
	public class LogoutType
	{
		public static ContentResult Execute(Configuration configuration, HttpRequest request, Page page, IKLog klog, Dictionary<string, string> aux)
		{
			if (configuration.SessionManager.CheckSessionToken(request, out string _, out string checkSessionMessage))
			{
				if (configuration.SessionManager.DisableSessionToken(request, out string token, out string tokenMessage))
				{
					aux.Add("Token", token);

					return page.GeneratePage(request, configuration.GlobalElements, klog, aux);
				}
				else
				{
					return SelectErrorPage.Execute(configuration, ErrorType.Invalid, request, tokenMessage, klog, aux);
				}
			}
			else
			{
				return SelectErrorPage.Execute(configuration, ErrorType.Access, request, checkSessionMessage, klog, aux);
			}
		}
	}
}
