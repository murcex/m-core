using KirokuG2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Murcex.Vyudro.Module.Client.Core;
using Murcex.Vyudro.Module.Client.Enum;
using Murcex.Vyudro.Module.Client.Models;
using Murcex.Vyudro.Module.Client.Operations.Error;

namespace Murcex.Vyudro.Module.Client.Operations.AuthType
{
	internal class TokenType
	{
		public static ContentResult Execute(Configuration configuration, HttpRequest request, Page page, IKLog klog, Dictionary<string, string> aux)
		{
			if (configuration.SessionManager.CheckSessionToken(request, out var token, out var message))
			{
				aux.Add("Token", token);

				return page.GeneratePage(request, configuration.GlobalElements, klog, aux);
			}
			else
			{
				klog.Trace($"Token is invalid: {message}");

				return SelectErrorPage.Execute(configuration, ErrorType.Access, request, message, klog, aux);
			}
		}
	}
}
