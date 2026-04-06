using KirokuG2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Murcex.Vyudro.Module.Client.Core;
using Murcex.Vyudro.Module.Client.Enum;
using Murcex.Vyudro.Module.Client.Models;
using Murcex.Vyudro.Module.Client.Operations.Error;

namespace Murcex.Vyudro.Module.Client.Operations.AuthType
{
	public class LoginType
	{
		public static ContentResult Execute(Configuration configuration, HttpRequest request, Page page, IKLog klog, Dictionary<string, string> aux)
		{
			if (configuration.SessionManager.CheckAccessToken(request, out string checkAccessMessage))
			{
				if (configuration.SessionManager.CreateSessionToken(request, out string token, out string createTokenMessage))
				{
					klog.Trace($"Token Created: {token} <- {createTokenMessage}");
					aux.Add("Token", token);

					return page.GeneratePage(request, configuration.GlobalElements, klog, aux);
				}
				else
				{
					return SelectErrorPage.Execute(configuration, ErrorType.Exception, request, createTokenMessage, klog, aux);
				}
			}
			else
			{
				return SelectErrorPage.Execute(configuration, ErrorType.Access, request, checkAccessMessage, klog, aux);
			}
		}
	}
}
