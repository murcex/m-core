using Murcex.PlyQor.Internal.Container.Storage;

namespace Murcex.PlyQor.Internal.Container.Operations
{
	public class ListContainers
	{
		public static List<string> Execute()
		{
			var containerConfigs = DownloadContainers.Execute();
			return containerConfigs.Select(container => container.Name).ToList();
		}
	}
}
