using KirokuG2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Murcex.Implements.DataTools.Extensions;
using Murcex.Vyudro.Module.Client.Core;
using Murcex.Vyudro.Module.Client.Enum;

namespace Murcex.Vyudro.Module.Client.Operations.Error
{
	public class SelectErrorPage
	{
		public static ContentResult Execute(Configuration configuration, ErrorType errorType, HttpRequest request, string message, IKLog klog, Dictionary<string, string> aux)
		{
			var trackingId = aux.GetValue("TrackingId");

			if (errorType == ErrorType.Exception)
			{
				klog.Error($"{trackingId} => {message}");
			}
			else
			{
				klog.Trace($"{trackingId} => {errorType} => {message}");
			}

			if (configuration.ErrorPages.TryGetValue(errorType, out var pageName))
			{
				return GenerateCustomErrorPage.Execute(configuration, request, klog, aux, message, pageName);
			}
			else
			{
				return GenerateDefaultErrorPage.Execute(errorType, trackingId);
			}
		}
	}
}
