using Microsoft.AspNetCore.Mvc;
using Murcex.Vyudro.Module.Client.Enum;

namespace Murcex.Vyudro.Module.Client.Operations.Error
{
	internal class GenerateDefaultErrorPage
	{
		public static ContentResult Execute(ErrorType errorType, string message)
		{
			return errorType switch
			{
				ErrorType.NotFound => new ContentResult()
				{
					Content = $"PageNotFound - {message}",
					ContentType = "text/html",
					StatusCode = 404
				},
				ErrorType.Access => new ContentResult()
				{
					Content = $"AccessError - {message}",
					ContentType = "text/html",
					StatusCode = 403
				},
				ErrorType.Invalid => new ContentResult()
				{
					Content = $"InvalidOperation - {message}",
					ContentType = "text/html",
					StatusCode = 400
				},
				_ => new ContentResult()
				{
					Content = $"Exception - {message}",
					ContentType = "text/html",
					StatusCode = 500
				},
			};
		}
	}
}
