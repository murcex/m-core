using Murcex.Implements.DataTools.Extensions;
using Murcex.Vyudro.Module.Client.Models;

namespace Murcex.PlyQor.Internal.Portal.Pages.Container;

public class ContainerCreatePage
{
	public static Dictionary<string, string> Execute(PageFuncData pageFuncData)
	{
		var dynamicElements = new Dictionary<string, string>();

		var token = pageFuncData.Auxiliary.GetValue("Token");

		dynamicElements.Add("dym-token", token);

		return dynamicElements;
	}
}
