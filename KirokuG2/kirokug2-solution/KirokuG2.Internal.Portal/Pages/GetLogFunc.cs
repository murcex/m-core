using KirokuG2.Internal.Portal.Core;
using Murcex.Implements.DataTools.Extensions;
using Murcex.Vyudro.Module.Client.Models;
using System.Net.Security;

namespace KirokuG2.Internal.Portal.Pages
{
	public class GetLogFunc
	{
		private static HttpClient _httpClient = new HttpClient(GetHttpClientHandler());

		public static Dictionary<string, string> Execute(PageFuncData pageFuncData)
		{
			pageFuncData.KLog.Trace($"Executing Get Log Func");

			var id = pageFuncData.Request.Query["id"];

			string url = Configuration.KirokuUrl + id;

			var dynamicElements = new Dictionary<string, string>
			{
				{ "dym-token", pageFuncData.Auxiliary.GetValue("Token") }
			};

			var content = string.Empty;

			try
			{
				content = _httpClient.GetStringAsync(url).GetAwaiter().GetResult();
				content = content.Replace("\r\n", "<br>");
			}
			catch (HttpRequestException e)
			{
				pageFuncData.KLog.Error(e.Message);
				content = $"Exception on KQuery request";
			}

			dynamicElements.Add("dym-log", content);

			return dynamicElements;
		}

		private static SocketsHttpHandler GetHttpClientHandler()
		{
			var sslOptions = new SslClientAuthenticationOptions
			{
				RemoteCertificateValidationCallback = (message, certificate, chain, sslErrors) =>
				{
					// direct to url with matching server certificate
					if (sslErrors.ToString() == "None")
					{
						return true;
					}

					// support Azure Function redirect
					if (sslErrors.ToString() == "RemoteCertificateNameMismatch")
					{

						return true;
						//	if (certificate == null)
						//	{
						//		throw new Exception($"Server Certificate Validation Failure: Certificate is Null");
						//	}
						//	else
						//	{
						//		// Issed by Microsoft
						//		if (certificate.Issuer.Contains("O=Microsoft Corporation")

						//		// Azure Function Host URL
						//		&& certificate.Subject.Contains("CN=*.azurewebsites.net"))
						//		{
						//			return true;
						//		}
						//		else
						//		{
						//			throw new Exception($"Server Certificate Validation Failure: Not Trusted Certificate - {certificate.Issuer}, {certificate.Subject}");
						//		}
						//	}
						//}

						//if (certificate == null)
						//{
						//	throw new Exception($"Server Certificate Validation Failure: {sslErrors}");
						//}
						//else
						//{
						//	throw new Exception($"Server Certificate Validation Failure: {sslErrors}, {certificate.Issuer}, {certificate.Subject}");
					}

					return true;
				}
			};

			var socketHandler = new SocketsHttpHandler
			{
				PooledConnectionLifetime = TimeSpan.FromMinutes(1),

				SslOptions = sslOptions
			};

			return socketHandler;
		}
	}
}
