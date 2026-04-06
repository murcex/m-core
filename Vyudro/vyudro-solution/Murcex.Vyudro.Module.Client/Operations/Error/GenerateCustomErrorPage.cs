using KirokuG2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Murcex.Implements.DataTools.Extensions;
using Murcex.Vyudro.Module.Client.Core;
using Murcex.Vyudro.Module.Client.Enum;
using Murcex.Vyudro.Module.Client.Operations.Data;

namespace Murcex.Vyudro.Module.Client.Operations.Error
{
	public class GenerateCustomErrorPage
	{
		public static ContentResult Execute(Configuration configuration, HttpRequest request, IKLog klog, Dictionary<string, string> aux, string message, string errorPageName)
		{
			var trackingId = aux.GetValue("TrackingId");

			try
			{
				if (GetErrorPage.Execute(configuration, errorPageName, out var page, out var errorPageMessage))
				{
					aux["Page"] = errorPageName;
					aux["Message"] = message;

					return page.GeneratePage(request, configuration.GlobalElements, klog, aux);
				}
				else
				{
					throw new InvalidOperationException($"{trackingId} => Failed to Generate Error Page: {errorPageMessage}");
				}
			}
			catch (Exception ex)
			{
				klog?.Error($"{trackingId} => Error generating custom error page: {ex.Message}");

				return GenerateDefaultErrorPage.Execute(ErrorType.Exception, trackingId);
			}
		}
	}
}
