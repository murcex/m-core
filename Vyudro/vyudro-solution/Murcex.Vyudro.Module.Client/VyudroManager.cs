using KirokuG2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Murcex.Vyudro.Module.Client.Core;
using Murcex.Vyudro.Module.Client.Enum;
using Murcex.Vyudro.Module.Client.Interface;
using Murcex.Vyudro.Module.Client.Models;
using Murcex.Vyudro.Module.Client.Operations.AuthType;
using Murcex.Vyudro.Module.Client.Operations.Data;
using Murcex.Vyudro.Module.Client.Operations.Error;

namespace Murcex.Vyudro.Module.Client
{
	public class VyudroManager
	{
		private Configuration _configuration = new Configuration(null, null, null, null, null);

		public void Initialize(Dictionary<string, Dictionary<string, string>> cfg,
			Func<
			// input
			PageFuncData,
			// return
			Dictionary<string, string>> dynamicPageFunc,
			IStorageAdapter storageAdapter = null)
		{
			_configuration = Initializer.Execute(cfg, dynamicPageFunc, storageAdapter);
		}

		public ContentResult GeneratePage(HttpRequest request, IKLog klog = null)
		{
			var trackingId = Guid.NewGuid().ToString();
			var aux = new Dictionary<string, string>
			{
				{ "TrackingId", trackingId }
			};

			try
			{
				klog ??= new NoLogger();

				klog.Trace($"Tracking Id: {trackingId}");

				if (!GetPage.Execute(_configuration, request, klog, aux, out var page, out var pageMessage))
				{
					return SelectErrorPage.Execute(_configuration, ErrorType.NotFound, request, pageMessage, klog, aux);
				}

				return page.AuthType switch
				{
					PageSecurityType.None => page.GeneratePage(request, _configuration.GlobalElements, klog, aux),
					PageSecurityType.Login => LoginType.Execute(_configuration, request, page, klog, aux),
					PageSecurityType.Logout => LogoutType.Execute(_configuration, request, page, klog, aux),
					PageSecurityType.Token => TokenType.Execute(_configuration, request, page, klog, aux),
					_ => SelectErrorPage.Execute(_configuration, ErrorType.Invalid, request, "InvalidParameter", klog, aux),
				};
			}
			catch (Exception ex)
			{
				return SelectErrorPage.Execute(_configuration, ErrorType.Exception, request, ex.Message, klog, aux);
			}
		}
	}
}
