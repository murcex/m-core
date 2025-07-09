using KirokuG2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Murcex.Vyudro.Module.Client.Core;
using Murcex.Vyudro.Module.Client.Enum;
using Murcex.Vyudro.Module.Client.Interface;
using Murcex.Vyudro.Module.Client.Models;

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
			try
			{
				var aux = new Dictionary<string, string>();

				if (klog == null)
				{
					klog = IgnoreLogging();
				}

				if (GetPage(request, out var page, out var pageMessage))
				{
					klog.Trace(pageMessage);
					aux.Add("Page", page.Name);
				}
				else
				{
					return GenerateErrorPage(ErrorType.NotFound, request, pageMessage, klog);
				}

				if (page.AuthType == PageSecurityType.None)
				{
					return page.GeneratePage(request, _configuration.GlobalElements, klog, aux);
				}
				else if (page.AuthType == PageSecurityType.Login)
				{
					if (_configuration.SessionManager.CheckAccessToken(request, out string checkAccessMessage))
					{
						if (_configuration.SessionManager.CreateSessionToken(request, out string token, out string createTokenMessage))
						{
							aux.Add("Token", token);

							return page.GeneratePage(request, _configuration.GlobalElements, klog, aux);
						}
						else
						{
							return GenerateErrorPage(ErrorType.Exception, request, createTokenMessage, klog);
						}
					}
					else
					{
						return GenerateErrorPage(ErrorType.Access, request, checkAccessMessage, klog);
					}
				}
				else if (page.AuthType == PageSecurityType.Logout)
				{
					if (_configuration.SessionManager.CheckSessionToken(request, out string _, out string checkSessionMessage))
					{
						if (_configuration.SessionManager.DisableSessionToken(request, out string token, out string tokenMessage))
						{
							aux.Add("Token", token);

							return page.GeneratePage(request, _configuration.GlobalElements, klog, aux);
						}
						else
						{
							return GenerateErrorPage(ErrorType.Invalid, request, tokenMessage, klog);
						}
					}
					else
					{
						return GenerateErrorPage(ErrorType.Access, request, checkSessionMessage, klog);
					}
				}
				else if (page.AuthType == PageSecurityType.Token)
				{
					if (_configuration.SessionManager.CheckSessionToken(request, out var token, out var message))
					{
						aux.Add("Token", token);

						return page.GeneratePage(request, _configuration.GlobalElements, klog, aux);
					}
					else
					{
						return GenerateErrorPage(ErrorType.Access, request, message, klog);
					}
				}
				else
				{
					return GenerateErrorPage(ErrorType.Invalid, request, "InvalidParameter", klog);
				}
			}
			catch (Exception ex)
			{
				return GenerateErrorPage(ErrorType.Exception, request, ex.Message, klog);
			}
		}

		private bool GetPage(HttpRequest request, out Page page, out string message)
		{
			var pageName = request.Query["page"];

			if (string.IsNullOrEmpty(pageName))
			{
				page = null;
				message = "Page Name Not Found in Request";
				return false;
			}

			if (_configuration.PageCache.TryGetValue(pageName, out page))
			{
				message = $"Generating Page {pageName}";
				return true;
			}
			else
			{
				message = $"Page {pageName} Not Found";
				return false;
			}
		}

		private ContentResult GenerateErrorPage(ErrorType errorType, HttpRequest request, string message, IKLog klog)
		{
			if (errorType == ErrorType.Exception)
			{
				klog.Error(message);
			}
			else
			{
				klog.Trace($"{errorType} - {message}");
			}

			if (_configuration.ErrorPages.TryGetValue(errorType, out var pageName))
			{
				return GenerateErrorPage(request, klog, message, pageName);
			}
			else
			{
				return DefaultErrorPage(errorType);
			}
		}

		private IKLog IgnoreLogging()
		{
			return new NoLogger();
		}

		private ContentResult GenerateErrorPage(HttpRequest request, IKLog klog, string message, string errorPageName)
		{
			var aux = new Dictionary<string, string>();

			try
			{
				if (GetErrorPage(errorPageName, out var page, out var erroPageMessage))
				{
					aux.Add("Page", errorPageName);
					aux.Add("Message", message);

					return page.GeneratePage(request, _configuration.GlobalElements, klog, aux);
				}
				else
				{
					throw new InvalidOperationException($"Failed to Generate Error Page: {erroPageMessage}");
				}
			}
			catch (Exception ex)
			{
				return DefaultErrorPage(ErrorType.Exception, ex.Message);
			}
		}

		private bool GetErrorPage(string errorPageName, out Page page, out string message)
		{
			if (_configuration.PageCache.TryGetValue(errorPageName, out page))
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

		private ContentResult DefaultErrorPage(ErrorType errorType, string message = null)
		{
			if (errorType == ErrorType.NotFound)
			{
				return new ContentResult()
				{
					Content = "PageNotFound",
					ContentType = "text/html",
					StatusCode = 400
				};
			}

			if (errorType == ErrorType.Access)
			{
				return new ContentResult()
				{
					Content = string.IsNullOrEmpty(message) ? "AccessError" : message,
					ContentType = "text/html",
					StatusCode = 400
				};
			}

			if (errorType == ErrorType.Invalid)
			{
				return new ContentResult()
				{
					Content = "InvalidParameter",
					ContentType = "text/html",
					StatusCode = 400
				};
			}

			return new ContentResult()
			{
				Content = message,
				ContentType = "text/html",
				StatusCode = 500
			};
		}
	}
}
