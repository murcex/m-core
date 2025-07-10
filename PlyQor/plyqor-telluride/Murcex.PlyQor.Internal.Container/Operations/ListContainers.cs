using Murcex.PlyQor.Internal.Container.Storage;

namespace Murcex.PlyQor.Internal.Container.Operations
{
	public class ListContainers
	{
		public static List<string> Execute()
		{
			List<string> result = new();

			var containerConfigs = DownloadContainers.Execute();

			foreach (var container in containerConfigs)
			{
				result.Add(container.Name);
			}

			return result;
		}
	}
}
